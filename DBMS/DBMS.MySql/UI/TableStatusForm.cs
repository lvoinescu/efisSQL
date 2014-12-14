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
using MySql.Data.MySqlClient;

namespace DBMS.MySQL
{
    public partial class TableStatusForm :Form
    {
        private string database, tablename;
        MySqlConnection connection;
        public TableStatusForm()
        {
            InitializeComponent();
        }
        public TableStatusForm(string database, string tablename,MySqlConnection connection)
        {
            this.database = database;
            this.connection = connection;
            this.tablename = tablename;
            InitializeComponent();
        }

        private void TableStatusForm_Load(object sender, EventArgs e)
        {
            try
            {
                string s = "show table status from `" + database + "` where name ='" + tablename + "';";
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, connection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        if (dt.Rows.Count < 1)
                            return ;
                        rez = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            ListViewItem li = new ListViewItem(new string[] {dt.Columns[i].ColumnName,dt.Rows[0][i].ToString() });
                            listView1.Items.Add(li);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
            }
            catch (OdbcException ex)
            {
            }

        }

 
    }
}