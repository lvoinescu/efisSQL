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
    public partial class ReorderColumnForm : Form
    {

        MySqlCommand cmd = new MySqlCommand();
        DataTable tableColumns;
        MySqlConnection connection;
        string dataBase, tableName;
        public ReorderColumnForm()
        {
            InitializeComponent();
        }

        public ReorderColumnForm(MySqlConnection connection, string dataBase, string tableName)
        {
            this.dataBase = dataBase;
            this.tableName = tableName;
            this.connection = connection;
            InitializeComponent();
            RefreshColumns();
        }

        private void RefreshColumns()
        {
            tableColumns = new DataTable();
            cmd = new MySqlCommand("show full fields from `" + dataBase + "`.`" + tableName + "`;", connection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(tableColumns);
            listView1.Items.Clear();
            for (int i = 0; i < tableColumns.Rows.Count; i++)
                listView1.Items.Add(new ListViewItem(tableColumns.Rows[i]["Field"].ToString()));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
                return;
            ListViewItem it = listView1.SelectedItems[0];
            int i = it.Index;
            if (i < 1)
                return;
            listView1.Items.RemoveAt(i);
            listView1.Items.Insert(i-1 , it);
            ChangeTableRows(i, i -1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
                return;
            ListViewItem it = listView1.SelectedItems[0];
            int i = it.Index;
            if (i > listView1.Items.Count-2)
                return;
            listView1.Items.RemoveAt(i);
            listView1.Items.Insert(i+1 , it);
            ChangeTableRows(i, i + 1);
        }

        private void ChangeTableRows(int i, int j)
        {
            for (int k = 0; k < tableColumns.Columns.Count; k++)
            {
                Object aux = tableColumns.Rows[i][k];
                tableColumns.Rows[i][k] = tableColumns.Rows[j][k];
                tableColumns.Rows[j][k] = aux;
            }
        }

        private void ReorderColumnForm_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            RefreshColumns();
        }

        //private void listView1_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.StringFormat))
        //        e.Effect = DragDropEffects.Move;
        //    else
        //        e.Effect = DragDropEffects.None;

        //}

        //private void listView1_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (listView1.SelectedIndices.Count < 1)
        //        return;
        //    int index = -1;
        //    int curentIndex = listView1.SelectedIndices[0];
        //    Point p = PointToClient(new Point(e.X - listView1.Left , e.Y - listView1.Top));
        //    ListViewItem item = listView1.GetItemAt(p.X, p.Y);
        //    if (item != null)
        //        index = item.Index;
        //    if (index == -1)
        //        index = listView1.Items.Count;
        //    object data = e.Data.GetData(typeof(string));
        //    ListViewItem it = listView1.FindItemWithText(data.ToString());
        //    if (index > listView1.Items.Count)
        //        index = listView1.Items.Count;
        //    string aux = it.Text;
        //    it.Text = this.listView1.Items[index].Text;
        //    this.listView1.Items[index].Text = aux;
        //    ChangeTableRows(index, curentIndex);
        //    //for(int i= Math.Max( curentIndex,index)-1;i>Math.Min( curentIndex,index);i--)
        //    //ChangeTableRows(i, i-1);

        //}

        //private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        //{
        //    DoDragDrop(((ListViewItem)e.Item).Text, DragDropEffects.Move);
        //}


        private string PutGhilimele(object value, string Tip)
        {
            if (value == DBNull.Value)
                return "NULL";
            string tip = Tip.ToLower();

            if (tip == "datetime" || (tip == "timestamp"))
                return value.ToString();
            
            if(tip.Contains("text")||tip.Contains("varchar"))
            {
                    string ret = value.ToString().Replace("'", "\\'");
                    ret = ret.ToString().Replace("\"", "\\\"");
                    return "'" + ret +"'";
            }
             return "'" + value.ToString() + "'";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string s = "alter table `" + dataBase + "`.`" + tableName + "` ";
                s += "change `" + tableColumns.Rows[0]["Field"].ToString() + "` `" + tableColumns.Rows[0]["Field"].ToString() + "` ";
                s += tableColumns.Rows[0]["Type"].ToString() + " ";
                s += tableColumns.Rows[0]["Null"].ToString().ToUpper() == "YES" ? "NULL " : "NOT NULL ";
                s += tableColumns.Rows[0]["Collation"] != DBNull.Value ? " collate " + tableColumns.Rows[0]["Collation"].ToString() +" " : " ";
                s += tableColumns.Rows[0]["Default"] != DBNull.Value ? " default " + PutGhilimele(tableColumns.Rows[0]["Default"].ToString(), tableColumns.Rows[0]["Type"].ToString()) : " ";
                s += " comment '" + tableColumns.Rows[0]["comment"].ToString() + "'";
                s += " first";
                for (int i = 1; i < tableColumns.Rows.Count; i++)
                {
                    s += ", change `" + tableColumns.Rows[i]["Field"].ToString() + "` `" + tableColumns.Rows[i]["Field"].ToString() + "` ";
                    s += tableColumns.Rows[i]["Type"].ToString() + " ";
                    s += tableColumns.Rows[i]["Null"].ToString().ToUpper() == "YES" ? "NULL " : "NOT NULL ";
                    s += tableColumns.Rows[i]["Collation"] != DBNull.Value ? " collate " + tableColumns.Rows[i]["Collation"].ToString() + " " : " ";
                    s += tableColumns.Rows[i]["Default"] != DBNull.Value ? " default " + PutGhilimele(tableColumns.Rows[i]["Default"].ToString(), tableColumns.Rows[i]["Type"].ToString()): " ";
                    s += " comment '" + tableColumns.Rows[i]["comment"].ToString() + "'";
                    s += " after `" + tableColumns.Rows[i - 1]["Field"].ToString() +"`";
                }
                cmd = new MySqlCommand(s, connection);
                cmd.ExecuteNonQuery();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}