/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 2/13/2012
 * Time: 10:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace DBMS.SQLite
{
	/// <summary>
	/// Description of AttachDbForm.
	/// </summary>
	public partial class AttachDbForm : Form
	{
		private bool canceled;
		private string alias, path;
		
		public string Path {
			get { return path; }
		}
		
		public string Alias {
			get { return alias; }
		}
		public bool Canceled {
			get { return canceled; }
			set { canceled = value; }
		}

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

		public AttachDbForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			alias = "";
			path = "";
			canceled = true;
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button3Click(object sender, EventArgs e)
		{
            canceled = true;
            this.Close();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			canceled = false;
			alias = aliasText.Text;
			path = pathText.Text;
            password = passText.Text;
			this.Close();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			OpenFileDialog opdfd = new OpenFileDialog();
			opdfd.Filter = ("Sqlite file|*.db3|All files|*.*");
			opdfd.ShowDialog();
			if(opdfd.FileName!="")
				pathText.Text= opdfd.FileName;
		}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.password = passText.Text;
        }
	}
}
