/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using DBMS.core;

namespace DBMS.MySQL
{
    public class MySqlconnectionBuilder : UserControl, IConnectionBuilder
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox serverAddress;
        private TextBox userName;
        private TextBox password;
        private TextBox port;
        private Label label5;
        private TextBox database;
        private ComboBox charSet;
        private Label label6;
        private MySqlConnection conection;
        private string server;
        private bool changed;
        private bool wait4change;
      
        public bool Changed
        {
            set
            {
                this.changed = value;
            }
            get
            {
                return changed;
            }
        }
        private MySQLDBBrowser browser;

        public MySQLDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        private MySqlConnectionSetting connectionSetting;

        public MySqlConnectionSetting ConnectionSetting
        {
            get { return connectionSetting; }
            set 
            {
                connectionSetting = value;
                if (connectionSetting != null)
                {
                    wait4change = true;
                    charSet.Text = connectionSetting.CharSet;
                    serverAddress.Text = connectionSetting.Host;
                    port.Text = connectionSetting.Port;
                    password.Text =Krypto.DecryptPassword(connectionSetting.Password);
                    userName.Text = connectionSetting.UserName;
                    database.Text = connectionSetting.Databases;
                    wait4change = false;
                }
                this.ResumeLayout();
            }
        }



        #region IconnectionBuilder Members

        public MySqlconnectionBuilder( )
        {
            InitializeComponent();
            browser = new MySQLDBBrowser(conection);
        }


        public IDBBrowser CreateBrowser()
        {
            MySQLDBBrowser br = new MySQLDBBrowser(conection);
            br.ConnectionString = this.GetString();
            return br;
        }

        public void ValidateConnection(string connectionName , IConnectionSetting sett)
        {
            MySqlConnectionSetting connectionSetting = (MySqlConnectionSetting)sett;
            connectionSetting.CharSet = charSet.Text;
            connectionSetting.Host = serverAddress.Text;
            connectionSetting.Port = port.Text;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.UserName = userName.Text;
            connectionSetting.ConnectionName = connectionName;
            connectionSetting.Databases = database.Text;
        }


        public IConnectionSetting GetConnectionSettings(string name)
        {
            MySqlConnectionSetting connectionSetting = new MySqlConnectionSetting(name);
            connectionSetting.CharSet = charSet.Text;
            connectionSetting.Host = serverAddress.Text;
            connectionSetting.Port = port.Text;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.UserName = userName.Text;
            connectionSetting.Databases = database.Text;
            return connectionSetting;
        }


        public string GetString()
        {
            server = serverAddress.Text;
            return "datasource=" + serverAddress.Text + ";" + "Uid=" + userName.Text + ";Pwd=" + password.Text + ";Database=" + database.Text + " ;port=" + port.Text + ";Treat Blobs As UTF8 = yes;Allow User Variables=True;Allow Zero Datetime=true;Protocol=socket;Connect Timeout=120;" + (charSet.SelectedIndex != 0 ? ("Charset=" + charSet.Text) : "Charset=utf8") + ";" + "Allow Batch=true; Ignore prepare=true;";
            //return "Driver={MySQL ODBC 5.1 Driver};Server=" + serverAddress.Text + ";" + "Database=" + database.Text + ";user=" + userName.Text + ";password=" + password.Text + ";port=" + port.Text + ";Allow User Variables=True;Allow Zero Datetime=true;Protocol=socket;Connect Timeout=120;" + ((charSet.Text.ToLower() != "(default)") ? "charset=" + charSet.Text + ";" : "" + "Allow Batch=true; Ignore prepare=false; Use old syntax=false;TreatBlobsAsUTF8=true;");
        }

        public string GetServerAddress()
        {
            return this.server;
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

            return this.conection;
        }

        public bool OpenConnection()
        {
            this.conection = new MySqlConnection(GetString());
            try
            {
                conection.Open();
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
            this.conection = new MySqlConnection(GetString());
            try
            {
                conection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ret = false;
            }
            finally
            {
                if (conection.State != ConnectionState.Closed)
                {
                    conection.Close();
                }
            }
            return ret;
        }
      
        #endregion

        private void InitializeComponent()
        {
        	this.label1 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.serverAddress = new System.Windows.Forms.TextBox();
        	this.userName = new System.Windows.Forms.TextBox();
        	this.password = new System.Windows.Forms.TextBox();
        	this.port = new System.Windows.Forms.TextBox();
        	this.label5 = new System.Windows.Forms.Label();
        	this.database = new System.Windows.Forms.TextBox();
        	this.label6 = new System.Windows.Forms.Label();
        	this.charSet = new System.Windows.Forms.ComboBox();
        	this.SuspendLayout();
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(3, 15);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(117, 17);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "MySql host address";
        	this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(3, 45);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(117, 17);
        	this.label2.TabIndex = 0;
        	this.label2.Text = "User name";
        	this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label3
        	// 
        	this.label3.Location = new System.Drawing.Point(3, 75);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(117, 17);
        	this.label3.TabIndex = 0;
        	this.label3.Text = "Password";
        	this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// label4
        	// 
        	this.label4.Location = new System.Drawing.Point(3, 105);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(117, 17);
        	this.label4.TabIndex = 0;
        	this.label4.Text = "Port";
        	this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// serverAddress
        	// 
        	this.serverAddress.Location = new System.Drawing.Point(125, 12);
        	this.serverAddress.Name = "serverAddress";
        	this.serverAddress.Size = new System.Drawing.Size(230, 20);
        	this.serverAddress.TabIndex = 0;
        	this.serverAddress.TextChanged += new System.EventHandler(this.fields_Changed);
        	// 
        	// userName
        	// 
        	this.userName.Location = new System.Drawing.Point(125, 42);
        	this.userName.Name = "userName";
        	this.userName.Size = new System.Drawing.Size(230, 20);
        	this.userName.TabIndex = 1;
        	this.userName.TextChanged += new System.EventHandler(this.fields_Changed);
        	// 
        	// password
        	// 
        	this.password.Location = new System.Drawing.Point(125, 72);
        	this.password.Name = "password";
        	this.password.Size = new System.Drawing.Size(230, 20);
        	this.password.TabIndex = 2;
        	this.password.UseSystemPasswordChar = true;
        	this.password.TextChanged += new System.EventHandler(this.fields_Changed);
        	// 
        	// port
        	// 
        	this.port.Location = new System.Drawing.Point(125, 102);
        	this.port.Name = "port";
        	this.port.Size = new System.Drawing.Size(68, 20);
        	this.port.TabIndex = 3;
        	this.port.TextChanged += new System.EventHandler(this.fields_Changed);
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Cursor = System.Windows.Forms.Cursors.IBeam;
        	this.label5.Location = new System.Drawing.Point(196, 105);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(79, 13);
        	this.label5.TabIndex = 0;
        	this.label5.Text = "Default charset";
        	// 
        	// database
        	// 
        	this.database.Location = new System.Drawing.Point(125, 132);
        	this.database.Name = "database";
        	this.database.Size = new System.Drawing.Size(230, 20);
        	this.database.TabIndex = 5;
        	this.database.TextChanged += new System.EventHandler(this.fields_Changed);
        	// 
        	// label6
        	// 
        	this.label6.Location = new System.Drawing.Point(3, 135);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(117, 17);
        	this.label6.TabIndex = 0;
        	this.label6.Text = "Database(s)";
        	this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	// 
        	// charSet
        	// 
        	this.charSet.Cursor = System.Windows.Forms.Cursors.IBeam;
        	this.charSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.charSet.FormattingEnabled = true;
        	this.charSet.Items.AddRange(new object[] {
        	        	        	"(default)",
        	        	        	"armscii8",
        	        	        	"ascii",
        	        	        	"big5",
        	        	        	"binary",
        	        	        	"cp1250",
        	        	        	"cp1251",
        	        	        	"cp1256",
        	        	        	"cp1257",
        	        	        	"cp850",
        	        	        	"cp852",
        	        	        	"cp866",
        	        	        	"cp932",
        	        	        	"dec8",
        	        	        	"eucimps",
        	        	        	"euckr",
        	        	        	"gb2312",
        	        	        	"gbk",
        	        	        	"geostd8",
        	        	        	"greek",
        	        	        	"hebrew",
        	        	        	"hp8",
        	        	        	"keybcs2",
        	        	        	"koi8r",
        	        	        	"koi8u",
        	        	        	"latin1",
        	        	        	"latin2",
        	        	        	"latin5",
        	        	        	"latin7",
        	        	        	"macce",
        	        	        	"macroma",
        	        	        	"sjis",
        	        	        	"swe7",
        	        	        	"tis620",
        	        	        	"ucs2",
        	        	        	"ujis",
        	        	        	"utf8"});
        	this.charSet.Location = new System.Drawing.Point(281, 102);
        	this.charSet.Name = "charSet";
        	this.charSet.Size = new System.Drawing.Size(74, 21);
        	this.charSet.TabIndex = 6;
        	this.charSet.SelectedIndexChanged += new System.EventHandler(this.charSet_SelectedIndexChanged);
        	// 
        	// MySqlconnectionBuilder
        	// 
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.charSet);
        	this.Controls.Add(this.serverAddress);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.label5);
        	this.Controls.Add(this.port);
        	this.Controls.Add(this.userName);
        	this.Controls.Add(this.label3);
        	this.Controls.Add(this.label6);
        	this.Controls.Add(this.database);
        	this.Controls.Add(this.password);
        	this.Controls.Add(this.label4);
        	this.Name = "MySqlconnectionBuilder";
        	this.Size = new System.Drawing.Size(388, 184);
        	this.Load += new System.EventHandler(this.MySqlconnectionBuilder_Load);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        private void fields_Changed(object sender, EventArgs e)
        {
            if (!wait4change) 
                changed = true;
        }

        private void MySqlconnectionBuilder_Load(object sender, EventArgs e)
        {
            //LanguageManager.LanguageSwitcher.Instance().SwitchLanguage(this.GetType().Namespace,this.GetType().Name,this);
        }

        private void charSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!wait4change)
                changed = true;
        }

    }





}
