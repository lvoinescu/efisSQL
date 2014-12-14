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

namespace DBMS.MySQL
{
    public partial class AlterDataBase : Form
    {
        private MySqlConnection connection;
        private string dataBase;
        public AlterDataBase()
        {
            InitializeComponent();
        }
        public AlterDataBase(MySqlConnection connection, string dataBase)
        {
            this.connection = connection;
            this.dataBase = dataBase;
            InitializeComponent();
            textBox1.Text = dataBase;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string charsetName = charSet.Text;
                string collationName = this.collate.Text;
                using (MySqlCommand cmd = new MySqlCommand("alter database `" + dataBase + "` CHARACTER SET " + charsetName + " COLLATE " + collationName + ";", connection))
                {
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AlterDataBase_Load(object sender, EventArgs e)
        {
            using (MySqlCommand cmd = new MySqlCommand("use `" +dataBase+"`;show variables like \"character_set_database\";", connection))
            {
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    charSet.Text = dt.Rows[0]["Value"].ToString().ToLower();

                    cmd.CommandText ="show variables like \"collation_database\";";
                    da = new MySqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);
                    collate.Text = dt.Rows[0]["Value"].ToString().ToLower();

                }
                catch (Exception ex)
                {
                }
            }
        }


    }
    
 
    
    
}
