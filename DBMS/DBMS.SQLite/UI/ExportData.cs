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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Xml;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using DBMS.core;

namespace DBMS.SQLite
{
    public partial class ExportData : Form
    {
        private SQLiteConnection connection;
        private string dataBase;
        private string tableName;
        private string[] columns;
        private string filter;
        private DataGridView dgView;
        private enum ModExport { select, dgview };
        private ModExport mod;
        public ExportData()
        {
            InitializeComponent();
        }

        public ExportData(SQLiteConnection connection, string dataBase, string tableName, string [] columns)
        {
            this.connection = connection;
            this.dataBase = dataBase;
            this.tableName = tableName;
            this.columns = columns;
            InitializeComponent();
            for (int i = 0; i < columns.Length; i++)
            {
                ListViewItem it = new ListViewItem(columns[i]);
                it.Checked = true;
                listView1.Items.Add(it);
                it.Selected = true;
            }
            mod = ModExport.select;
        }

        public ExportData(DataGridView dgView, string[] cols)
        {
            this.dgView = dgView;
            mod = ModExport.dgview;
            InitializeComponent();
            this.columns = cols;
            for (int i = 0; i < cols.Length; i++)
            {
                ListViewItem it = new ListViewItem(cols[i]);
                it.Checked = true;
                listView1.Items.Add(it);
                it.Selected = true;
            }
            
        }

        private string EscapeString(string s)
        {
            Regex r = new Regex("\\\\+");
            MatchCollection matches = r.Matches(s,0);
            foreach (Match m in matches)
            {
                char[] special = new char[] { 'n', 'r', 't', 'b' };
                if(m.Value.Length%2!=0)
                {
                    int next = m.Index+m.Value.Length;
                    if (next < s.Length)
                    {
                        if (Array.IndexOf(special, s[next]) < 0)
                            s = s.Insert(m.Index + m.Value.Length, m.Value);
                    }
                    else
                        s = s.Insert(m.Index + m.Value.Length, m.Value);
                }
            }
            s = s.Replace("\'", "\\'");
            return s;
        }


        private string FormatFieldByType(object data)
        {
            Type type =data.GetType();
            if (type == typeof(byte[]))
                return System.Text.Encoding.Default.GetString((byte[])data);
            else
                if (data == DBNull.Value)
                    return "(null)";
                else
                    if (type == typeof(DateTime))
                    {
                        DateTime t = (DateTime)data;
                        return t.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                        return data.ToString();
        }

        private void ExportToCsv(string fileName, string[] cols, List<SqlDataGridViewRow> rows, int[] indexes)
        {
            string lineEnd = textBox4.Text.Replace("\\n", "\n");
            lineEnd = lineEnd.Replace("\\r", "\r");
            lineEnd = lineEnd.Replace("\\t", "\t");
            string fieldEnd = textBox1.Text.Replace("\\n", "\n");
            fieldEnd = fieldEnd.Replace("\\r", "\r");
            fieldEnd = fieldEnd.Replace("\\t", "\t");
            string fieldEnclosed = textBox2.Text.Replace("\\n", "\n");
            fieldEnclosed = fieldEnclosed.Replace("\\r", "\r");
            fieldEnclosed = fieldEnclosed.Replace("\\t", "\t");
            try
            {
                switch (mod)
                {
                    case ModExport.select:
                        fileName = fileName.Replace('\\', '/');
                        string s = "select ";
                        for (int i = 0; i < indexes.Length; i++)
                        {
                            s += "`" + cols[indexes[i]] + "`,";
                        }
                        s = s.Remove(s.Length - 1, 1);
                        s += " from `" +dataBase +"`.`"+ tableName+ "`";
                        SQLiteCommand cmd = new SQLiteCommand(s, connection);
                        using (StreamWriter writter = new StreamWriter(fileName))
                        {
                            SQLiteDataReader reader = cmd.ExecuteReader();
                            object[] temp = new object[reader.FieldCount];
                            try
                            {
                                while (reader.Read())
                                {
                                    reader.GetValues(temp);
                                    for (int j = 0; j < indexes.Length; j++)
                                    {
                                        string value = FormatFieldByType(temp[j]);
                                        writter.Write(fieldEnclosed);
                                        writter.Write(value);
                                        writter.Write(fieldEnclosed);
                                        if (j < indexes.Length - 1)
                                            writter.Write(fieldEnd);
                                    }
                                    writter.Write(lineEnd);
                                }

                            }
                            catch (Exception exception)
                            {
                            }
                            finally
                            {
                                reader.Close();
                            }
                            writter.Close();
                        }
                        break;
                    case ModExport.dgview:
                        TextWriter  tw = new StreamWriter(fileName);
                        for (int i = 0; i < rows.Count; i++)
                        {
                            SqlDataGridViewRow row = rows[i];
                            for (int j = 0; j < indexes.Length; j++)
                            {
                                object data = row.Data[indexes[j]];
                                string value = FormatFieldByType(data);
                                value = value.Replace("\\", textBox3.Text+"\\");
                                tw.Write(fieldEnclosed);
                                tw.Write(value);
                                tw.Write(fieldEnclosed);
                                tw.Write(fieldEnd);
                            }
                            tw.Write(lineEnd);
                        }
                        tw.Close();
                        break;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportToXml(string fileName, string [] cols, List<SqlDataGridViewRow> rows, int[] indexes)
        {
            XmlDocument doc = new XmlDocument();
            switch (mod)
            {
                case ModExport.select:
                    string s = "select ";
                    for(int i=0;i<indexes.Length;i++)
                    {
                        s +="`" + cols[indexes[i]] + "`,";
                    }
                    s = s.Remove(s.Length - 1, 1) + " from `" +dataBase +"`.`" + tableName+"`";
                    using(SQLiteCommand cmd = new SQLiteCommand(s,connection))
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader(); 
                        object[] temp = new object[reader.FieldCount];
                        XmlNode nod = doc.CreateElement("data");
                        try
                        {
                            while (reader.Read())
                            {
                                temp = new object[reader.FieldCount];
                                reader.GetValues(temp);
                                XmlNode n = doc.CreateElement("row");
                                for (int j = 0; j < temp.Length; j++)
                                {
                                    XmlNode nc = doc.CreateElement(cols[j]);
                                    nc.InnerText = FormatFieldByType(temp[j]);
                                    n.AppendChild(nc);
                                }
                                nod.AppendChild(n);
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                        finally
                        {
                            reader.Close();
                        }
                        doc.AppendChild(nod);
                        doc.Save(fileName);
                    }
                    break;
                case ModExport.dgview:
                    XmlNode nod2 = doc.CreateElement("data");
                    for(int i=0;i<rows.Count;i++)
                    {
                        SqlDataGridViewRow row = rows[i];
                        XmlNode n = doc.CreateElement("row");
                        for (int j = 0; j < indexes.Length; j++)
                        {
                            XmlNode nc = doc.CreateElement(cols[indexes[j]]);
                            object data = row.Data[indexes[j]];
                            nc.InnerText = FormatFieldByType(data);
                            n.AppendChild(nc);
                        }
                        nod2.AppendChild(n);
                    }
                    doc.AppendChild(nod2);
                    doc.Save(fileName);
                    break;
            }
        }

        private void ExportToHTML(string fileName, string[] cols, List<SqlDataGridViewRow> rows, int[] indexes)
        {
            using (StreamWriter writter = new StreamWriter(fileName))
            {
                string s = "<html>" + Environment.NewLine + "<head>" + Environment.NewLine + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" + Environment.NewLine + "";
                s += "<title>" + tableName + "</title>" + Environment.NewLine + "<style type=\"text/css\" <!--" + Environment.NewLine + ".normal {  font-family: Verdana, Arial,";
                s += "Helvetica, sans-serif; font-size: 12px; font-weight: normal; color: #000000}";
                s += Environment.NewLine + ".medium {  font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 15px; font-weight: bold; color: #000000; text-decoration: none}--></style>";
                s += "</head>" + Environment.NewLine + "";
                s += "<h3>" + tableName + "</h3>" + Environment.NewLine + "<table border=1>" + Environment.NewLine;

                for (int i = 0; i < indexes.Length; i++)
                    s += "<td bgcolor=silver class=\'medium\'>" + cols[indexes[i]] + "</td>" + Environment.NewLine + "";
                writter.Write(s.ToCharArray());
                switch (mod)
                {
                    case ModExport.select:
                        string sel = "select ";
                        for (int i = 0; i < indexes.Length; i++)
                            sel += cols[indexes[i]] + ",";
                        sel = sel.Remove(sel.Length - 1, 1) + " from " +dataBase +"`.`" + tableName;
                        using (SQLiteCommand cmd = new SQLiteCommand(sel, connection))
                        {
                            SQLiteDataReader reader = cmd.ExecuteReader();
                            object[] temp = new object[reader.FieldCount];
                            try
                            {
                                while (reader.Read())
                                {
                                    reader.GetValues(temp);
                                    s = "<tr>" + Environment.NewLine + "";
                                    for (int j = 0; j < indexes.Length; j++)
                                    {
                                        s += "<td bgcolor=white class=\'normal\'>";
                                        s += FormatFieldByType(temp[j]);
                                        s += "</td>" + Environment.NewLine + "";
                                    }
                                    s += "</tr>" + Environment.NewLine + "";
                                    writter.Write(s.ToCharArray());
                                }
                                writter.WriteLine("</table>");
                                writter.WriteLine("</body>");
                                writter.WriteLine("</html>");
                            }
                            catch (Exception exception)
                            {
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                        break;
                    case ModExport.dgview:
                        StringBuilder strb = new StringBuilder();
                        for (int i = 0; i < rows.Count; i++)
                        {
                            strb.Append( "<tr>" + Environment.NewLine + "");
                            SqlDataGridViewRow row = rows[i];
                            for (int j = 0; j < indexes.Length; j++)
                            {
                                strb.Append( "<td bgcolor=white class=\'normal\'>");
                                object data = row.Data[indexes[j]];
                                strb.Append(FormatFieldByType(data));
                               strb.Append( "</td>" + Environment.NewLine + "");
                            }
                            strb.Append(  "</tr>" + Environment.NewLine + "");
                            writter.Write(strb.ToString());
                        }
                        writter.WriteLine("</table>");
                        writter.WriteLine("</body>");
                        writter.WriteLine("</html>");
                        break;

                }
                writter.Close();
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sFile = new SaveFileDialog();
            sFile.Filter = "csv files (*.csv)|*.txt|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sFile.ShowDialog();
            textBox5.Text = sFile.FileName;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedIndices.Count < 1)
            {
                MessageBox.Show("Please select at least one column", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            status.Visible = true;
            status.Text = "Please wait";
            progressBar1.Visible = true;
            progressBar1.MarqueeAnimationSpeed = 10;
            string[] cols = new string[listView1.CheckedItems.Count];
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    int[] selIndexesCsv = new int[listView1.CheckedIndices.Count];
                    for (int i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        selIndexesCsv[i] = listView1.CheckedIndices[i];
                    }
                    if (mod == ModExport.select)
                        backgroundWorker1.RunWorkerAsync(new object[] { columns, null, selIndexesCsv });
                    else
                        backgroundWorker1.RunWorkerAsync(new object[] { columns, (List<SqlDataGridViewRow>)dgView.Tag, selIndexesCsv });

                    break;
                case 1:
                    int[] selIndexes = new int[listView1.CheckedIndices.Count];
                    for (int i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        selIndexes[i] = listView1.CheckedIndices[i];
                    }
                    if(mod == ModExport.select)
                        backgroundWorker1.RunWorkerAsync(new object[] { columns,null, selIndexes });
                    else
                        backgroundWorker1.RunWorkerAsync(new object[] {	columns,(List<SqlDataGridViewRow>)dgView.Tag, selIndexes});
                    break;
                case 2:
                    int[] selIndexesHtml = new int[listView1.CheckedIndices.Count];
                    for (int i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        selIndexesHtml[i] = listView1.CheckedIndices[i];
                    }
                    if(mod == ModExport.select)
                        backgroundWorker1.RunWorkerAsync(new object[] { columns,null, selIndexesHtml });
                    else
                        backgroundWorker1.RunWorkerAsync(new object[] {	columns,(List<SqlDataGridViewRow>)dgView.Tag, selIndexesHtml});
                    break;
            }
        }

        private void tabControl1_SelectionChanged(object sender, EventArgs e)
        {
           
            int i= textBox5.Text.LastIndexOf('.');
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    filter = "CSV file|*.csv";
                    if (i > 0)
                        textBox5.Text = textBox5.Text.Remove(i, textBox5.Text.Length - i) + ".csv";
                    break;
                case 1:
                    filter = "XML file|*.xml";
                    if (i > 0)
                        textBox5.Text = textBox5.Text.Remove(i, textBox5.Text.Length - i) + ".xml";
                    break;
                case 2:
                    filter = "HTML file|*.html";
                    if (i > 0)
                        textBox5.Text = textBox5.Text.Remove(i, textBox5.Text.Length - i) + ".html";
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
                item.Checked = checkBox1.Checked;
        }

        private void ExportData_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = comboBox1.SelectedIndex;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    object[] argCsv = (object[])e.Argument;
                    List<SqlDataGridViewRow> rowsCsv = (List<SqlDataGridViewRow>)argCsv[1];
                    string[] colsCsv = (string[])argCsv[0];
                    int[] indexesCsv = (int[])argCsv[2];
                    ExportToCsv(textBox5.Text, colsCsv, rowsCsv, indexesCsv);
                    break;
                case 1:
                    object[] arg = (object[])e.Argument;
                    List<SqlDataGridViewRow> rows = (List<SqlDataGridViewRow>)arg[1];
                    string[] colsXml = (string[])arg[0];
                    int[] indexes = (int[])arg[2];
                    ExportToXml(textBox5.Text, colsXml, rows, indexes);
                    break;
                case 2:
                    object[] argHtml = (object[])e.Argument;
                    List<SqlDataGridViewRow> rowsHtml = (List<SqlDataGridViewRow>)argHtml[1];
                    string[] colsHtml = (string[])argHtml[0];
                    int[] indexesHtml = (int[])argHtml[2];
                    ExportToHTML(textBox5.Text, colsHtml, rowsHtml, indexesHtml);
                    break;

            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Visible = false;
            status.Text = "Data exported...";
        }

    }
}