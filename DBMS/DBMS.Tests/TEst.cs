/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 2/22/2012
 * Time: 8:06 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using DBMS.core;

namespace DBMS.Tests
{
	/// <summary>
	/// Description of TEst.
	/// </summary>
	public partial class TEst : Form
	{
		public TEst()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void TEstLoad(object sender, EventArgs e)
		{
            try
            {
                DataUserView dview = new DataUserView(this, null);

                this.Controls.Add(dview);
            }
            catch (Exception ex)
            {
            }
		}
	}
}
