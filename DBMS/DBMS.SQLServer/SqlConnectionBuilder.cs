using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SqlControls.DBEngine;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using SqlControls.DBEngine.SQLServer;
namespace SqlControls.DBEngine.SQL
{
    public class SqlConnectionBuilder : UserControl, IConnectionBuilder
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
        private string server;
        private OdbcConnection sqlconnection;
        private bool changed;
        public bool Changed
        {
            get
            {
                return changed;
            }
        }


        private SQLDBBrowser browser;
        private Label label4;
    
        public SQLDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        private SQLConnectionSetting connectionSetting;

        public SQLConnectionSetting ConnectionSetting
        {
            get { return connectionSetting; }
            set
            {
                connectionSetting = value;
                if (connectionSetting != null)
                {
                    serverAddress.Text = connectionSetting.Host;
                    password.Text = Krypto.DecryptPassword(connectionSetting.Password);
                    userName.Text = connectionSetting.UserName;
                    comboBox1.Text = connectionSetting.Authentication;
                }

            }
        }

        public string GetServerAddress()
        {
            return this.server;
        }

        public void ValidateConnection(string connectionName, IConnectionSetting sett)
        {
            SQLConnectionSetting connectionSetting = (SQLConnectionSetting)sett;
            connectionSetting.Host = serverAddress.Text;
            connectionSetting.Authentication = comboBox1.Text;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.UserName = userName.Text;
            connectionSetting.connectionName = connectionName;
        }

        #region IconnectionBuilder Members

        public SqlConnectionBuilder()
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

        public object GetConnection()
        {
            return sqlconnection;
        }

        public bool OpenConnection()
        {
            this.sqlconnection = new OdbcConnection(GetString());
            browser = new SQLDBBrowser(sqlconnection);
            try
            {
                sqlconnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        public bool TestConnection()
        {
            bool ret = true;
            this.sqlconnection = new OdbcConnection(GetString());
            try
            {
                sqlconnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ret = false;
            }
            finally
            {
                if (sqlconnection.State != ConnectionState.Closed)
                {
                    sqlconnection.Close();
                }
            }
            return ret;
        }


        public string GetSerializedConnection()
        {
            return null;
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
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(60, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(59, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "User name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(64, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            // 
            // serverAddress
            // 
            this.serverAddress.Enabled = false;
            this.serverAddress.Location = new System.Drawing.Point(120, 20);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(230, 20);
            this.serverAddress.TabIndex = 0;
            this.serverAddress.Validated += new System.EventHandler(this.fields_Changed);
            // 
            // userName
            // 
            this.userName.Enabled = false;
            this.userName.Location = new System.Drawing.Point(120, 80);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(230, 20);
            this.userName.TabIndex = 1;
            this.userName.Validated += new System.EventHandler(this.fields_Changed);
            // 
            // password
            // 
            this.password.Enabled = false;
            this.password.Location = new System.Drawing.Point(120, 110);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(230, 20);
            this.password.TabIndex = 2;
            this.password.Validated += new System.EventHandler(this.fields_Changed);
            // 
            // database
            // 
            this.database.Enabled = false;
            this.database.Location = new System.Drawing.Point(120, 140);
            this.database.Name = "database";
            this.database.Size = new System.Drawing.Size(230, 20);
            this.database.TabIndex = 5;
            this.database.Validated += new System.EventHandler(this.fields_Changed);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(53, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Database(s)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(42, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Authentication";
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.comboBox1.Location = new System.Drawing.Point(120, 50);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(230, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Validated += new System.EventHandler(this.fields_Changed);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(3, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Not yet implemented... coming soon...";
            // 
            // SqlConnectionBuilder
            // 
            this.Controls.Add(this.label4);
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
            this.Name = "SqlConnectionBuilder";
            this.Size = new System.Drawing.Size(400, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void fields_Changed(object sender, EventArgs e)
        {
            changed = true;
        }
    }
}
