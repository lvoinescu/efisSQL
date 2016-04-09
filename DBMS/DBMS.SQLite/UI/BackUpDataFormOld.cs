/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DBMS.SQLite;
using System.IO;
using System.Data.SQLite;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DBMS.core;

namespace DBMS.SQLite
{
    public partial class BackUpDataFormOld : Form
    {
        private bool advanced = false;
        private string[] dataBases;
        private string[] tables;
        private string[] columnsType;
        private string selectedTable;
        private string selectedDataBase;
        private SQLiteConnection connection;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter ad;
        private string delimiter = ";;";
        private bool mustStop;
        SQLiteDBBrowser browser;
        PropertyExporter backupSettings;

        private enum ExportMode { StructureAndData, StructureOnly, DataOnly };

        public SQLiteDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        public BackUpDataFormOld()
        {
            InitializeComponent();
        }

        public BackUpDataFormOld(SQLiteConnection connection, string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
        {
            backupSettings = new PropertyExporter();
            this.dataBases = dataBases;
            this.connection = connection;
            this.tables = tables;
            this.selectedTable = selectedTable;
            this.selectedDataBase = selectedDataBase;
            InitializeComponent();
            checkBox15.CheckedChanged -= checkBox15_CheckedChanged;
            for (int i = 0; i < tables.Length; i++)
            {
                tableList.Items.Add(tables[i]);
                if (tables[i] == selectedTable)
                    tableList.Items[i].Selected = true;
            }
            checkBox15.CheckedChanged += checkBox15_CheckedChanged;
        }

        private void SetWidthAdv(bool mod)
        {
            if (!mod)
            {
                this.Width = propertyGrid1.Left - 2;
                button4.Text = "Advanced";
            }
            else
            {
                this.Width = propertyGrid1.Left + propertyGrid1.Width + 2;
                button4.Text = "Simple";
            }
        }

        private void BackUpDataForm_Load(object sender, EventArgs e)
        {
            advanced = false;
            SetWidthAdv(advanced);
            QueryResult qr = this.browser.ListTables(selectedDataBase);
            if (qr.ErrorNo == 0)
            {
                tableList.Items.Clear();
                string[] tables = (string[])qr.Result;
                if (tables == null)
                    return;
                for (int i = 0; i < tables.Length; i++)
                    tableList.Items.Add(new ListViewItem(tables[i], tables[i]));
            }

            tableList.Columns[0].Width = tableList.Width - 10;
            propertyGrid1.SelectedObject = backupSettings;
            checkBox15.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sFile = new SaveFileDialog();
            sFile.Filter = "Text files|*.txt|Sql files|*.sql";
            sFile.ShowDialog();
            fileText.Text = sFile.FileName;
        }




        private void WriteDataWithFormat(TextWriter fs, object data, string columnType)
        {
            try
            {
                string ret = "";
                switch (columnType.ToLower())
                {
                    case "bigint":
                    case "currency":
                    case "dec":
                    case "double":
                    case "double precision":
                    case "float":
                    case "int":
                    case "int64":
                    case "integer":
                    case "largeint":
                    case "money":
                    case "number":
                    case "numeric":
                    case "samllint":
                    case "smallmoney":
                        fs.Write(data.ToString());
                        break;
                    case "binary":
                        fs.Write("'" + Encoding.Default.GetString((byte[])data) + "'");
                        break;
                    case "char":
                    case "ntext":
                    case "nvarchar":
                    case "nvarchar2":
                    case "text":
                    case "varchar":
                    case "varchar2":
                        fs.Write("'");
                        WriteEscapeData(fs, (string)data);
                        fs.Write("'");
                        break;
                    case "time":
                        fs.Write("'" + data + "'");
                        break;
                    case "datetime":
                    case "timestamp":
                        DateTime v = (DateTime)data;
                        string dt = v.ToShortDateString();//string.Format("{0}-{1}-{2} {3}:{4}:{5}", v.Year, v.Month, v.Day, v.Hour, v.Minute, v.Second);
                        fs.Write("'" + dt + "'");
                        break;
                    case "date":
                        fs.Write("'");
                        string ts = ((DateTime)data).ToShortDateString(); //((mysqlLib.Data.Types.MySqlDateTime)data).Value.ToString("yyyy-MM-dd");
                        fs.Write(ts);
                        fs.Write("'");
                        break;
                    case "blob":
                    case "blob_text":
                    case "clob":
                    case "graphic":
                    case "image":
                    case "memo":
                    case "photo":
                    case "picture":
                    case "raw":
                        fs.Write("'");
                        WriteEscapeData(fs, (byte[])data);
                        fs.Write("'");
                        break;
                    default:
                        ret = Encoding.Default.GetString((byte[])data);
                        fs.Write("'");
                        WriteEscapeData(fs, ret);
                        fs.Write("'");
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void WriteEscapeData(TextWriter fs, byte[] data)
        {
            int l = data.Length;
            int k = 0;
            int bufferSize = 128;
            if (data.Length < bufferSize)
            {
                WriteEscapeData(fs, Encoding.Default.GetString(data, k, data.Length));
                return;
            }
            int n = (int)l / bufferSize;
            while (k < n)
            {
                string s = Encoding.Default.GetString(data, k * bufferSize, bufferSize);
                WriteEscapeData(fs, s);
                k++;
            }
            if (k >= n)
            {
                string s = Encoding.Default.GetString(data, k * bufferSize, l - k * bufferSize);
                WriteEscapeData(fs, s);

            }
        }

        private void WriteEscapeData(TextWriter fs, string data)
        {

            char[] buf = data.ToCharArray();
            DateTime start1 = DateTime.Now;
            Regex r = new Regex("[']");

            MatchCollection mc = r.Matches(data);
            string sdfsdf = "";
            char[] tt = sdfsdf.ToCharArray();
            if (mc.Count > 0)
            {
                int k = 0;
                for (int i = 0; i < mc.Count; i++)
                {
                    string aux = mc[i].ToString();
                    fs.Write(buf, k, mc[i].Index - k);
                    k = mc[i].Index + 1;
                    switch (buf[mc[i].Index])
                    {
                        //case '\0':
                        //    fs.Write("\\0");
                        //    break;
                        case '\'':
                            fs.Write("\'\'");
                            break;
                        //case '\"':
                        //    fs.Write("\\\"");
                        //    break;
                        ////case '\b':
                        ////    fs.Write("\\b");
                        ////    break;
                        //case '\n':
                        //    fs.Write("\\n");
                        //    break;
                        //case '\r':
                        //    fs.Write("\\r");
                        //    break;

                        //case (char)26:
                        //    fs.Write(@"\Z");
                        //    break;
                        //case '\\':
                        //    fs.Write("\\\\");
                        //    break;

                        //// this ones does not require escapeing

                        ////case '\t':
                        ////    fs.Write("\\t");
                        ////    break;
                        ////case '%':
                        ////    fs.Write("\\%");
                        ////    break;
                        ////case '_':
                        ////    fs.Write("\\_");
                        ////    break;
                    }

                }
                if (mc[mc.Count - 1].Index < data.Length - 1)
                {
                    fs.Write(buf, mc[mc.Count - 1].Index + 1, data.Length - mc[mc.Count - 1].Index - 1);
                }
                DateTime stop1 = DateTime.Now;
                TimeSpan t = stop1.Subtract(start1);
            }
            else
                fs.Write(data);
        }

        private void WriteInsertBlockToFile(TextWriter fs, string table, string[] cols, string[] columnsType,object[] toInsert)
        {
            fs.Write("insert into `");
            fs.Write(table);
            fs.Write("` (");
            fs.Write("`" + cols[0] + "`");
            for (int u = 1; u < cols.Length; u++)
            {
                fs.Write(",`");
                fs.Write(cols[u]);
                fs.Write("`");
            }
            fs.Write(") values ");

            fs.Write("(");
            if (toInsert[0] != DBNull.Value)
            {
                WriteDataWithFormat(fs, toInsert[0], columnsType[0]);
            }
            else
            {
                fs.Write("null");
            }
            for (int j = 1; j < cols.Length; j++)
            {
                fs.Write(",");
                if (toInsert[j] != DBNull.Value)
                {
                    WriteDataWithFormat(fs, toInsert[j], columnsType[j]);
                }
                else
                {
                    fs.Write("null");
                }
            }
            fs.Write(")");
            fs.Write(";");
            fs.WriteLine();

        }

        private void WriteInsertSyntaxTable(TextWriter fs, string dataBase, string tableName)
        {
            fs.WriteLine();
            fs.Write("/*Data for table " + tableName + "` */" + Environment.NewLine);
            SQLiteDataReader reader = null;
            QueryResult qR = browser.GetColumnInformation(dataBase, tableName);
            DataTable cTable = (DataTable)qR.Result;
            columnsType = new string[cTable.Rows.Count];
            Regex r = new Regex("\\w+");
            int i = 0;
            for (i = 0; i < cTable.Rows.Count; i++)
                columnsType[i] = r.Matches(cTable.Rows[i]["type"].ToString())[0].Value;
            QueryResult qr = browser.ListColumns(dataBase, tableName);
            string[] cols = (string[])qr.Result;
            DataTable dt = new DataTable();
            SQLiteCommand cmd = new SQLiteCommand("select * from `" + tableName + "`;", connection);
            cmd.CommandTimeout = 60000;
            reader = cmd.ExecuteReader();
            int nr = 50;
            nr = backupSettings.MaxInsertValueGroup;
            List<object[]> rez = new List<object[]>();
            i = 0;
            object[] temp = new object[reader.FieldCount];

            while (reader.Read() && !mustStop)
            {
                temp = new object[reader.FieldCount];
                reader.GetValues(temp);
                WriteInsertBlockToFile(fs, tableName, cols, columnsType, temp);
            }
            reader.Close();
            reader.Dispose();
            fs.WriteLine();
            fs.WriteLine();
        }

        private void WriteExportStringTable(TextWriter stream,  string tableName, bool includeDrop, ExportMode mod)
        {
            switch (mod)
            {
                case ExportMode.StructureAndData:
                    stream.WriteLine();
                    stream.Write("/*Table structure for table `" + tableName + "` */" + Environment.NewLine);
                    stream.WriteLine(browser.GetDDLInformation(selectedDataBase, tableName));
                    WriteInsertSyntaxTable(stream,selectedDataBase, tableName);
                    break;
                case ExportMode.StructureOnly:
                    stream.Write(browser.GetDDLInformation(selectedDataBase, tableName) + Environment.NewLine);
                    break;
                case ExportMode.DataOnly:
                    WriteInsertSyntaxTable(stream, selectedDataBase, tableName);
                    break;
            }
            if (includeDrop)
                stream.Write("drop table if exists `" + tableName + "`;" + Environment.NewLine);
        }

        private void WriteCreateView(TextWriter fs, string dataBase)
        {
            cmd = new SQLiteCommand("show table status from `" + dataBase + "` where engine is NULL;", connection);
            ad = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new SQLiteCommand("show create table `" + dataBase + "`.`" + dt.Rows[i]["name"].ToString() + "`;", connection);
                    ad = new SQLiteDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropView)
                            fs.Write("drop view if exists " + dt.Rows[i]["name"].ToString() + ";" + Environment.NewLine);
                        object x = tc.Rows[0]["create view"];
                        if (x.GetType() == typeof(string))
                            fs.Write((string)x + ";" + Environment.NewLine);
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                fs.Write(Encoding.Default.GetString((byte[])x, 0, ((byte[])x).Length) + ";" + Environment.NewLine);
                            }
                    }
                }
            }
        }

        private void WriteCreateProcedure(TextWriter fs, string dataBase)
        {
            cmd = new SQLiteCommand("show procedure status where db = '" + dataBase + "';", connection);
            ad = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new SQLiteCommand("SHOW CREATE PROCEDURE `" + dataBase + "`.`" + dt.Rows[i]["name"].ToString() + "`", connection);
                    ad = new SQLiteDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropProcedure)
                            fs.Write("drop procedure if exists " + dt.Rows[i]["name"].ToString() + ";" + Environment.NewLine);
                        fs.Write("DELIMITER " + delimiter + Environment.NewLine);
                        object x = tc.Rows[i]["create procedure"];
                        if (x.GetType() == typeof(string))
                            fs.Write((string)x + delimiter + Environment.NewLine);
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                fs.Write(Encoding.Default.GetString((byte[])x, 0, ((byte[])x).Length) + delimiter + Environment.NewLine);
                            }
                        fs.Write("DELIMITER ;" + Environment.NewLine);

                    }
                }
            }
        }

        private void WriteCreateFunction(TextWriter fs, string dataBase)
        {
            cmd = new SQLiteCommand("show function status where db = '" + dataBase + "';", connection);
            ad = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new SQLiteCommand("show create function " + dt.Rows[i]["name"].ToString() + ";", connection);
                    ad = new SQLiteDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropFunction)
                            fs.Write("drop function if exists " + dt.Rows[i]["name"].ToString() + ";" + Environment.NewLine);
                        fs.Write("DELIMITER " + delimiter + Environment.NewLine);
                        object x = tc.Rows[0]["create function"];
                        if (x.GetType() == typeof(string))
                            fs.Write((string)x + delimiter + Environment.NewLine);
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                fs.Write(Encoding.Default.GetString((byte[])x, 0, ((byte[])x).Length) + delimiter + Environment.NewLine);
                            }
                        fs.Write("DELIMITER ;" + Environment.NewLine);
                    }
                }
            }

        }

        private void WriteCreateTrigger(TextWriter fs, string dataBase)
        {
            cmd = new SQLiteCommand("use `" + dataBase + "`;show triggers in `" + dataBase + "`;", connection);
            ad = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (backupSettings.AddCreateTrigger)
                        fs.Write("drop trigger `" + dataBase + "`.`" + dt.Rows[i]["trigger"].ToString() + "`;" + Environment.NewLine);
                    fs.Write("DELIMITER " + delimiter + Environment.NewLine);
                    fs.Write("CREATE TRIGGER `" + dataBase + "`.`" + dt.Rows[i]["trigger"].ToString() + "` ");
                    fs.Write(dt.Rows[i]["timing"].ToString() + " " + dt.Rows[i]["event"].ToString() + " ON ");
                    fs.Write("`" + dataBase + "`.`" + dt.Rows[i]["table"].ToString() + "`" + Environment.NewLine);
                    fs.Write("FOR EACH ROW" + Environment.NewLine);
                    fs.Write(dt.Rows[0]["statement"].ToString());
                    fs.Write(delimiter + Environment.NewLine);
                    fs.Write("DELIMITER ;" + Environment.NewLine);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mustStop = false;
            progressBar1.Visible = true;

            if (Path.GetFullPath(fileText.Text).Length == 0)
            {
                MessageBox.Show("Not a valid path", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button1.Enabled = false;
            button3.Enabled = true;
            string[] tables = new string[tableList.CheckedItems.Count];
            string fileName = fileText.Text;
            int i = 0;
            foreach (ListViewItem li in tableList.CheckedItems)
            {
                tables[i] = li.Text;
                i++;
            }
            progressBar1.Maximum = 100;
            backgroundWorker1.RunWorkerAsync(new object[] { fileName,  tables });

        }


        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            tableList.ItemChecked -= listView1_ItemChecked;
            foreach (ListViewItem li in tableList.Items)
            {
                li.Checked = checkBox15.Checked;
            }
            tableList.ItemChecked += listView1_ItemChecked;
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            checkBox15.CheckedChanged -= checkBox15_CheckedChanged;
            if (!e.Item.Checked)
                checkBox15.Checked = false;
            else
            {
                bool check = true;
                foreach (ListViewItem li in tableList.Items)
                {
                    if (!li.Checked)
                        check = false;
                }
                if (check)
                    checkBox15.Checked = true;
            }
            checkBox15.CheckedChanged += checkBox15_CheckedChanged;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime start = DateTime.Now;
            backgroundWorker1.ReportProgress(0, "Backup started at :" + DateTime.Now.ToString() + Environment.NewLine);
            delimiter = backupSettings.Delimiter;
            ExportMode mod = new ExportMode();
            if (backupSettings.ExportTye == PropertyExporter.btype.StructureAndData)
                mod = ExportMode.StructureAndData;
            else
                if (backupSettings.ExportTye == PropertyExporter.btype.DataOnly)
                    mod = ExportMode.DataOnly;
                else
                    if (backupSettings.ExportTye == PropertyExporter.btype.StructureOnly)
                        mod = ExportMode.StructureOnly;

            object[] arg = (object[])e.Argument;
            string[] tables = (string[])arg[1];
            using (TextWriter fs = new StreamWriter(fileText.Text, false, Encoding.Default))
            {
                try
                {
                    fs.Flush();

                        fs.Write("PRAGMA encoding = \"UTF-8\";" + Environment.NewLine);

                    //if (backupSettings.AddUseDataBase)
                    //    fs.Write("use `" + db + "`;" + Environment.NewLine);


                    //if (backupSettings.SetForeignKeyZero)
                    //{
                    //    fs.WriteLine("/*!40101 SET SQL_MODE='' */;");
                    //    fs.WriteLine("/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;");
                    //    fs.WriteLine("/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;");
                    //}

                    //fs.Write("/*!40101 SET NAMES utf8 */;" + Environment.NewLine);
                    int x = 2;
                    for (int k = 0; k < tables.Length && !mustStop; k++)
                    {
                        backgroundWorker1.ReportProgress(x, "Deploying table '" + tables[k] + "'...");
                        WriteExportStringTable(fs, tables[k], backupSettings.AddDropTable, mod);
                        x = (int)(((float)k / (tables.Length + 1)) * 50);
                        backgroundWorker1.ReportProgress(x, " Done." + Environment.NewLine);
                    }


                    //if (backupSettings.SetForeignKeyZero)
                    //{
                    //    fs.Write("/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;" + Environment.NewLine);
                    //    fs.Write("/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;" + Environment.NewLine);
                    //}

                    //if (backupSettings.FlushLogsBeforeDump)
                    //{
                    //    backgroundWorker1.ReportProgress(99, "Flushing logs...");
                    //    cmd = new SQLiteCommand("flush logs;", connection);
                    //    cmd.ExecuteNonQuery();
                    //    backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    //}



                    //if (backupSettings.AddCreateView)
                    //{
                    //    backgroundWorker1.ReportProgress(99, "Deploying views...");
                    //    fs.Write(Environment.NewLine + Environment.NewLine);
                    //    WriteCreateView(fs, db);
                    //    backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    //}

                    //if (backupSettings.AddCreateProcedure)
                    //{
                    //    backgroundWorker1.ReportProgress(99, "Deploying procedures...");
                    //    fs.Write(Environment.NewLine + Environment.NewLine);
                    //    WriteCreateProcedure(fs, db);
                    //    backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    //}

                    //if (backupSettings.AddCreateFunction)
                    //{
                    //    backgroundWorker1.ReportProgress(99, "Deploying functions...");
                    //    fs.Write(Environment.NewLine + Environment.NewLine);
                    //    WriteCreateFunction(fs, db);
                    //    backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    //}

                    //if (backupSettings.AddCreateTrigger)
                    //{
                    //    backgroundWorker1.ReportProgress(99, "Deploying triggers...");
                    //    fs.Write(Environment.NewLine + Environment.NewLine);
                    //    WriteCreateTrigger(fs, db);
                    //    backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    fs.Close();
                    DateTime stop = DateTime.Now;
                    backgroundWorker1.ReportProgress(100, "Backup finished at " + DateTime.Now.ToString() + "." + Environment.NewLine);
                    TimeSpan t = stop.Subtract(start);
                    backgroundWorker1.ReportProgress(100, "Backup time:" + Math.Round(t.TotalSeconds, 2).ToString() + " seconds" + Environment.NewLine);

                }
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (e.UserState.GetType() == typeof(string))
            {
                string rez = (string)e.UserState;
                outputText.AppendText(rez);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            progressBar1.Visible = false;
            progressBar1.Value = 100;
            button1.Enabled = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            mustStop = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            advanced = !advanced;
            SetWidthAdv(advanced);
           
        }

    }

    [DefaultPropertyAttribute("BackupType")]
    public class PropertyExporter
    {
        public enum btype { [Description("Stucture only")] StructureOnly,[Description("Structure & data")] StructureAndData,[Description("Data only")] DataOnly };

        private btype exportTye;

        private bool addCreateDatabase;
        private bool addIncludeDataBase;
        private bool flushLogsBeforeDump;
        private bool addDropTable;
        private bool lockTablesForRead;
        private bool setForeignKeyZero;


        private bool addDropView;
        private bool addDropProcedure;
        private bool addDropFunction;
        private bool addDropTrigger;

        private bool addCreateView;
        private bool addCreateProcedure;
        private bool addCreateFunction;
        private bool addCreateTrigger;

        private string delimiter;
        private int maxInsertValueGroup;



        public PropertyExporter()
        {
            exportTye = btype.StructureAndData;
            addCreateDatabase = true;
            addIncludeDataBase = true;
            flushLogsBeforeDump = true;
            lockTablesForRead = true;
            setForeignKeyZero = true;


            addDropView = true ;
            addDropProcedure = true;
            addDropFunction = true;
            addDropTrigger = true;

            addCreateView = true;
            addCreateProcedure = true;
            addCreateFunction = true;
            addCreateTrigger = true;

            delimiter = ";;";
            maxInsertValueGroup = 100;

        }




        [CategoryAttribute("Backup Type"), DescriptionAttribute("Backup type"),  DisplayName("Backup type"), TypeConverter(typeof(BackupTypeConverter))]
        public btype ExportTye
        {
            get { return exportTye; }
            set { exportTye = value; }
        }
        [CategoryAttribute("Main Settings"), DescriptionAttribute(""), DisplayName("Add \"create database\"")]
        public bool AddCreateDatabase
        {
            get { return addCreateDatabase; }
            set { addCreateDatabase = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""),DisplayName("Add \"include db-name\"")]
        public bool AddIncludeDataBase
        {
            get { return addIncludeDataBase; }
            set { addIncludeDataBase = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""),DisplayName("Flush logs before dump")]
        public bool FlushLogsBeforeDump
        {
            get { return flushLogsBeforeDump; }
            set { flushLogsBeforeDump = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""), DisplayName("Include \"drop table\"")]
        public bool AddDropTable
        {
            get { return addDropTable; }
            set { addDropTable = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""),DisplayName("Lock tables for read")]
        public bool LockTablesForRead
        {
            get { return lockTablesForRead; }
            set { lockTablesForRead = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""), DisplayName("Set foreign keys to zero")]
        public bool SetForeignKeyZero
        {
            get { return setForeignKeyZero; }
            set { setForeignKeyZero = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""), DisplayName("Delimiter")]
        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""), DisplayName("MaximumInsertValuesGroup")]
        public int MaxInsertValueGroup
        {
            get { return maxInsertValueGroup; }
            set { maxInsertValueGroup = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"drop view\"")]
        public bool AddDropView
        {
            get { return addDropView; }
            set { addDropView = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"create view\"")]
        public bool AddCreateView
        {
            get { return addCreateView; }
            set { addCreateView = value; }
        }


        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"drop procedure\"")]
        public bool AddDropProcedure
        {
            get { return addDropProcedure; }
            set { addDropProcedure = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"create procedure\"")]
        public bool AddCreateProcedure
        {
            get { return addCreateProcedure; }
            set { addCreateProcedure = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"drop function\"")]
        public bool AddDropFunction
        {
            get { return addDropFunction; }
            set { addDropFunction = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"create function\"")]
        public bool AddCreateFunction
        {
            get { return addCreateFunction; }
            set { addCreateFunction = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"drop trigger\"")]
        public bool AddDropTrigger
        {
            get { return addDropTrigger; }
            set { addDropTrigger = value; }
        }

        [CategoryAttribute("Include"), DescriptionAttribute(""), DisplayName("Include \"create trigger\"")]
        public bool AddCreateTrigger
        {
            get { return addCreateTrigger; }
            set { addCreateTrigger = value; }
        }

    }




    class BackupTypeConverter : EnumConverter
    {
        private Type btype;

        public BackupTypeConverter(Type type)
            : base(type)
        {
            btype = type;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            FieldInfo fi = btype.GetField(Enum.GetName(btype, value));
            DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            foreach (FieldInfo fi in btype.GetFields())
            {
                DescriptionAttribute dna =
                (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(btype, fi.Name);
            }
            return Enum.Parse(btype, (string)value);
        }
    }

}