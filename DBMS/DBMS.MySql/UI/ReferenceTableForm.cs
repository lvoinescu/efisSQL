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
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.Odbc;

namespace DBMS.MySQL
{
    public partial class ReferenceTableForm : Form
    {
        string dataBase, tableName;
        MySqlConnection connection;
        MySqlCommand cmd;
        MySqlDataAdapter da;
        public ReferenceTableForm()
        {
            InitializeComponent();
        }

        public ReferenceTableForm(string dataBase, string tableName, MySqlConnection connection)
        {
            this.dataBase = dataBase;
            this.tableName = tableName;
            this.connection = connection;
            InitializeComponent();
            this.Text = "References on " + dataBase + "." + tableName;
        }

        private void ReferenceTableForm_Load(object sender, EventArgs e)
        {
            RefreshFK();
        }

        void RefreshFK()
        {
            cmd = new MySqlCommand("USE information_schema;SELECT * FROM KEY_COLUMN_USAGE WHERE TABLE_NAME = '"+tableName+"' and constraint_name<>'PRIMARY' and table_schema='"+dataBase+"';",connection);
            da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            listView1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = GetIdForKey(listView1, dt.Rows[i]["constraint_name"].ToString());
                if(id<0)
                {
                    ListViewItem it = new ListViewItem(new string[] { dt.Rows[i]["constraint_name"].ToString(), dt.Rows[i]["column_name"].ToString(), dt.Rows[i]["referenced_table_name"].ToString(), dt.Rows[i]["referenced_column_name"].ToString() });
                    listView1.Items.Add(it);
                }
                else
                {
                    listView1.Items[id].SubItems[1].Text += "," + dt.Rows[i]["column_name"].ToString();
                    listView1.Items[id].SubItems[2].Text += "," + dt.Rows[i]["referenced_table_name"].ToString();
                    listView1.Items[id].SubItems[3].Text += "," + dt.Rows[i]["referenced_column_name"].ToString();
                }
            }

        }

        int GetIdForKey(ListView lv, string key)
        {
            foreach (ListViewItem it in lv.Items)
            {
                if (it.Text == key)
                    return it.Index;
            }
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
                return;
            try
            {
                cmd = new MySqlCommand("alter table `" + dataBase + "`.`" + tableName + "` drop foreign key `" + listView1.SelectedItems[0].Text + "`;", connection);
                cmd.ExecuteNonQuery();
                RefreshFK();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewReferenceForm nrForm = new NewReferenceForm(dataBase, tableName, connection);
            nrForm.ShowDialog();
            this.RefreshFK();
        }

    }
}