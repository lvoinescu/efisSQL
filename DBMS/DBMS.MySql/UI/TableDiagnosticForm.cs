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
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using DBMS.core;

namespace DBMS.MySQL
{
    public partial class TableDiagnosticForm : Form
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataAdapter da;
        private DataTable dt;
        private IDBBrowser browser;
        private string activeDatabase;
        public TableDiagnosticForm(MySqlConnection connection,IDBBrowser browser, string activeDatabase)
        {
            this.connection = connection;
            this.browser = browser;
            this.activeDatabase = activeDatabase;
            da = new MySqlDataAdapter();
            dt = new DataTable();
            InitializeComponent();
        }

        private void TableDiagnosticForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange((string[])browser.ListDatabases().Result);
            comboBox1.Text = activeDatabase;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            object rez = browser.ListTables(comboBox1.Text).Result;
            if ((string[])rez != null)
            {
                string[] s = (string[])browser.ListTables(comboBox1.Text).Result;
                for (int i = 0; i < s.Length; i++)
                {
                    listView1.Items.Add(s[i],s[i]);
                }
            }
        }


        private void CheckTables()
        {
            if (listView1.CheckedItems.Count == 0)
                return;
            string s = "check table ";
            int k = 0;
            foreach (ListViewItem li in listView1.CheckedItems)
            {
                if (k != 0)
                    s += ",";
                s += "`" + comboBox1.Text + "`.`" + li.Text + "`";
                k++;
            }
            cmd = new MySqlCommand(s, connection);
            cmd.CommandTimeout = 600;
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } if (dt.Rows.Count > 0)
            {
                listView2.Items.Clear();
                listView2.Columns.Clear();
                for(int j=0;j<dt.Columns.Count;j++)
                    listView2.Columns.Add(dt.Columns[j].ColumnName);
                listView2.Columns[0].Width = 200;
                listView2.Columns[1].Width = 100;
                listView2.Columns[2].Width = 100;
                listView2.Columns[3].Width = 300;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string [] li= new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                        if(dt.Rows[i][j].GetType()== typeof(byte[]))
                            li[j] = Encoding.Default.GetString((byte[])dt.Rows[i][j]);
                        else
                            if(dt.Rows[i][j].GetType()== typeof(string))
                                li[j] = (string)dt.Rows[i][j];
                    listView2.Items.Add(new ListViewItem(li));
                }
            }
        }

        private void AnalyzeTables()
        {
            if (listView1.CheckedItems.Count == 0)
                return;
            string s = "analyze table ";
            int k = 0;
            foreach(ListViewItem li in listView1.CheckedItems)
            {
                if(k!=0)
                    s += ",";
                s+="`" + comboBox1.Text + "`.`" + li.Text + "`";
                k++;
            }
            cmd = new MySqlCommand(s, connection);
            cmd.CommandTimeout = 600;
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } if (dt.Rows.Count > 0)
            {
                listView3.Items.Clear();
                listView3.Columns.Clear();
                for (int j = 0; j < dt.Columns.Count; j++)
                    listView3.Columns.Add(dt.Columns[j].ColumnName);
                listView3.Columns[0].Width = 200;
                listView3.Columns[1].Width = 100;
                listView3.Columns[2].Width = 100;
                listView3.Columns[3].Width = 300;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] li = new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                        if (dt.Rows[i][j].GetType() == typeof(byte[]))
                            li[j] = Encoding.Default.GetString((byte[])dt.Rows[i][j]);
                        else
                            if (dt.Rows[i][j].GetType() == typeof(string))
                                li[j] = (string)dt.Rows[i][j];
                    listView3.Items.Add(new ListViewItem(li));
                }
            }
        }

        private void OptimizeTables()
        {
            if (listView1.CheckedItems.Count == 0)
                return;
            string s = "optimize table ";
            int k = 0;
            foreach (ListViewItem li in listView1.CheckedItems)
            {
                if (k != 0)
                    s += ",";
                s += "`" + comboBox1.Text + "`.`" + li.Text + "`";
                k++;
            }
            cmd = new MySqlCommand(s, connection);
            cmd.CommandTimeout = 600;
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (dt.Rows.Count > 0)
            {
                listView4.Items.Clear();
                listView4.Columns.Clear();
                for (int j = 0; j < dt.Columns.Count; j++)
                    listView4.Columns.Add(dt.Columns[j].ColumnName);
                listView4.Columns[0].Width = 200;
                listView4.Columns[1].Width = 100;
                listView4.Columns[2].Width = 100;
                listView4.Columns[3].Width = 300;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] li = new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                        if (dt.Rows[i][j].GetType() == typeof(byte[]))
                            li[j] = Encoding.Default.GetString((byte[])dt.Rows[i][j]);
                        else
                            if (dt.Rows[i][j].GetType() == typeof(string))
                                li[j] = (string)dt.Rows[i][j];
                    listView4.Items.Add(new ListViewItem(li));
                }
            }
        }

        private void RepairTables()
        {
            if (listView1.CheckedItems.Count == 0)
                return;
            string s = "repair table ";
            int k = 0;
            foreach (ListViewItem li in listView1.CheckedItems)
            {
                if (k != 0)
                    s += ",";
                s += "`" + comboBox1.Text + "`.`" + li.Text + "`";
                k++;
            }
            cmd = new MySqlCommand(s, connection);
            cmd.CommandTimeout = 600;
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } if (dt.Rows.Count > 0)
            {
                listView5.Items.Clear();
                listView5.Columns.Clear();
                for (int j = 0; j < dt.Columns.Count; j++)
                    listView5.Columns.Add(dt.Columns[j].ColumnName);
                listView5.Columns[0].Width = 200;
                listView5.Columns[1].Width = 100;
                listView5.Columns[2].Width = 100;
                listView5.Columns[3].Width = 300;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] li = new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                        if (dt.Rows[i][j].GetType() == typeof(byte[]))
                            li[j] = Encoding.Default.GetString((byte[])dt.Rows[i][j]);
                        else
                            if (dt.Rows[i][j].GetType() == typeof(string))
                                li[j] = (string)dt.Rows[i][j];
                    listView5.Items.Add(new ListViewItem(li));
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            CheckTables();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RepairTables();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnalyzeTables();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OptimizeTables();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listView1.ItemChecked -= listView1_ItemChecked;
            foreach (ListViewItem li in listView1.Items)
            {
                li.Checked = checkBox1.Checked;
            }
            listView1.ItemChecked += listView1_ItemChecked;
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

    }
}