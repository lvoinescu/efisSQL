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
    public partial class NewDbForm : Form
    {
        private MySqlConnection connection;
        private bool dataBaseCreated;

        public bool DataBaseCreated
        {
            get { return dataBaseCreated; }
            set { dataBaseCreated = value; }
        }
        private string dataBaseName;

        public string DataBaseName
        {
            get { return dataBaseName; }
            set { dataBaseName = value; }
        }
        public NewDbForm()
        {
            InitializeComponent();
        }


        public NewDbForm(MySqlConnection connection)
        {
            this.connection = connection;
            dataBaseCreated = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string charsetName = charSet.Text;
                string collationName = this.collate.Text;
                using (MySqlCommand cmd = new MySqlCommand("create database `" + textBox1.Text + "` CHARACTER SET "+charsetName +" COLLATE "+ collationName+";", connection))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Database `"+textBox1.Text+"` created.",Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    dataBaseCreated = true;
                    dataBaseName = textBox1.Text;
                }
            }
            catch(Exception ex)
            {
                dataBaseCreated = false;
                MessageBox.Show(ex.Message,Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void NewDbForm_Load(object sender, EventArgs e)
        {
            charSet.SelectedIndex = 0;
            collate.SelectedIndex = 0;
        }
    }
}