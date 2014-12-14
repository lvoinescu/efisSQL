using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SqlControls.DBEngine;
using System.Data;
using System.Data.Odbc;
namespace SqlControls.DBEngine.SQL
{
    public class SqlconnectionBuilder : UserControl, IconnectionBuilder
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox serverAddress;
        private TextBox userName;
        private TextBox password;
        private TextBox database;
        private Label label7;
        private ComboBox comboBox1;
        private Label label6;
      
        private SQLDBBrowser browser;
        public SQLDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }


        #region IconnectionBuilder Members

        public SqlconnectionBuilder()
        {
            InitializeComponent();
        }

        
        public string GetString()
        {
            return "Driver={SQL Server};Server=" + serverAddress.Text + ";" + "Database=" + database.Text + ";user=" + userName.Text + ";password=" + password.Text + ";";
        }

        public string GetDBEngine()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDBBrowser GetBrowser()
        {
            return (IDBBrowser)browser;
        }

        public OdbcConnection GetConnection()
        {
            OdbcConnection con = new OdbcConnection(GetString());
            browser = new SQLDBBrowser(con);
            return con;
        }

        #endregion

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.userName = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.database = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "User name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            // 
            // serverAddress
            // 
            this.serverAddress.Location = new System.Drawing.Point(114, 27);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(224, 20);
            this.serverAddress.TabIndex = 0;
            this.serverAddress.Text = "localhost";
            // 
            // userName
            // 
            this.userName.Location = new System.Drawing.Point(114, 100);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(223, 20);
            this.userName.TabIndex = 1;
            this.userName.Text = "root";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(114, 136);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(223, 20);
            this.password.TabIndex = 2;
            this.password.Text = "123456";
            // 
            // database
            // 
            this.database.Location = new System.Drawing.Point(114, 172);
            this.database.Name = "database";
            this.database.Size = new System.Drawing.Size(223, 20);
            this.database.TabIndex = 5;
            this.database.Text = "monitorizare";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 175);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Database(s)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(35, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Authentication";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.comboBox1.Location = new System.Drawing.Point(114, 63);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(224, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Text = "Windows Authentication";
            // 
            // SqlconnectionBuilder
            // 
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.database);
            this.Controls.Add(this.password);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.serverAddress);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SqlconnectionBuilder";
            this.Size = new System.Drawing.Size(405, 226);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
