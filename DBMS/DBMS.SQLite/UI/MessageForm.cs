/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 2/14/2012
 * Time: 9:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DBMS.SQLite
{
	/// <summary>
	/// Description of MessageForm.
	/// </summary>
	public partial class MessageForm : Form
	{
		private string message;
		public MessageForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public MessageForm(string message)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.message=message;
			this.textBox1.Text=message;
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
