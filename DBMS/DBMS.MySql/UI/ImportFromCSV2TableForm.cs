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
using MySql.Data.MySqlClient;

using System.IO;
using System.Data.Odbc;
using DBMS.core;

namespace DBMS.MySQL
{
    public partial class ImportFromCSV2TableForm : Form
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataAdapter da;
        private DataTable dt;
        private string dataBase;
        private IDBBrowser browser;
        private string currentTable;
        private string fileName;


        public ImportFromCSV2TableForm(MySqlConnection connection,IDBBrowser browser, string dataBase, string tableName)
        {
            this.dataBase = dataBase;
            this.connection = connection;
            this.browser = browser;
            this.currentTable = tableName;
            InitializeComponent();
            comboBox1.Items.Clear();
            if (dataBase == null)
                return;
            comboBox1.Items.AddRange((string[])browser.ListTables(dataBase).Result);
            comboBox1.Text = currentTable;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTable = comboBox1.Text;
            listView1.Items.Clear();
            string [] cols = (string[])browser.ListColumns(dataBase, currentTable).Result;
            for(int i=0;i<cols.Length;i++)
                listView1.Items.Add(cols[i]);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.Items)
                li.Checked = checkBox1.Checked;
        }


        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
            if (!e.Item.Checked)
                checkBox1.Checked = false;
            else
            {
                bool check = true;
                foreach (ListViewItem li in listView1.Items)
                {
                    if (!li.Checked)
                        check = false;
                }
                if (check)
                    checkBox1.Checked = true;
            }
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV files|*.CSV|Text files|*.txt";
            ofd.ShowDialog();
            fileName = ofd.FileName;
            textBox1.Text = fileName;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                checkBox4.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                checkBox3.Checked = false;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                checkBox6.Checked = false;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                checkBox5.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
                return;
            string s = "load data ";
            if (checkBox3.Checked)
                s += "concurrent ";
            if (checkBox4.Checked)
                s += "low_priority ";
            s+="local infile '" + fileName.Replace('\\','/') +"' ";
            if (checkBox5.Checked)
                s += "ignore ";
            if (checkBox6.Checked)
                s += "replace ";
            s += "into table `" + dataBase + "`.`" + currentTable + "` ";
            s += "fields escaped by '" + FormatText(comboBox4.Text)+"'";
            s += " terminated by '" +FormatText(comboBox2.Text) +"'";
            s += " enclosed by '" + FormatText(comboBox3.Text) +"'";

            s += " lines terminated by '" + FormatText(comboBox7.Text) +"'";
            if (checkBox2.Checked)
                s += " ignore " + numericUpDown1.Value.ToString() + " lines ";
            s += " (";
            int k = 0;
            foreach (ListViewItem li in listView1.CheckedItems)
            {
                if (k == 0)
                    s += "`" + li.Text + "`";
                else
                    s += ",`" + li.Text + "`";
                k++;
            }

            s += ");";
            cmd = new MySqlCommand(s, connection);
            cmd.ExecuteNonQuery();
        }


        string FormatText(string text)
        {
            string ret = text;
            ret = ret.Replace("\\t", "\t");
            ret = ret.Replace("\\t", "\t");
            return ret;
        }

        private void ImportFromCSV2TableForm_Load(object sender, EventArgs e)
        {

        }
    }
}