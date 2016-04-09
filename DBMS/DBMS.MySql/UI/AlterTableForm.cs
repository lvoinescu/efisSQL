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
using System.Data;
using System.Windows.Forms;
using DBMS.MySQL;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace DBMS.MySQL
{
    public partial class AlterTableForm : Form
    {
        private bool advanced;
        private bool tableCreated;
        private List<string> initialPrimaryKeys;
        public bool TableCreated
        {
            get { return tableCreated; }
            set { tableCreated = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        private MySqlConnection conection;
        private string dataBase ,tableName;
        private List<string> rowsDeleted;
        public enum Mod { Alter, Create };
        public AlterTableForm(MySqlConnection conection, string dataBase, string tableName, Mod mod)
        {
            this.mod = mod;
            advanced = false;
            this.conection = conection;
            this.dataBase = dataBase;
            this.tableName = tableName;
            rowsDeleted = new List<string>();
            InitializeComponent();
           // SwitchLanguage();
        }
        public Mod mod;
        private void AlterTableForm_Load(object sender, EventArgs e)
        {
            outPut.Text = "";
            if (mod == Mod.Alter)
            {
                initialPrimaryKeys = new List<string>();
                button1.Text = "Alter";
                LoadColumns();
                LoadAdvanced();
                table.Enabled = false;
                this.Text = "Alter table " + dataBase+"." + tableName;
            }
            else
            {
                this.Text = "Create table in `" + dataBase +"` database";
                button1.Text = "Create";
            }
            SetMode(advanced);
        }

        private void LoadAdvanced()
        {
            try
            {
                table.Text = tableName;
                using (MySqlCommand cmd = new MySqlCommand("use `"+ dataBase +"`;", conection))
                {
                    cmd.ExecuteNonQuery();
                    string s = "show table status from`" + dataBase + "`;";
                    cmd.CommandText = s;
                    using (MySqlDataAdapter ad = new MySqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            ad.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                int k = 0;
                                while (dt.Rows[k]["name"].ToString() != tableName && k < dt.Rows.Count)
                                {
                                    k++;
                                }
                                engine.SelectedItem = dt.Rows[k]["Engine"].ToString();
                                checksum.SelectedItem = dt.Rows[k]["checksum"] == DBNull.Value ? "DEFAULT" : dt.Rows[k]["check_sum"].ToString();
                                autoincrement.Text = dt.Rows[k]["auto_increment"] == DBNull.Value ? "" : dt.Rows[k]["auto_increment"].ToString();
                                avgRowLen.Text = dt.Rows[k]["avg_row_length"] == DBNull.Value ? "" : dt.Rows[k]["avg_row_length"].ToString();
                                comment.Text = dt.Rows[k]["comment"] == DBNull.Value ? "" : dt.Rows[k]["comment"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void LoadColumns()
        {
            initialPrimaryKeys = new List<string>();
            dg1.Rows.Clear();
            rowsDeleted = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand("show full fields from `" + dataBase + "`.`" + tableName + "`;", conection))
            {
                using (MySqlDataAdapter ad = new MySqlDataAdapter(cmd))
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
                                dgr.Cells[0].Value = dt.Rows[i][0].ToString();
                                string mixedType = dt.Rows[i]["Type"].ToString();
                                Regex reg = new Regex("[a-zA-Z]*\\s*\\(.+\\)");
 
                                Match m = reg.Match(mixedType);
                                string pureType = "";
                                string auxeType = "";
                                if(m.Length==0)
                                {
                                    pureType = mixedType;
                                }
                                else
                                {
                                    Regex reg2 =new Regex("[a-zA-Z]*");
                                    Match m2 = reg2.Match(m.ToString());
                                    pureType = m2.ToString();
                                    auxeType = mixedType.Remove(m2.Index, m2.Length);
                                    auxeType = auxeType.Remove(auxeType.IndexOf(')')).TrimStart('(');
                                    
                                }
                                string def = dt.Rows[i]["default"].ToString();
                                dgr.Cells[1].Value = pureType;
                                dgr.Cells[2].Value = auxeType;
                                dgr.Cells[3].Value = def;
                                dgr.Cells[4].Value = (dt.Rows[i]["Collation"].ToString());
                                if (dt.Rows[i]["Key"].ToString().ToUpper() == "PRI")
                                    initialPrimaryKeys.Add(dt.Rows[i][0].ToString());
                                dgr.Cells[5].Value = (dt.Rows[i]["Key"].ToString().ToUpper() == "PRI");
                                dgr.Cells[6].Value = (dt.Rows[i]["Extra"].ToString().ToUpper().Contains("AUTO_INCREMENT"));
                                dgr.Cells[7].Value = (dt.Rows[i]["Null"].ToString().ToUpper() == "NO");
                                dgr.Cells[8].Value = (mixedType.Contains("zerofill"));
                                dgr.Cells[9].Value = (mixedType.Contains("unsigned"));
                                dgr.Cells[10].Value = (dt.Rows[i]["comment"].ToString());
                                dgr.Tag = dgr.Cells[0].Value.ToString();
                                dg1.Rows.Add(dgr);

                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
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
            string alterString = "alter table `" + dataBase + "`.`" + tableName + "` ";
            bool autoIncrCol = false;
            List<string> pKeys = new List<string>();
            for (int i = 0; i < dg1.Rows.Count - 1; i++)
            {
                DataGridViewRow dgr = dg1.Rows[i];
                bool newFirst = false;
                if (dgr.Tag != null)
                { 
                    string idm = dgr.Tag.ToString();
                    alterString += "change `" + idm + "` ";
                }
                else
                {
                    alterString += "add column ";
                    newFirst = (i == 0);
                }
                if (dgr.Cells[0].Value == null)
                {
                    MessageBox.Show("Please set names for each introduced columns");
                    return;
                }
                alterString += "`" + dgr.Cells[0].Value.ToString() + "` "; //col name
                if (dgr.Cells[1].Value == null)
                {
                    MessageBox.Show("Please select a type for the `" + dgr.Cells[0].Value.ToString() + "` columns");
                    return;
                }
                alterString += dgr.Cells[1].Value.ToString() + " "; //tip
                if (dgr.Cells[2].Value != null)
                {
                    if(dgr.Cells[2].Value.ToString().Trim()!="")
                        alterString += "(" + dgr.Cells[2].Value.ToString() + ") ";
                }

                alterString += ((bool)dgr.Cells[9].FormattedValue == true) ? "unsigned " : " ";
                alterString += (autoIncrCol =(bool)dgr.Cells[6].FormattedValue == true) ? "auto_increment " : " ";

                if (dgr.Cells[3].Value != null)
                {
                    if (dgr.Cells[3].Value.ToString() != "")
                        alterString += "default " + PutDefaultValueApostrophe(dgr.Cells[3].Value.ToString(),dgr.Cells[1].Value.ToString()) + " ";
                }
                
                if (dgr.Cells[4].Value != null)
                {
                    if (dgr.Cells[4].Value.ToString() != "")
                        alterString += "collate " + dgr.Cells[4].Value.ToString() + " ";
                }
                DataGridViewCheckBoxCell cCell = (DataGridViewCheckBoxCell)dgr.Cells[5];
                if ((bool)cCell.FormattedValue== true)
                {
                    pKeys.Add(dgr.Cells[0].Value.ToString());
                }

                alterString += ((bool)dgr.Cells[5].FormattedValue == true) ? "not null " : " ";
                if (newFirst)
                {
                    alterString += "first";
                }
                if (dgr.Index>0 && dgr.Tag == null)
                        alterString += " after `" + dg1.Rows[dgr.Index - 1].Cells[0].Value.ToString() + "`";
                if(i<dg1.Rows.Count-2)
                    alterString += ",";
            }
            if (rowsDeleted.Count > 0)
            {
                for (int j = 0; j < rowsDeleted.Count; j++)
                {
                    alterString += ",drop column `" + rowsDeleted[j] + "`";
                }
            }
            if (initialPrimaryKeys.Count > 0)
                alterString += ", DROP PRIMARY KEY";
            if (pKeys.Count > 0)
            {
                alterString += ", ADD PRIMARY KEY (";
                for (int k = 0; k < pKeys.Count; k++)
                {
                    alterString += "`" + pKeys[k] + "`";
                    if (k < pKeys.Count - 1)
                        alterString += ",";
                }
                alterString += ") ";
            }

           
            alterString += ";";
            MySqlCommand cmd = new MySqlCommand(alterString, conection);
            try
            {
               
                cmd.ExecuteNonQuery();
                if (advanced)
                {
                    alterString = "alter table `"+dataBase+"`.`"+table.Text+"` " ;
                    if (engine.Text != "")
                        alterString += "Engine=" + engine.Text + " ";
                    if (checksum.Text != "")
                        alterString += "checksum=" + checksum.Text + " ";
                    if (autoincrement.Text != "")
                        alterString += "auto_increment=" + autoincrement.Text + " ";
                    if (avgRowLen.Text != "")
                        alterString += "avg_row_length=" + avgRowLen.Text + " ";
                    if (delaykeywrite.Text != "")
                        alterString += "delay_key_write=" + delaykeywrite.Text + " ";
                    if (rowFormat.Text != "")
                        alterString += "row_format=" + rowFormat.Text + " ";
                    if (raidType.Text != "")
                        alterString += "raid_type=" + raidType.Text + " ";
                    alterString += "comment=\'" + comment.Text + "\' ";
                    cmd = new MySqlCommand(alterString, conection);
                    cmd.ExecuteNonQuery();
                }
                outPut.Text = "Operation succesfull.";
            }
            catch (MySqlException ex)
            {
                outPut.Text = "Error:" + ex.Message ;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        private void Create()
        {
            this.tableName = table.Text;
            string createString = "create table `"+ dataBase +"`.`" + tableName + "` (";
            string pk = "";
            for (int i = 0; i < dg1.Rows.Count - 1; i++)
            {

                DataGridViewRow dgr = dg1.Rows[i];
                if (dgr.Cells[1].Value == null)
                {
                    MessageBox.Show("Select a column type");
                    return;
                }
                createString += " `" + dgr.Cells[0].Value.ToString() + "` "; //col name
                createString += dgr.Cells[1].Value.ToString() + " "; //tip
                if (dgr.Cells[2].Value != null)
                {
                    createString += " (" + dgr.Cells[2].Value.ToString() + ")";
                }

                createString += ((bool)dgr.Cells[9].FormattedValue == true) ? " unsigned" : " ";
                createString += ((bool)dgr.Cells[7].FormattedValue == true) ? " zerofill " : " ";
                createString += ((bool)dgr.Cells[6].FormattedValue == true) ? " not null " : " null ";

                if (dgr.Cells[3].Value != null)
                    if (dgr.Cells[3].Value.ToString() != "")
                        createString += " default " + PutDefaultValueApostrophe(dgr.Cells[3].Value.ToString(), dgr.Cells[1].Value.ToString());

                if (dgr.Cells[4].Value != null)
                    if (dgr.Cells[4].Value.ToString() != "")
                        createString += " collate " + dgr.Cells[4].Value.ToString() + " ";

                if (dgr.Cells[5].Value != null)
                    if (dgr.Cells[5].Value.ToString() != "")
                        pk += "`" + dgr.Cells[0].Value.ToString() + "`,";
                
                if (dgr.Cells[6].FormattedValue != null)
                    if ((bool)dgr.Cells[6].FormattedValue)
                        createString += " auto_increment";
               
                createString += " comment '" +dgr.Cells[10].FormattedValue.ToString() +"',";

            }
            if (pk != "")
            {
                pk = pk.Remove(pk.Length - 1, 1);
                createString += "primary key (" + pk + ") ";
            }
            createString = createString.Remove(createString.Length - 1, 1) +") ";
            if (engine.Text != "")
                createString += "Engine=" + engine.Text.ToString()+" ";
            if (checksum.Text != "")
                createString += "checksum=" + checksum.Text + " ";
            if (autoincrement.Text != "")
                createString += "auto_increment=" + autoincrement.Text + " ";
            if (avgRowLen.Text != "")
                createString += "avg_row_length=" + avgRowLen.Text + " ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(createString, conection);
                cmd.ExecuteNonQuery();
                tableCreated = true;
                outPut.Text = "Table `" + dataBase + "`.`" + tableName + "` has been created ";
            }
            catch (Exception ex)
            {
                tableCreated = false;
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
                LoadColumns();
                LoadAdvanced();
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
            if (e.Row.Tag != null)
                rowsDeleted.Add(e.Row.Tag.ToString());

        }

        private void dg1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (dg1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    return;
                switch (dg1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToLower())
                {
                    case "int":
                    case "tinyint":
                    case "double":
                    case "bit":
                    case "bool":
                    case "bigint":
                    case "float":
                    case "mediumint":
                    case "smallint":
                    case"real":
                    case "set":
                        dg1.Rows[e.RowIndex].Cells[6].ReadOnly = false;
                        dg1.Rows[e.RowIndex].Cells[8].ReadOnly = false;
                        dg1.Rows[e.RowIndex].Cells[9].ReadOnly = false;
                        break;
                    default:
                        dg1.Rows[e.RowIndex].Cells[6].Value = false;
                        dg1.Rows[e.RowIndex].Cells[8].Value = false;
                        dg1.Rows[e.RowIndex].Cells[9].Value = false;
                        dg1.Rows[e.RowIndex].Cells[6].ReadOnly = true;
                        dg1.Rows[e.RowIndex].Cells[8].ReadOnly = true;
                        dg1.Rows[e.RowIndex].Cells[9].ReadOnly = true;

                        break;
                }
            }


            if (e.ColumnIndex == 6 && e.RowIndex > -1)
            {
                bool val = (bool)dg1.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue;
                if (val)
                {
                    bool find = false;
                    for (int i = 0; i < dg1.Rows.Count && !find; i++)
                    {
                        if (i != e.RowIndex)
                        {
                            if ((bool)dg1.Rows[i].Cells[e.ColumnIndex].FormattedValue)
                                find = true;
                        }
                    }
                    if (find)
                    {
                        MessageBox.Show("There can be only one autoincrement column");
                        dg1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dg1.SelectedRows.Count > 0)
            {
                int i = dg1.SelectedRows[0].Index;
                dg1.Rows.Insert(i, 1);
            }
            else
                dg1.Rows.Add(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            advanced = !advanced;
            button4.Text = advanced ? "Simple" : "Advanced";
            SetMode(advanced);
        }

        private void SetMode(bool advanced)
        {
            this.SuspendLayout();
            groupBox1.Visible = advanced;
            if (!advanced)
            {
                dg1.Height = this.Height - button2.Height - dg1.Top - 50 - button4.Height;
                button3.Top = dg1.Top + dg1.Height + 4;
                button4.Top = button3.Top;
            }
            else
            {
                button3.Top = groupBox1.Top - button3.Height - 10;
                button4.Top = button3.Top;
                dg1.Height = button3.Top-dg1.Top-4;
            }
            this.ResumeLayout();
        }

        private void dg1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
        }


    }
}