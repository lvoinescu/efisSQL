
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DBMS.SQLite
{
	/// <summary>
	/// Description of ImportFromCSV.
	/// </summary>
	public partial class ImportFromCSV : Form
	{
		private SQLiteConnection connection;
		private string fileName;
		private string dataBase, table;
		public ImportFromCSV(SQLiteConnection connection, string dataBase, string table)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.dataBase = dataBase;
			this.table = table;
			this.connection = connection;
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button1Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathText.Text))
            {
                MessageBox.Show("Please select a valid file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string file = Path.GetFileName(pathText.Text);
            string[] num = textBox1.Text.Split(new char[] { ';', ',' });
            int[] ind = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                int.TryParse(num[i], out ind[i]);
            }
            if (radioButton3.Checked && num.Length != listView1.CheckedItems.Count)
            {
                MessageBox.Show("Selected column count in listview must be \n the same with numbers of filled indices in \"Selected columns\" input field ");
                return;
            }
            string dir = Path.GetDirectoryName(pathText.Text);
            OleDbConnection cn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + dir + "\";Extended Properties=\"text;HDR=" + (checkBox1.Checked ? "Yes" : "No") + ";FMT=Delimited\"");
            cn.Open();
            OleDbCommand cmd = new OleDbCommand("select * from [" + file + "]", cn);
            OleDbDataReader reader = cmd.ExecuteReader();

            bool allCollumns = false;
            allCollumns = radioButton1.Checked;


            string sql = "insert into `" + dataBase + "`.`" + table + "` (";
            string sqlp ="";
            for(int i=0;i<listView1.CheckedItems.Count;i++)
            {
                sql +="`" + listView1.CheckedItems[i].Text +"`";
                sqlp += "@p" + i.ToString();
                if (i < listView1.CheckedItems.Count - 1)
                {
                    sql += ", ";
                    sqlp += ", ";
                }
            }
            sql += ") values (" + sqlp + ");";

        

            int row= 0;
            SQLiteTransaction transaction = connection.BeginTransaction();
            SQLiteCommand cmdi = new SQLiteCommand(sql, connection);
            cmdi.Transaction = transaction;
            int nrMin = (int) Math.Round(numericUpDown1.Value);
            try
            {
                while (reader.Read())
                {
                    if (allCollumns)
                    {
                        for (int i = 0; i < Math.Min(reader.FieldCount, listView1.CheckedItems.Count); i++)
                        {
                            cmdi.Parameters.AddWithValue("@p" + i.ToString(), reader.GetValue(i));
                        }
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                    }
                    else
                        if (radioButton2.Checked)
                        {
                            for (int i = 0; i < Math.Min(reader.FieldCount, listView1.CheckedItems.Count); i++)
                            {
                                cmdi.Parameters.AddWithValue("@p" + i.ToString(), reader.GetValue(i + nrMin));
                            }
                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                        }
                        else
                            if (radioButton3.Checked)
                            {
                                for (int i = 0; i < listView1.CheckedItems.Count; i++)
                                {
                                    cmdi.Parameters.AddWithValue("@p" + i.ToString(), reader.GetValue(ind[i]));
                                }
                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                            }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName,MessageBoxButtons.OK, MessageBoxIcon.Error);
                transaction.Rollback();
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                cn.Close();
            }

        }
		
		void Button2Click(object sender, EventArgs e)
		{
			OpenFileDialog opdfd = new OpenFileDialog();
            opdfd.Filter = ("CSV file|*.csv|Text file|*.txt|All files|*.*");
			opdfd.ShowDialog();
			if(opdfd.FileName!="")
			{
				pathText.Text= opdfd.FileName;
			}
		}

        private void ImportFromCSV_Load(object sender, EventArgs e)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("pragma `" + dataBase + "`.table_info(`" + table + "`);", connection))
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListViewItem it = new ListViewItem(dt.Rows[i]["name"].ToString());
                    it.Checked = true;
                    listView1.Items.Add(it);
                }
                da.Dispose();
            }
        }
    }
}
