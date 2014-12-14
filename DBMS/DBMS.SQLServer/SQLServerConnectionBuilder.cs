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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Security;
using System.IO;
using DBMS.core;

namespace DBMS.SQLServer
{
    public class SQLServerconnectionBuilder : UserControl, IConnectionBuilder
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox serverAddress;
        private TextBox userName;
        private TextBox password;
        private TextBox database;
        private Label label6;
        private SqlConnection conection;
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
        private SQLServerBrowser browser;

        public SQLServerBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        private SQLServerConnectionSetting connectionSetting;

        public SQLServerConnectionSetting ConnectionSetting
        {
            get { return connectionSetting; }
            set 
            {
                connectionSetting = value;
                if (connectionSetting != null)
                {
                    wait4change = true;
                    serverAddress.Text = connectionSetting.Host;
                    password.Text =Krypto.DecryptPassword(connectionSetting.Password);
                    userName.Text = connectionSetting.UserName;
                    database.Text = connectionSetting.Databases;
                    wait4change = false;
                }
                this.ResumeLayout();
            }
        }



        #region IconnectionBuilder Members

        public SQLServerconnectionBuilder( )
        {
            InitializeComponent();
            browser = new SQLServerBrowser(conection);
        }


        public IDBBrowser CreateBrowser()
        {
            SQLServerBrowser br = new SQLServerBrowser(conection);
            br.ConnectionString = this.GetString();
            return br;
        }

        public void ValidateConnection(string connectionName , IConnectionSetting sett)
        {
            SQLServerConnectionSetting connectionSetting = (SQLServerConnectionSetting)sett;
            connectionSetting.Host = serverAddress.Text;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.UserName = userName.Text;
            connectionSetting.ConnectionName = connectionName;
            connectionSetting.Databases = database.Text;
        }


        public IConnectionSetting GetConnectionSettings(string name)
        {
            SQLServerConnectionSetting connectionSetting = new SQLServerConnectionSetting(name);
            connectionSetting.Host = serverAddress.Text;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.UserName = userName.Text;
            connectionSetting.Databases = database.Text;
            return connectionSetting;
        }


        public string GetString()
        {
            server = serverAddress.Text;
            if(!radioButton1.Checked)
                return "data source=" + serverAddress.Text + ";" + "user id=" + userName.Text + ";Password=" + password.Text + ";Initial catalog=" + database.Text + " ;";
            else
                return "data source=" + serverAddress.Text + ";Integrated Security=SSPI;Initial catalog=" + database.Text + " ;";
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
            this.conection = new SqlConnection(GetString());
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
            this.conection = new SqlConnection(GetString());
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
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.userName = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.database = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(25, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "User name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(25, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // serverAddress
            // 
            this.serverAddress.Location = new System.Drawing.Point(125, 12);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(230, 20);
            this.serverAddress.TabIndex = 0;
            this.serverAddress.Text = "localhost\\sqlexpress";
            this.serverAddress.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // userName
            // 
            this.userName.Enabled = false;
            this.userName.Location = new System.Drawing.Point(147, 100);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(230, 20);
            this.userName.TabIndex = 1;
            this.userName.Text = "sa";
            this.userName.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // password
            // 
            this.password.Enabled = false;
            this.password.Location = new System.Drawing.Point(147, 130);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(230, 20);
            this.password.TabIndex = 2;
            this.password.Text = "123456";
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // database
            // 
            this.database.Location = new System.Drawing.Point(147, 176);
            this.database.Name = "database";
            this.database.Size = new System.Drawing.Size(230, 20);
            this.database.TabIndex = 5;
            this.database.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(25, 179);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Initial catalog";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(53, 48);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(190, 24);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Use Windows authentication";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(53, 70);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(190, 24);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.Text = "Use SQL Server authentication";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // SQLServerconnectionBuilder
            // 
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.database);
            this.Controls.Add(this.password);
            this.Name = "SQLServerconnectionBuilder";
            this.Size = new System.Drawing.Size(431, 209);
            this.Load += new System.EventHandler(this.SqlconnectionBuilder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;

        private void fields_Changed(object sender, EventArgs e)
        {
            if (!wait4change) 
                changed = true;
        }

        private void SqlconnectionBuilder_Load(object sender, EventArgs e)
        {
            //LanguageManager.LanguageSwitcher.Instance().SwitchLanguage(this.GetType().Namespace,this.GetType().Name,this);
        }

        private void charSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!wait4change)
                changed = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                userName.Enabled = false;
                password.Enabled = false;
            }
            else
            {
                userName.Enabled = true;
                password.Enabled = true;
            }
        }

    }





}
