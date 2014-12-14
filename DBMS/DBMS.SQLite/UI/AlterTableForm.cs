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
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace DBMS.SQLite
{
    public partial class AlterTableForm : Form
    {
        private SQLiteConnection connection;
        private string dataBase ,tableName;
        private List<string> rowsDeleted;
        public enum Mod { Alter, Create };
        public AlterTableForm(SQLiteConnection conection, string dataBase, string tableName, Mod mod)
        {
            this.mod = mod;
            this.connection = conection;
            this.dataBase = dataBase;
            this.tableName = tableName;
            rowsDeleted = new List<string>();
            InitializeComponent();
        }
        public Mod mod;
        private void AlterTableForm_Load(object sender, EventArgs e)
        {
            if (mod == Mod.Alter)
            {
                button1.Text = "Alter";
                LoadColumns();
                LoadAdvanced();
                table.Enabled = false;
                table.Text = tableName;
                this.Text = "Alter table " + tableName;
                dg1.Columns[9].ReadOnly = true;
            }
            else
            {
                this.Text = "Create table";
                button1.Text = "Create";
            }
        }

        private void LoadAdvanced()
        {


        }

        private void LoadColumns()
        {
            dg1.Rows.Clear();
            rowsDeleted = new List<string>();
            using (SQLiteCommand cmd = new SQLiteCommand("pragma `" +dataBase+"`.table_info(`"+ tableName + "`);", connection))
            {
                using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        ad.Fill(dt);
                        try
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                DataGridViewRow dgr = new DataGridViewRow();
                                dgr.CreateCells(dg1);
                                dgr.Cells[0].Value = dt.Rows[i]["name"].ToString();
                                Regex reg = new Regex("[a-zA-Z]*\\s*\\(.+\\)");
                                string mixedType = dt.Rows[i]["Type"].ToString();
                                Match m = reg.Match(mixedType);
                                string pureType = "";
                                string auxeType = "";
                                string len = "";
                                if (m.Length == 0)
                                {
                                    pureType = mixedType;
                                }
                                else
                                {
                                    Regex reg2 = new Regex("[a-zA-Z]*");
                                    Match m2 = reg2.Match(m.ToString());
                                    pureType = m2.ToString();
                                    auxeType = mixedType.Remove(m2.Index, m2.Length);
                                    auxeType = auxeType.Remove(auxeType.IndexOf(')')).TrimStart('(');
                                    len = auxeType;
                                }
                                string def = dt.Rows[i]["dflt_value"].ToString();
                                dgr.Cells[1].Value = pureType;
                                dgr.Cells[2].Value = len;
                                dgr.Cells[3].Value = def;
                                dgr.Cells[5].Value = (dt.Rows[i]["pk"].ToString().ToUpper() == "1");
                                dgr.Cells[7].Value = (dt.Rows[i]["notnull"].ToString().ToUpper() == "NO");
                                dgr.Tag = dgr.Cells[0].Value;
                                dgr.Frozen = true;
                                dgr.ReadOnly = true;
                                dg1.Rows.Add(dgr);

                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            using (SQLiteCommand cmd = new SQLiteCommand("pragma `" +dataBase+"`.foreign_key_list (`" + tableName + "`);", connection))
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                        for (int j = 0; j < dg1.Rows.Count-1; j++)
                        {
                            if (dg1.Rows[j].Cells[0].Value.ToString().ToLower() == dt.Rows[i]["from"].ToString().ToLower())
                                dg1.Rows[j].Cells[9].Value = dt.Rows[i]["table"].ToString() + "(" + dt.Rows[i]["to"].ToString() + ")";
                        }
                        //listView1.Items.Add(new ListViewItem(new string[] { dt.Rows[i]["to"].ToString(), dt.Rows[i]["table"].ToString(), dt.Rows[i]["from"].ToString() }));
                }
                dt.Dispose();
                da.Dispose();
            }

        }

        private string PutDefaultValueApostrophe(string value, string type)
        {
            switch (type.ToLower())
            {
                case "varchar":
                case "text":
                case "char":
                case "date":
                case "datetime":
                case "time":
                case "varbinary":
                case "enum":
                case "set":
                    return "'"+value.Replace("\'","\\'")+"'";
                default:
                    return value;
            }

        }

        private void AlterIt()
        {
            bool ok = true;
            for (int i = 0; i < dg1.Rows.Count - 1; i++)
            {
                DataGridViewRow dgr = dg1.Rows[i];
                if (dgr.Cells[0].Value == null)
                {
                    ok = false;
                    MessageBox.Show("Please provide a column name", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                }

                if (!dg1.Rows[i].ReadOnly)
                {
                    string alterString = "alter table `" + dataBase + "`.`" + tableName + "` ";
                    alterString += "add column ";

                    alterString += " `" + dgr.Cells[0].Value.ToString() + "` "; //col name
                    alterString += dgr.Cells[1].Value.ToString() + " "; //tip
                    if (dgr.Cells[2].Value != null)
                    {
                        alterString += " (" + dgr.Cells[2].Value.ToString() + ")";
                    }

                    alterString += ((bool)dgr.Cells[6].FormattedValue == true) ? " not null " : " null ";

                    if (dgr.Cells[3].Value != null)
                        if (dgr.Cells[3].Value.ToString() != "")
                            alterString += " default '" + dgr.Cells[3].Value.ToString() + "'";

                    if (dgr.Cells[4].Value != null)
                        if (dgr.Cells[4].Value.ToString() != "")
                            alterString += " collate " + dgr.Cells[4].Value.ToString() + "";


                    if (dgr.Cells[6].FormattedValue != null)
                        if ((bool)dgr.Cells[6].FormattedValue)
                            alterString += " auto_increment";
                    SQLiteCommand cmd = new SQLiteCommand(alterString, connection);
                    try
                    {
                        if (ok)
                            cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
            }
           
           
        }

        private void Create()
        {
            this.tableName = table.Text;
            string createString = "create table `" +dataBase +"`.`" + tableName + "` (";
            string pk = "";
            for (int i = 0; i < dg1.Rows.Count - 1; i++)
            {
                DataGridViewRow dgr = dg1.Rows[i];
                createString += " `" + dgr.Cells[0].Value.ToString() + "` "; //col name
                createString += dgr.Cells[1].Value.ToString() + " "; //tip
                if (dgr.Cells[2].Value != null)
                {
                    createString += " (" + dgr.Cells[2].Value.ToString() + ")";
                }

               

                if (dgr.Cells[3].Value != null)
                    if (dgr.Cells[3].Value.ToString() != "")
                        createString += " default '" + dgr.Cells[3].Value.ToString() + "'";

                if (dgr.Cells[4].Value != null)
                    if (dgr.Cells[4].Value.ToString() != "")
                        createString += " collate " + dgr.Cells[4].Value.ToString() + " ";

                if (dgr.Cells[5].Value != null)
                    if (dgr.Cells[5].Value.ToString() != "")
                        pk += "`" + dgr.Cells[0].Value.ToString() + "`,";
                
                if (dgr.Cells[6].FormattedValue != null)
                    if ((bool)dgr.Cells[6].FormattedValue)
                        createString += " auto_increment";

                createString += ((bool)dgr.Cells[6].FormattedValue == true) ? " not null " : " null ";
                
                createString += ",";

            }

            for (int i = 0; i < dg1.Rows.Count - 1; i++)
            {
                DataGridViewRow dgr = dg1.Rows[i];
                if (dgr.Cells[9].Value != null)
                {
                    createString += ", foreign key (" + dgr.Cells[0].Value.ToString() + ") references " + dgr.Cells[9].Value.ToString();
                    createString += ",";
                }
            }


            if (pk != "")
            {
                pk = pk.Remove(pk.Length - 1, 1);
                createString += "primary key (" + pk + ") ";
            }
            createString = createString.Remove(createString.Length - 1, 1) +") ";
            SQLiteCommand cmd = new SQLiteCommand(createString, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dg1.Rows.Count <= 1)
            {
                MessageBox.Show("Table must have at least one column.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (mod == Mod.Alter)
            {
                AlterIt();
            }
            else
            {
                if (table.Text.Trim() != "")
                    Create();
                else
                    MessageBox.Show("Please provide a table name.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mod == Mod.Alter)
            {
                LoadColumns();
                LoadAdvanced();
            }
        }

        private void dg1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.ReadOnly)
            {
                MessageBox.Show("You cannot delete an exisiting column in SQLite", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void dg1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dg1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 9 && !dg1.Rows[e.RowIndex].ReadOnly && e.RowIndex<dg1.Rows.Count-1)
            {
                SelectForeignKeyForm skForm = new SelectForeignKeyForm(connection);
                skForm.ShowDialog();
                if (!skForm.Canceled)
                {
                    dg1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = skForm.SelectedTable + "(" + skForm.SelectedColumn + ")";
                    skForm.Dispose();
                }
            }
        }



    }
}