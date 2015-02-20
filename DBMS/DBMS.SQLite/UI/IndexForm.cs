using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Data.SQLite;

namespace DBMS.SQLite
{
    public partial class IndexForm : Form
    {

        private class Index
        {

            public bool Changed;
            private string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public List<string> columns;
            private bool unique;

            public bool Unique
            {
                get { return unique; }
                set { unique = value; }
            }
            private bool fulltext;

            public bool Fulltext
            {
                get { return fulltext; }
                set { fulltext = value; }
            }

            private bool isNew;

            public bool IsNew
            {
                get { return isNew; }
                set { isNew = value; }
            }

            public Index()
            {
                Changed = true;
                isNew = false;
                columns = new List<string>();
            }
            public Index(string name, bool changed)
            {
                this.name = name;
                Changed = changed;
                isNew = false;
                columns = new List<string>();
            }

        }


        private bool arange;
        private string dataBase;
        private string tableName;
        private SQLiteConnection conection;
        private List<Index> indexes;
        private int downIndex;
        private int curentIndex;
        private int curentColumn;
        public IndexForm()
        {
            curentIndex = -1;
            InitializeComponent();
        }

        public IndexForm(SQLiteConnection conection, string dataBase, string tableName)
        {
            curentIndex = -1;
            this.dataBase = dataBase;
            this.tableName = tableName;
            this.conection = conection;
            InitializeComponent();
        }

        private void LoadIndexes()
        {
            curentIndex = -1;
            curentColumn = -1;
            try
            {
                listBox1.Items.Clear();
                using (DataTable dt = new DataTable())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("pragma table_info( `" + tableName + "`);", this.conection))
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        string[] rez = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                            listBox1.Items.Add(dt.Rows[i]["name"].ToString());
                    }
                }

                listView2.Items.Clear();
                indexes = new List<Index>();
                using (SQLiteCommand cmd = new SQLiteCommand("pragma index_list(`" + tableName + "`);", conection))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            ad.Fill(dt);
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Index indx = new Index(dt.Rows[i]["name"].ToString(), false);
                                indx.Unique = (dt.Rows[i]["unique"].ToString() == "1");
                                indexes.Add(indx);
                                listView2.Items.Add(new ListViewItem(dt.Rows[i]["name"].ToString()));
                            }

                            for (int j = 0; j < indexes.Count; j++)
                            {
                                SQLiteCommand cmdi = new SQLiteCommand("pragma index_info(`" + indexes[j].Name + "`);", conection);
                                DataTable dt2 = new DataTable();
                                SQLiteDataAdapter ad2 = new SQLiteDataAdapter(cmdi);
                                ad2.Fill(dt2);
                                
                                if (dt2.Rows.Count>0)
                                {
                                    for (int i = 0; i < dt2.Rows.Count; i++)
                                    {
                                        indexes[j].columns.Add(dt2.Rows[i]["name"].ToString());
                                    }
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        private void IndexForm_Load(object sender, EventArgs e)
        {
            groupBox2.Text = "'" + tableName + "'" + " columns";
            LoadIndexes();
        }


        private void listBox2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }





        private void SaveIndexes()
        {
            string s ="";
            using (SQLiteCommand cmd = new SQLiteCommand("",conection))
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    if (indexes[i].Changed)
                    {
                        try
                        {
                            if (indexes[i].Name.ToUpper().Contains("SQLITE_AUTOINDEX_"+tableName.ToUpper()))
                            {

                                s =  "drop index if exists " + indexes[i].Name +";";
                            }
                            else
                            {
                                s = "drop index if exists " + indexes[i].Name + ";";
                                s += " create ";
                                if (indexes[i].Unique)
                                    s += " unique";
                                s+="  index `" + indexes[i].Name + "` on " + tableName + " (";
                                for (int j = 0; j < indexes[i].columns.Count; j++)
                                    s += "`" + indexes[i].columns[j] + "`,";
                                s = s.Remove(s.Length - 1, 1) + ");";
                                
                            }
                            cmd.CommandText = s;
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
            }
        }
        LoadIndexes();
    }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count < 1)
                return;
            if (MessageBox.Show("Are you sure you want to drop " + listView2.Items[listView2.SelectedIndices[0]].ToString() + "?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            using (SQLiteCommand cmd = new SQLiteCommand("", conection))
            {
                int i = listView2.SelectedIndices[0];
                try
                {
                    string s = "";
                    s = "drop index " + indexes[i].Name;
                    cmd.CommandText = s;
                    cmd.ExecuteNonQuery();
                    listView2.Items.RemoveAt(i);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button5.Enabled = (listView1.SelectedIndices!=null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (curentColumn<0)
                return;
            indexes[curentIndex].columns.RemoveAt(curentColumn);
            indexes[curentIndex].Changed = true;
            listView1.Items.RemoveAt(curentColumn);
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            arange = true;
            DoDragDrop(((ListViewItem)e.Item).Text, DragDropEffects.Move);
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (listView2.SelectedIndices.Count < 1)
                return;
            int index = -1;
            int curentIndex = listView2.SelectedIndices[0];
            Point p = PointToClient(new Point(e.X-listView1.Left-groupBox1.Left, e.Y-listView1.Top- groupBox1.Top));
            ListViewItem item = listView1.GetItemAt(p.X,p.Y);
            if (item != null)
                index = item.Index;
            if (index == -1)
                index = listView1.Items.Count;
            object data = e.Data.GetData(typeof(string));
            if (!arange)
            {
                if (listView1.FindItemWithText(data.ToString())!=null)
                    return;
                this.listView1.Items.Insert(index, new ListViewItem(data.ToString()));
                indexes[curentIndex].columns.Insert(index, data.ToString());
            }
            else
            {
                ListViewItem it = listView1.FindItemWithText(data.ToString());
                this.listView1.Items.Remove(it);
                if (index > listView1.Items.Count)
                    index = listView1.Items.Count;
                this.listView1.Items.Insert(index, it);
                indexes[curentIndex].columns.Remove(data.ToString());
                indexes[curentIndex].columns.Insert(index, data.ToString());
            }
            indexes[curentIndex].Changed = true;
            listView1.BackColor = Color.White;
            arange = false;

        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            arange = false;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBox1.IndexFromPoint(e.X, e.Y)<-1)
                return;
            downIndex = listBox1.IndexFromPoint(e.X, e.Y);
            if (downIndex >= 0 && downIndex < listBox1.Items.Count)
            {
                DoDragDrop(listBox1.Items[downIndex], DragDropEffects.All);
                listBox1.AllowDrop = true;
            }
            arange = false;

        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputNewIndexForm iform = new InputNewIndexForm();
            iform.ShowDialog();
            if (!iform.Canceled)
            {
                Index ind = new Index(iform.IndexName, true);
                ind.IsNew = true;
                indexes.Add(ind);
                listView2.Items.Add(new ListViewItem(iform.IndexName));
              
            }
            iform.Dispose();

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listView2.SelectedIndices.Count < 1)
            {
                curentIndex = -1;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                return;
            }
            int i = listView2.SelectedIndices[0];
            curentIndex = i;
            if (indexes[curentIndex].Name.ToUpper() == "PRIMARY")
            {
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
            }
            listView1.Items.Clear();
            for (int j = 0; j < indexes[i].columns.Count; j++)
                listView1.Items.Add(new ListViewItem(indexes[i].columns[j], indexes[i].columns[j], null));
            checkBox1.Checked = indexes[i].Unique;
            checkBox2.Checked = indexes[i].Fulltext;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveIndexes();
        }



        private void checkBox1_Validated(object sender, EventArgs e)
        {
            if (curentIndex < 0)
                return;
            indexes[curentIndex].Unique = checkBox1.Checked;
            indexes[curentIndex].Changed = true;
            indexes[curentIndex].Fulltext = false;

        }

        private void checkBox2_Validated(object sender, EventArgs e)
        {
            if (curentIndex < 0)
                return;
            indexes[curentIndex].Fulltext = checkBox2.Checked;
            indexes[curentIndex].Changed = true;
            indexes[curentIndex].Unique = false;

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (curentIndex < 0)
                return;
            if(checkBox2.Checked)
                checkBox1.Checked = false;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (curentIndex < 0)
                return;
            if (checkBox1.Checked)
                checkBox2.Checked = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
                curentColumn = listView1.SelectedIndices[0];

        }

 


        }
}