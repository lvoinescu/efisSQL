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
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DBMS.MySQL
{
    public partial class NewReferenceForm : Form
    {
        string dataBase, tableName;
        MySqlConnection connection;
        MySqlCommand cmd;
        MySqlDataAdapter da;
        public NewReferenceForm()
        {
            InitializeComponent();
        }

        public NewReferenceForm(string dataBase, string tableName, MySqlConnection connection)
        {
            this.connection = connection;
            this.dataBase = dataBase;
            this.tableName = tableName;
            InitializeComponent();
        }

        void Init()
        {
            comboBox1.Items.AddRange(GetTablesFromDb(dataBase));
            DataGridViewComboBoxColumn col = dg1.Columns[0] as DataGridViewComboBoxColumn;
            col.Items.Clear();
            col.Items.AddRange(GetColumnsForTable(dataBase,tableName));

        }

        string[] GetColumnsForTable(string database, string table)
        {
            MySqlCommand cmd = new MySqlCommand("show fields from `" + database + "`.`" + table + "`;", connection);
            da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string []rez = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                rez[i]=dt.Rows[i][0].ToString();
            return rez;
        }

        string[] GetTablesFromDb(string database)
        {
            MySqlCommand cmd = new MySqlCommand("show tables from `" + database + "`;", connection);
            da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string[] rez = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                rez[i] = dt.Rows[i][0].ToString();
            return rez;
        }


        private void NewReferenceForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewComboBoxColumn col = dg1.Columns[1] as DataGridViewComboBoxColumn;
            col.Items.Clear();
            for (int i = 0; i < dg1.Rows.Count - 1; i++)
                dg1.Rows[i].Cells[1].Value = "";
            col.Items.AddRange(GetColumnsForTable(dataBase, comboBox1.Text));
            col.Items.Add("");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = checkBox2.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string listst = "", listdr = "";
            if (dg1.Rows.Count < 2)
                return;
            listst += "`" + dg1.Rows[0].Cells[0].Value.ToString() + "`";
            listdr += "`" + dg1.Rows[0].Cells[1].Value.ToString() + "`";
            for (int i = 1; i < dg1.Rows.Count - 1; i++)
            {
                listst += ",`" + dg1.Rows[0].Cells[0].Value.ToString() + "`";
                listdr += ",`" + dg1.Rows[0].Cells[1].Value.ToString() + "`";
            }
            string cString = "alter table `" + dataBase + "`.`" + tableName + "` add constraint `" + textBox1.Text + "` foreign key ("+listst+") references `" + comboBox1.Text + "` ("+listdr+")";


            if (checkBox2.Checked)
            {
                if (radioButton5.Checked)
                    cString += " on delete cascade";
                else
                    if (radioButton6.Checked)
                        cString += " on delete no action";
                    else
                        if (radioButton7.Checked)
                            cString += " on delete set null";
                        else
                            if (radioButton8.Checked)
                                cString += " on delete restrict";
            }
             if (checkBox1.Checked)
            {
                if (radioButton1.Checked)
                    cString += " on update cascade";
                else
                    if (radioButton2.Checked)
                        cString += " on update no action";
                    else
                        if (radioButton3.Checked)
                            cString += " on update set null";
                        else
                            if (radioButton4.Checked)
                                cString += " on update restrict";
            }
            
            cmd = new MySqlCommand(cString, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}