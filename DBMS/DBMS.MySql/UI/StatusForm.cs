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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.Odbc;

namespace DBMS.MySQL
{
    public partial class StatusForm : Form
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataAdapter da;
        private DataTable dt;

        public StatusForm(MySqlConnection connection)
        {
            this.connection = connection;
            dt = new DataTable();
            InitializeComponent();
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {
            GetServerStatus();
            GetVariableStatus();
            GetProcessList();
        }

        private void GetProcessList()
        {
            string x = "show processlist";
            cmd = new MySqlCommand(x, connection);
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            listView3.Items.Clear();
            listView3.Columns.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
             ColumnHeader ch = listView3.Columns.Add(dt.Columns[i].ColumnName);
             ch.Width = 150;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] it = new string[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if(j!=1&&j!=2&&j!=3&&j!=3&&j!=4&&j!=6&&j!=7)
                        it[j] = dt.Rows[i][j].ToString();
                    else
                    {
                        if(dt.Rows[i][j] != DBNull.Value)
                        {
                            if (dt.Rows[i][j].GetType() == typeof(byte[]))
                                it[j] = Encoding.Default.GetString((byte[])dt.Rows[i][j]);
                            else
                                if (dt.Rows[i][j].GetType() == typeof(string))
                                    it[j] = (string)dt.Rows[i][j];
                        }
                    }
                }
                ListViewItem li = new ListViewItem(it);
                listView3.Items.Add(li);
            }
        }

        private void GetVariableStatus()
        {
            string x = "show variables";
            cmd = new MySqlCommand(x, connection);
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            listView2.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                listView2.Items.Add(new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString() }));
            }
        }

        private void GetServerStatus()
        {
            string x = "show status";
            cmd = new MySqlCommand(x, connection);
            da = new MySqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            listView1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                listView1.Items.Add(new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString() }));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            GetServerStatus();
            GetVariableStatus();
            GetProcessList();
        }

    }
}