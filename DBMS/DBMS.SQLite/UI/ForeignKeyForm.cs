using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;

namespace DBMS.SQLite
{
    public partial class ForeignKeyForm : Form
    {
        private SQLiteConnection connection;
        private string dataBase,tableName;

        public ForeignKeyForm(SQLiteConnection connection, string dataBase, string tableName)
        {
            this.connection = connection;
            this.dataBase = dataBase;
            this.tableName = tableName;
            InitializeComponent();
        }

        private void ForeignKeyForm_Load(object sender, EventArgs e)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("pragma foreign_key_list (`" +tableName +"`);" ,connection))
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                listView1.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                        listView1.Items.Add(new ListViewItem(new string[] { dt.Rows[i]["to"].ToString(), dt.Rows[i]["table"].ToString(), dt.Rows[i]["from"].ToString() }));
                }
                dt.Dispose();
                da.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
