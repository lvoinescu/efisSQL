using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;



namespace DBMS.SQLite
{
    public partial class SelectForeignKeyForm : Form
    {

        private bool canceled;
        private string selectedTable, selectedColumn;

        public string SelectedColumn
        {
            get { return selectedColumn; }
            set { selectedColumn = value; }
        }

        public string SelectedTable
        {
            get { return selectedTable; }
            set { selectedTable = value; }
        }
        private SQLiteConnection connection;
        public bool Canceled
        {
            get { return canceled; }
            set { canceled = value; }
        }

        public SelectForeignKeyForm(SQLiteConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
        }

        private void SelectForeignKeyForm_Load(object sender, EventArgs e)
        {
            canceled = true;
            using(SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type = \"table\"",connection))
            {
               using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
               {
                   listBox1.Items.Clear();
                   DataTable dt = new DataTable();
                   da.Fill(dt);
                   for(int i=0;i<dt.Rows.Count;i++)
                   {
                       listBox1.Items.Add(dt.Rows[i]["name"].ToString());
                   }
                   dt.Dispose();
               }
            }
            selectedTable = listBox1.SelectedItem.ToString();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using(SQLiteCommand cmd = new SQLiteCommand("pragma table_info(`"+listBox1.SelectedItem.ToString()+"`);",connection))
            {
               using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
               {
                   listBox2.Items.Clear();
                   DataTable dt = new DataTable();
                   da.Fill(dt);
                   for(int i=0;i<dt.Rows.Count;i++)
                   {
                       listBox2.Items.Add(dt.Rows[i]["name"].ToString());
                   }
                   dt.Dispose();
               }
            }
            SelectedTable = listBox1.SelectedItem.ToString();
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            canceled = false;
            selectedColumn = listBox2.SelectedItem.ToString();
            this.Close();
        }

    }
}