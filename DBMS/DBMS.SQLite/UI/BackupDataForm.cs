/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 2/19/2012
 * Time: 10:49 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;


namespace DBMS.SQLite
{
	/// <summary>
	/// Description of BackupDataForm.
	/// </summary>
	public partial class BackUpDataForm : Form
	{
        private SQLiteConnection connection;
        private SQLiteDBBrowser browser;
		public BackUpDataForm(SQLiteConnection connection, SQLiteDBBrowser browser)
		{
            this.connection = connection;
            this.browser = browser;
            //
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Close();
                File.Copy(browser.FilePath, textBox1.Text);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Open();
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.db3|db3 file|all files|*.*";
            sfd.DefaultExt = "db3";
            sfd.AddExtension = true;
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                textBox1.Text = sfd.FileName;
            }
            
        }
	}
}
