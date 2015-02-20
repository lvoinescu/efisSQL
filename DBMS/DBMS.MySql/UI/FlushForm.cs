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
    public partial class FlushForm : Form
    {
        MySqlCommand cmd = new MySqlCommand();
        MySqlConnection connection;
        private bool auxBool = false;

        public FlushForm()
        {
            InitializeComponent();
        }


        public FlushForm(MySqlConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = "";
            string nwrb = "";
            if (checkBox10.Checked)
            {
                nwrb = "NO_WRITE_TO_BINLOG";
            }
            if (checkBox1.Checked)
                s += "flush " + nwrb + " logs;";
            if (checkBox2.Checked)
                s += "flush " + nwrb + " privileges;";
            if (checkBox3.Checked)
                s += "flush " + nwrb + " tables;";
            if (checkBox4.Checked)
                s += "flush " + nwrb + " hosts;";
            if (checkBox5.Checked)
                s += "flush " + nwrb + " status;";
            if (checkBox6.Checked)
                s += "flush " + nwrb + " user_resources;";
            if (checkBox7.Checked)
                s += "flush " + nwrb + " des_key_file;";
            if (checkBox8.Checked)
                s += "flush " + nwrb + " tables with read lock;";
            if (checkBox9.Checked)
                s += "flush query cache;";
            try
            {
                if (s == "")
                    return;
                cmd = new MySqlCommand(s, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Flush succesful");
            }
            catch(Exception ex)
            {

            }
            try
            {
                cmd = new MySqlCommand("unlock tables", connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                checkBox1.Checked = true;
                checkBox2.Checked = true;
                checkBox3.Checked = true;
                checkBox4.Checked = true;
                checkBox5.Checked = true;
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox8.Checked = true;
                checkBox9.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            if (!ch.Checked)
            {
                checkBox11.CheckedChanged -= checkBox11_CheckedChanged;
                checkBox11.Checked = false;
                checkBox11.CheckedChanged += checkBox11_CheckedChanged;
                return;
            }
            bool all = true;
            foreach (Control c in groupBox1.Controls)
            {
                if(c is CheckBox)
                {
                    if (!(c as CheckBox).Checked)
                        all = false;
                }
            }
            if (all)
            {
                checkBox11.CheckedChanged -= checkBox11_CheckedChanged;
                checkBox11.Checked = true;
                checkBox11.CheckedChanged += checkBox11_CheckedChanged;

            }
        }
    }
}