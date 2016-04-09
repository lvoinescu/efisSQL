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
using DBMS.MySQL;
using System.IO;
using mysqlLib = MySql ;
using MySql.Data.Types;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DBMS.core;

namespace DBMS.MySQL
{
    public partial class BackUpDataForm : Form
    {
        private bool advanced = false;
        private string[] dataBases;
        private string[] tables;
        private string[] columnsType;
        private string selectedTable;
        private string selectedDataBase;
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataAdapter ad;
        private string delimiter = ";;";
        private bool mustStop;
        MySQLDBBrowser browser;
        PropertyExporter backupSettings;
        Encoding encoding;
        private enum ExportMode { StructureAndData, StructureOnly, DataOnly };

        public MySQLDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        public BackUpDataForm()
        {
            InitializeComponent();
        }

        public BackUpDataForm(MySqlConnection connection, string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
        {
            backupSettings = new PropertyExporter();
            this.dataBases = dataBases;
            this.connection = connection;
            this.tables = tables;
            this.selectedTable = selectedTable;
            this.selectedDataBase = selectedDataBase;
            InitializeComponent();
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
            dataBaseComboBox.Text = selectedDataBase;
            dataBaseComboBox.Items.AddRange(dataBases);
            if (selectedTable == null)
            {
                dataBaseComboBox.SelectedItem = selectedDataBase;
                for (int i = 0; i < tables.Length; i++)
                    tableList.Items.Add(tables[i]);
            }
            else
            {
                tableList.Items.Add(selectedTable);
                string[] x = new string[] { };
                
            }

            tableList.Columns[0].Width = tableList.Width - 10;
            propertyGrid1.SelectedObject = backupSettings;
            checkBox15.Checked = true;
            dataBaseComboBox.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sFile = new SaveFileDialog();
            sFile.Filter = "Text files|*.txt|Sql files|*.sql";
            sFile.ShowDialog();
            fileText.Text = sFile.FileName;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryResult qr= this.browser.ListTables(dataBaseComboBox.SelectedItem.ToString());
            if (qr.ErrorNo == 0)
            {
                tableList.Items.Clear();
                string [] tables=  (string[])qr.Result;
                if (tables == null)
                    return;
                for (int i = 0; i < tables.Length; i++)
                    tableList.Items.Add(new ListViewItem(tables[i],tables[i]));
            }
        }

        private void WriteDataWithFormat(BinaryWriter fs, object data, string columnType)
        {
            byte[] apostroph = encoding.GetBytes("\"");
            try
            {
                string ret = "";
                switch (columnType)
                {
                    case "bigint":
                    case "tinyint":
                    case "decimal":
                    case "float":
                    case "long":
                    case "int":
                    case "mediumint":
                    case "double":
                    case "smallint":
                    case "year":
                    case "bit":
                        FSWrite(fs, data.ToString());
                        break;
                    case "binary":
                        FSWrite(fs, "'");
                        WriteEscapeData(fs, encoding.GetString((byte[])data));
                        FSWrite(fs, "'");
                        break;
                    case "char":
                    case "enum":
                    case "set":
                    case "text":
                    case "mediumtext":
                    case "longtext":
                    case "varchar":
                    case "tinytext":
                        FSWrite(fs, "'");
                        WriteEscapeData(fs, (string)data);
                        FSWrite(fs, "'");
                        break;
                    case "time":
                        FSWrite(fs, "'");
                        FSWrite(fs, data.ToString());
                        FSWrite(fs, "'");
                        break;
                    case "datetime":
                    case "timestamp":
                        MySqlDateTime v = (MySqlDateTime)data;
                        string dt = string.Format("{0}-{1}-{2} {3}:{4}:{5}", v.Year, v.Month, v.Day, v.Hour, v.Minute, v.Second);
                        FSWrite(fs, "'" + dt + "'");
                        break;
                    case "date":
                        FSWrite(fs, "'");
                        Type t = data.GetType();
                        string ts = ((mysqlLib.Data.Types.MySqlDateTime)data).Value.ToString("yyyy-MM-dd");
                        FSWrite(fs, ts);
                        FSWrite(fs, "'");
                        break;
                    case "blob":
                    case "longblob":
                    case "smallblob":
                    case "mediumblob":
                        FSWrite(fs,"'");
                        //string x = encoding.GetString((byte[])data);
                        //byte []y =encoding.GetBytes(x);
                        WriteEscapeData(fs, (byte[])data);
                        FSWrite(fs, "'");
                        break;
                    default:
                        if (data.GetType() == typeof(string))
                            ret = (string)data;
                        else
                            if (data.GetType() == typeof(byte[]))
                                ret = encoding.GetString((byte[])data);
                            else
                                ret = data.ToString();

                        FSWrite(fs, "'");
                        WriteEscapeData(fs, ret);
                        FSWrite(fs, "'");
                        break;
                }
            }
            catch (DecoderFallbackException dex)
            {
            }
            catch (Exception ex)
            {
            }
        }


        private void WriteEscapeData(BinaryWriter fs, byte[] data)
        {
            int l = data.Length;
            int k=0;
            int bufferSize = 128;
            Encoding encoding = Encoding.Default;
            if (data.Length < bufferSize)
            {
                //string sdata = Encoding.Default.GetString(data, k, data.Length);
                WriteEscapeByteArray(fs, data, k, data.Length);
                return;
            }
            int n = (int)l / bufferSize;
            while (k < n)
            {
                //string s = Encoding.Default.GetString(data, k * bufferSize, bufferSize);
                WriteEscapeByteArray(fs, data, k * bufferSize, bufferSize);
                k++;
            }
            if (k >= n)
            {
                //string s = Encoding.Default.GetString(data, k * bufferSize, l - k * bufferSize);
                WriteEscapeByteArray(fs, data, k * bufferSize, l - k * bufferSize);

            }
        }

        private void WriteEscapeByteArray(BinaryWriter fs, byte[] data, int start, int len)
        {
            string sdata = Encoding.Default.GetString(data, start, len);
            char[] buf = sdata.ToCharArray();
            // FSWrite(fs,buf);
            DateTime start1 = DateTime.Now;
            Regex r = new Regex("['\"\n\r\0\u001A\\\\]");

            MatchCollection mc = r.Matches(sdata);
            if (mc.Count > 0)
            {
                int k = 0;
                for (int i = 0; i < mc.Count; i++)
                {
                    //string aux = mc[i].ToString();
                    fs.Write(data,start + k, mc[i].Index - k);
                    k =  mc[i].Index + 1;
                    switch (buf[mc[i].Index])
                    {
                        case '\0':
                            FSWrite(fs,"\\0");
                            break;
                        case '\'':
                           FSWrite(fs,"\\'");
                            break;
                        case '\"':
                           FSWrite(fs,"\\\"");
                            break;
                        case '\n':
                          FSWrite(fs,"\\n");
                            break;
                        case '\r':
                           FSWrite(fs,"\\r");
                            break;
                        case (char)26:
                            FSWrite(fs,@"\Z");
                            break;
                        case '\\':
                           FSWrite(fs,"\\\\");
                            break;


                        //case '\b':
                        //     FSWrite(fs,"\\b");
                        //    break;
                        //case '\t':
                        //     FSWrite(fs,"\\t");
                        //    break;
                        //case '%':
                        //     FSWrite(fs,"\\%");
                        //    break;
                        //case '_':
                        //     FSWrite(fs,"\\_");
                        //    break;
                    }

                }
                int index = mc[mc.Count - 1].Index;
                if (index < len - 1)
                {
                	int starta =start + index+1;
                	int lena =  len -index-1;
                	byte [] aux = new byte[lena];
                		
                	Array.Copy(data, starta,aux,0,lena);
                     fs.Write(aux, 0, lena);
                }
                DateTime stop1 = DateTime.Now;
                TimeSpan t = stop1.Subtract(start1);
            }
            else
                 fs.Write(data,start, len);
        }

        private void WriteEscapeData(BinaryWriter fs, string data)
        {

            byte[] dt = encoding.GetBytes(data);

            char[] buf = data.ToCharArray();
            // FSWrite(fs,buf);
            DateTime start1 = DateTime.Now;
            Regex r = new Regex("['\"\n\r\0\u001A\\\\]");

            MatchCollection mc = r.Matches(data);
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
                        case '\0':
                             FSWrite(fs,"\\0");
                            break;
                        case '\'':
                             FSWrite(fs,"\\'");
                            break;
                        case '\"':
                             FSWrite(fs,"\\\"");
                            break;
                        case '\n':
                             FSWrite(fs,"\\n");
                            break;
                        case '\r':
                             FSWrite(fs,"\\r");
                            break;
                        case (char)26:
                             FSWrite(fs,@"\Z");
                            break;
                        case '\\':
                             FSWrite(fs,"\\\\");
                            break;


                        //case '\b':
                        //     FSWrite(fs,"\\b");
                        //    break;
                        //case '\t':
                        //     FSWrite(fs,"\\t");
                        //    break;
                        //case '%':
                        //     FSWrite(fs,"\\%");
                        //    break;
                        //case '_':
                        //     FSWrite(fs,"\\_");
                        //    break;
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
                 fs.Write(dt);
        }

        private void WriteInsertBlockToFile(BinaryWriter fs, string table, string[] cols, string [] columnsType, List<object[]> rez)
        {
            FSWrite(fs,"insert into `");
            FSWrite(fs,table);
            FSWrite(fs,"` (");
            FSWrite(fs,"`" + cols[0] + "`");
            for (int u = 1; u < cols.Length; u++)
            {
            	FSWrite(fs,",`");
            	FSWrite(fs,cols[u]);
                FSWrite(fs,"`");
            }
            FSWrite(fs,") values ");

            FSWrite(fs, "(");
            if (rez[0][0] != DBNull.Value)
            {
                WriteDataWithFormat(fs, rez[0][0], columnsType[0]);
            }
            else
            {
                FSWrite(fs, "null");
            }
            for (int j = 1; j < cols.Length; j++)
            {
                FSWrite(fs, ",");
                if (rez[0][j] != DBNull.Value)
                {
                    WriteDataWithFormat(fs, rez[0][j], columnsType[j]);
                }
                else
                {
                    FSWrite(fs, "null");
                }
            }
            FSWrite(fs, ")");

            for (int n = 1; n < rez.Count; n++)
            {
                FSWrite(fs, ",(");
                if (rez[n][0] != DBNull.Value)
                {
                    WriteDataWithFormat(fs, rez[n][0], columnsType[0]);
                }
                else
                {
                    FSWrite(fs, "''");
                }

                for (int j = 1; j < cols.Length; j++)
                {
                     FSWrite(fs,",");
                    if (rez[n][j] != DBNull.Value)
                    {
                        WriteDataWithFormat(fs, rez[n][j], columnsType[j]);
                    }
                    else
                    {
                        FSWrite(fs, "null");
                    }
                }
                FSWrite(fs, ")");
            }
            FSWrite(fs, ";");

        }

        private void WriteInsertSyntaxTable(BinaryWriter fs, string dataBase, string tableName)
        {

                 FSWrite(fs,"/*Data for table `" + dataBase +"`.`" + tableName +"` */" );
                MySqlDataReader reader = null;
                QueryResult qR = browser.GetColumnInformation(dataBase, tableName);
                DataTable cTable = (DataTable)qR.Result;
                columnsType = new string[cTable.Rows.Count];
                Regex r = new Regex("\\w+");
                int i = 0;
                for (i = 0; i < cTable.Rows.Count; i++)
                    columnsType[i] =  r.Matches(cTable.Rows[i][1].ToString())[0].Value.ToLower();
                QueryResult qr = browser.ListColumns(dataBase, tableName);
                string[] cols = (string[])qr.Result;
                DataTable dt = new DataTable();
                MySqlCommand cmd = new MySqlCommand("select * from `" + dataBase + "`.`" + tableName + "`;", connection);
                cmd.CommandTimeout = 60000;
                reader = cmd.ExecuteReader();
                int nr = 50;
                nr = backupSettings.MaxInsertValueGroup;
                List<object[]> rez = new List<object[]>();
                i = 0;
                int k = 0;
                object[] temp = new object[reader.FieldCount];

                while (reader.Read() && !mustStop)
                {
                    if (mustStop)
                    {
                    }
                    temp = new object[reader.FieldCount];
                    reader.GetValues(temp);
                    rez.Add(temp);
                    k++;
                    i++;
                    if (i % nr == 0)
                    {
                        WriteInsertBlockToFile(fs, tableName, cols, columnsType, rez);
                        rez = new List<object[]>();
                        k = 0;
                    }
                }
                if (k < nr && k > 0 && !mustStop)
                {
                    //last rows
                    WriteInsertBlockToFile(fs, tableName, cols, columnsType, rez);
                }
                reader.Close();
                reader.Dispose();
                 FSWrite(fs,Environment.NewLine);;
                 FSWrite(fs,Environment.NewLine);;
        }

        private void WriteExportStringTable(BinaryWriter stream, string dataBase, string tableName, bool includeDrop, ExportMode mod)
        {
             switch(mod)
            {
                case ExportMode.StructureAndData:
                    FSWrite(stream,Environment.NewLine);
                    FSWrite(stream,"/*Table structure for table `" + dataBase + "`.`" + tableName + "` */" + Environment.NewLine);
                    FSWrite(stream, browser.GetDDLInformation(dataBase, tableName) + Environment.NewLine);
                    WriteInsertSyntaxTable(stream,dataBase, tableName);
                    break;
                case ExportMode.StructureOnly:
                    stream.Write(browser.GetDDLInformation(dataBase, tableName));
                    break;
                case ExportMode.DataOnly:
                    WriteInsertSyntaxTable(stream,dataBase, tableName);
                    break;
            }
            if(includeDrop)
                stream.Write("drop table if exists `" + tableName + "`;");
        }

        private void WriteCreateView(BinaryWriter fs, string dataBase)
        {
            cmd = new MySqlCommand("show table status from `"+dataBase+"` where engine is NULL;", connection);
            ad = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new MySqlCommand("show create table `" + dataBase + "`.`" + dt.Rows[i]["name"].ToString() + "`;",connection);
                    ad = new MySqlDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropView)
                             FSWrite(fs,"drop view if exists " + dt.Rows[i]["name"].ToString() + ";");
                        object x =tc.Rows[0]["create view"];
                        if(x.GetType()==typeof(string))
                             FSWrite(fs,(string)x+ ";");
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                 FSWrite(fs,encoding.GetString((byte[])x,0,((byte[])x).Length) + ";");
                            }
                    }
                }
            }
        }

        private void WriteCreateProcedure(BinaryWriter fs, string dataBase)
        {
            cmd = new MySqlCommand("show procedure status where db = '" + dataBase + "';", connection);
            ad = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new MySqlCommand("SHOW CREATE PROCEDURE `" + dataBase + "`.`" + dt.Rows[i]["name"].ToString() + "`", connection);
                    ad = new MySqlDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropProcedure)
                             FSWrite(fs,"drop procedure if exists " + dt.Rows[i]["name"].ToString() + ";");
                         FSWrite(fs,"DELIMITER " + delimiter);
                        object x = tc.Rows[i]["create procedure"];
                        if (x.GetType() == typeof(string))
                             FSWrite(fs,(string)x + delimiter );
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                 FSWrite(fs,encoding.GetString((byte[])x, 0, ((byte[])x).Length) + delimiter );
                            }
                         FSWrite(fs,"DELIMITER ;");

                    }
                }
            }
        }

        private void WriteCreateFunction(BinaryWriter fs, string dataBase)
        {
            cmd = new MySqlCommand("show function status where db = '" + dataBase + "';", connection);
            ad = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmd = new MySqlCommand("show create function " + dt.Rows[i]["name"].ToString() + ";", connection);
                    ad = new MySqlDataAdapter(cmd);
                    DataTable tc = new DataTable();
                    ad.Fill(tc);
                    if (tc.Rows.Count > 0)
                    {
                        if (backupSettings.AddDropFunction)
                             FSWrite(fs,"drop function if exists " + dt.Rows[i]["name"].ToString() + ";");
                         FSWrite(fs,"DELIMITER " + delimiter);
                        object x = tc.Rows[0]["create function"];
                        if (x.GetType() == typeof(string))
                             FSWrite(fs,(string)x + delimiter);
                        else
                            if (x.GetType() == typeof(byte[]))
                            {
                                 FSWrite(fs,encoding.GetString((byte[])x, 0, ((byte[])x).Length) + delimiter);
                            }
                         FSWrite(fs,"DELIMITER ;");
                    }
                }
            }

        }

        private void WriteCreateTrigger(BinaryWriter fs, string dataBase)
        {
            cmd = new MySqlCommand("use `"+dataBase+"`;show triggers in `"+dataBase+"`;", connection);
            ad = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                        if (backupSettings.AddCreateTrigger)
                             FSWrite(fs,"drop trigger `" +dataBase+"`.`" + dt.Rows[i]["trigger"].ToString() + "`;");
                         FSWrite(fs,"DELIMITER " + delimiter);
                         FSWrite(fs,"CREATE TRIGGER `" +dataBase +"`.`" + dt.Rows[i]["trigger"].ToString() +"` ");
                         FSWrite(fs,dt.Rows[i]["timing"].ToString() + " " + dt.Rows[i]["event"].ToString() + " ON ");
                         FSWrite(fs,"`"+dataBase+"`.`" + dt.Rows[i]["table"].ToString() + "`");
                         FSWrite(fs,"FOR EACH ROW");
                         FSWrite(fs,dt.Rows[0]["statement"].ToString() );
                         FSWrite(fs,delimiter);
                         FSWrite(fs,"DELIMITER ;");
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mustStop = false;
            progressBar1.Visible = true;
            cmd = new MySqlCommand("set net_read_timeout = 20000", connection);
            cmd.ExecuteNonQuery();

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
            progressBar1.Maximum =100;
            backgroundWorker1.RunWorkerAsync(new object[] {fileName, dataBaseComboBox.Text, tables});

        }



        private void FSWrite(BinaryWriter fs,string str)
        {
           fs.Write(encoding.GetBytes(str));
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


            //encoding = new UTF8Encoding(false,false) ;
            encoding = new UTF8Encoding(false);
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
            string[] tables = (string[])arg[2];
            string db = (string)arg[1];

            cmd.CommandText = @" /*!40101 SET SQL_MODE=''*/;
                                 /*!40101 SET NAMES utf8 */;";
            cmd.ExecuteNonQuery();
            FileStream stream = new FileStream(fileText.Text,FileMode.Create);

            using (BinaryWriter fs = new BinaryWriter(stream))
            {
                try
                {

                    fs.Flush();
                    if (backupSettings.AddCreateDatabase)
                        FSWrite(fs, "create database if not exists `" + db + "`;");

                    if (backupSettings.AddUseDataBase)
                        FSWrite(fs, "use `" + db + "`;" + Environment.NewLine);


                    //if (backupSettings.SetForeignKeyZero)
                    //{
                    //     FSWrite(fs,"/*!40101 SET SQL_MODE='' */;");
                    //     FSWrite(fs,"/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;");
                    //     FSWrite(fs,"/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO'*/;");
                    //}
//                    cmd.CommandText = @"/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
///*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
///*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
///*!40101 SET NAMES utf8 */;
///*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
///*!40103 SET TIME_ZONE='+00:00' */;
///*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
///*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
///*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
///*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;";
//                        cmd.ExecuteNonQuery();;



                    FSWrite(fs,@"/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;


");
                    int x = 2;
                    for (int k = 0; k < tables.Length && ! mustStop; k++)
                    {
                        backgroundWorker1.ReportProgress(x, "Deploying table '" + tables[k] + "'...");
                        WriteExportStringTable(fs, db, tables[k], backupSettings.AddDropTable, mod);
                        x = (int)(((float)k / (tables.Length+1)) * 50);
                        backgroundWorker1.ReportProgress(x, " Done." + Environment.NewLine);
                    }


                    //if (backupSettings.SetForeignKeyZero)
                    //{
                    //     FSWrite(fs,"/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;");
                    //     FSWrite(fs,"/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;");
                    //}

                    if (backupSettings.FlushLogsBeforeDump)
                    {
                        backgroundWorker1.ReportProgress(99, "Flushing logs...");
                        cmd = new MySqlCommand("flush logs;", connection);
                        cmd.ExecuteNonQuery();
                        backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    }



                    if (backupSettings.AddCreateView)
                    {
                        backgroundWorker1.ReportProgress(99, "Deploying views...");
                        FSWrite(fs, Environment.NewLine);
                        FSWrite(fs, Environment.NewLine);
                        WriteCreateView(fs, db);
                        backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    }

                    if (backupSettings.AddCreateProcedure)
                    {
                        backgroundWorker1.ReportProgress(99, "Deploying procedures...");
                        FSWrite(fs, Environment.NewLine);
                        FSWrite(fs, Environment.NewLine);
                        WriteCreateProcedure(fs, db);
                        backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    }

                    if (backupSettings.AddCreateFunction)
                    {
                        backgroundWorker1.ReportProgress(99, "Deploying functions...");
                        FSWrite(fs, Environment.NewLine);
                        FSWrite(fs, Environment.NewLine);
                        WriteCreateFunction(fs, db);
                        backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    }

                    if (backupSettings.AddCreateTrigger)
                    {
                        backgroundWorker1.ReportProgress(99, "Deploying triggers...");
                         FSWrite(fs,Environment.NewLine);
                         FSWrite(fs,Environment.NewLine);
                        WriteCreateTrigger(fs, db);
                        backgroundWorker1.ReportProgress(99, " Done." + Environment.NewLine);
                    }
//                    FSWrite(fs,@"/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
///*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
///*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
///*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
///*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
///*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
///*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
///*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;");
//                     cmd.CommandText = @"/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
///*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
///*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
///*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
///*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
///*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
///*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
///*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;";
 //                   cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    fs.Close();
                    DateTime stop = DateTime.Now;
                    backgroundWorker1.ReportProgress(100, "Backup finished at " + DateTime.Now.ToString() +"." + Environment.NewLine);
                    TimeSpan t = stop.Subtract(start);
                    backgroundWorker1.ReportProgress(100, "Backup time:" + Math.Round(t.TotalSeconds,2).ToString() +" seconds" + Environment.NewLine);

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
            SaveSettings();
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
 		
        
        private void SaveSettings()
        {
//           // StreamWriter writer = new StreamWriter(Application.UserAppDataPath + "\\config.xml", false, Encoding.UTF8);
//            //writer.Flush();
//            try
//            {
//	            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PropertyExporter));
//	            Stream s = new MemoryStream();
//	            xmlSerializer.Serialize(s, backupSettings);
//	            byte []x = new byte[100000];
//	            s.Seek(0, SeekOrigin.Begin);
//	            s.Read(x, 0,(int) s.Length);
//	            string aa = Encoding.Default.GetString(x);
//	            //MessageBox.Show(aa);
//            }
//            catch(Exception ex)
//            {
//            	
//            }
            //xmlSerializer.Serialize(writer, backupSettings);
            //writer.Write(aa);
            //writer.Close();

        }

        
        
    }


    [DefaultPropertyAttribute("BackupType")]
    public class PropertyExporter
    {
        public enum btype { [Description("Stucture only")] StructureOnly,[Description("Structure & data")] StructureAndData,[Description("Data only")] DataOnly };

        private btype exportTye;

        private bool addCreateDatabase;
        private bool addUseDataBase;
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
            addCreateDatabase = false;
            addUseDataBase = false;
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

        [CategoryAttribute("Main Settings"), DescriptionAttribute(""),DisplayName("Add \"use db-name\"")]
        public bool AddUseDataBase
        {
            get { return addUseDataBase; }
            set { addUseDataBase = value; }
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