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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using DBMS.core;
using DBMS.MySQL;
using DBMS.SQLite;
using DBMS.SQLServer;

namespace efisSQL
{

    public partial class SelectDBEngineForm : Form
    {


        private ConfigurationManager managerConnection ;
        private bool isNewConnection;
        private IConnectionSetting newConnection;

        public SelectDBEngineForm()
        {
            InitializeComponent();
            isNewConnection = false;
            managerConnection = new ConfigurationManager();
        }

        private bool TextArea_DoProcessDialogKey(Keys data)
        {
        	if(data== (Keys.Control|Keys.F))
        	   {
        	   	
        	   }
        	return false;
        }
        
        private void SelectDBEngineForm_Load(object sender, EventArgs e)
        {
        	StreamReader reader = null;
            try
            {
                reader = new StreamReader(Application.UserAppDataPath + "\\config.xml");
            }
            catch (FileNotFoundException ex)
            {
            	
                return;
            }
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationManager), new Type[] { typeof(MySqlConnectionSetting), typeof(SQLiteConnectionSetting), typeof(SQLServerConnectionSetting) });
                managerConnection = (ConfigurationManager)xmlSerializer.Deserialize(reader);
                reader.Close();
                reader.Dispose();
                if (managerConnection != null)
                {
                    int nr = 0;
                    foreach (object x in managerConnection.Connections)
                    {
                        ConnectionSetting x1 = x as ConnectionSetting;
                        x1.Id = nr;
                        nr++;
                        comboBox1.Items.Add(x1.ConnectionName);
                    }
                }
                if(comboBox1.Items.Count>0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading config.xml" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                reader.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
        	connectButton.Enabled = false;
            try
            {
                object conBobj = dmbsTabControl.SelectedTab.Controls[0];
                IConnectionBuilder conB = (IConnectionBuilder)conBobj;
                if (conB.Changed || isNewConnection)
                    if (MessageBox.Show("Save changes to current connection?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        conB.ValidateConnection(comboBox1.Text, newConnection);
                        if(isNewConnection)
                            managerConnection.Connections.Add(conB.GetConnectionSettings(comboBox1.Text));
                        if(index>=0)
                        {
                        	IConnectionSetting currentSettings = (IConnectionSetting)managerConnection.Connections[index];
                       		managerConnection.Connections.RemoveAt(index);
                       		managerConnection.Connections.Insert(0, currentSettings);
                        }
                        conB.Changed = false;
                        isNewConnection = false;
                        SaveConnections();
                    }
                    else
                        conB.ValidateConnection(comboBox1.Text, newConnection);
                if (!conB.OpenConnection())
                {
                    connectButton.Enabled = true;
                    return;
                }
                      
               if(index>=0)
				{
					IConnectionSetting currentSettings = (IConnectionSetting)managerConnection.Connections[index];
					managerConnection.Connections.RemoveAt(index);
					managerConnection.Connections.Insert(0, currentSettings);
				}

                IDBBrowser browser = (IDBBrowser)conB.CreateBrowser();
                browser.ServerAddress = conB.GetServerAddress();
                browser.Connection = conB.GetConnection();

                if (!radioButton1.Checked)
                {
                    DBManagerForm dform = new DBManagerForm();
                    MainMDIManagerForm mainForm = (MainMDIManagerForm)this.Owner;
                    dform.MdiParent = mainForm;
                    dform.SuspendLayout();
                    dform.BrowserChanged += new BrowserChangedDelegate(mainForm.ChildBrowserChanged);
                    dform.AddNewBrowser(browser);
                    dform.Show();
                    dform.ResumeLayout();
                    dform.WindowState = ((MainMDIManagerForm)this.Owner).ActiveMdiChild.WindowState;
                }
                else
                {

                    Form[] f = this.Owner.MdiChildren;
                    for (int i = 0; i < f.Length; i++)
                        if (f[i] is DBManagerForm)
                        {
                            (f[i] as DBManagerForm).AddNewBrowser(browser);
                            connectButton.Enabled = true;
                            return;
                        }
                    DBManagerForm dform = new DBManagerForm();
                    MainMDIManagerForm mainForm = (MainMDIManagerForm)this.Owner;
                    dform.BrowserChanged += new BrowserChangedDelegate(mainForm.ChildBrowserChanged);
                    if (((MainMDIManagerForm)this.Owner).ActiveMdiChild != null)
                        dform.WindowState = ((MainMDIManagerForm)this.Owner).ActiveMdiChild.WindowState;
                    else
                        dform.WindowState = FormWindowState.Maximized;
                    dform.SuspendLayout();
                    dform.AddNewBrowser(browser);
                    dform.MdiParent = (Form)this.Owner;
                    dform.Show();
                    dform.ResumeLayout();
                    mainForm.ResolveIconBug(dform);

                }
                connectButton.Enabled = true;
                this.Close();
                this.Dispose();
            }
            catch (Exception ex)
            {
                connectButton.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadConnection(comboBox1.SelectedIndex);
        }

        private void LoadConnection(int p)
        {
            for (int i = 0; i < managerConnection.Connections.Count; i++)
            {
                ConnectionSetting set = managerConnection.Connections[i] as ConnectionSetting;
                if (comboBox1.Text == set.ConnectionName && p == set.Id)
                {
                    newConnection = (IConnectionSetting)managerConnection.Connections[i];
                    String type = managerConnection.Connections[i].GetType().ToString();
                    switch (type)
                    {
                        case "DBMS.MySQL.MySqlConnectionSetting":
                            dmbsTabControl.SelectedIndex = 0;
                            object x = dmbsTabControl.SelectedTab.Controls[0];
                            MySqlconnectionBuilder conB = (MySqlconnectionBuilder)x;
                            conB.Changed = false;
                            conB.ConnectionSetting = (MySqlConnectionSetting)managerConnection.Connections[i];
                            break;
                        case "DBMS.SQLite.SQLiteConnectionSetting":
                            dmbsTabControl.SelectedIndex = 1;
                            object x1 = dmbsTabControl.SelectedTab.Controls[0];
                            SQLiteconnectionBuilder conB1 = (SQLiteconnectionBuilder)x1;
                            conB1.ConnectionSetting = (SQLiteConnectionSetting)managerConnection.Connections[i];
                            break;
                        case "DBMS.SQLServer.SQLServerConnectionSetting":
                            dmbsTabControl.SelectedIndex = 2;
                            object x2 = dmbsTabControl.SelectedTab.Controls[0];
                            SQLServerconnectionBuilder conB2 = (SQLServerconnectionBuilder)x2;
                            conB2.ConnectionSetting = (SQLServerConnectionSetting)managerConnection.Connections[i];
                            break;
                    }
                    break;
                }
            }
            
           //comboBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
                return;
            if (MessageBox.Show("Are you sure you want to delete the connection?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;
            for (int i = 0; i < managerConnection.Connections.Count; i++)
            {
                ConnectionSetting set = managerConnection.Connections[i] as ConnectionSetting;
                if (comboBox1.Text == set.ConnectionName&& comboBox1.SelectedIndex==set.Id)
                {
                    managerConnection.Connections.RemoveAt(i);
                    comboBox1.Items.Remove(comboBox1.Text);
                    if(comboBox1.Items.Count>0)
                        comboBox1.SelectedIndex = 0;
                }
            }
            for (int j = 0; j < managerConnection.Connections.Count; j++)
                ((managerConnection.Connections[j]) as ConnectionSetting).Id = j;
            StreamWriter writer = new StreamWriter(Application.UserAppDataPath + "\\config.xml");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationManager), new Type[] {typeof(MySqlConnectionSetting), typeof(SQLiteConnectionSetting) });
            xmlSerializer.Serialize(writer, managerConnection);
            writer.Close();
            writer.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isNewConnection = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            object conBobj = dmbsTabControl.SelectedTab.Controls[0];
            switch (dmbsTabControl.SelectedIndex)
            {
                case 0:
                    comboBox1.Text = "New MySql connection";
                    newConnection = new MySqlConnectionSetting();
                    break;
                case 1:
                    comboBox1.Text = "New SQLite connection"; 
                    newConnection = new SQLiteConnectionSetting();
                    break;
                   case 2:
                    comboBox1.Text="New SQL Server connection";
                    newConnection = new SQLServerConnectionSetting();
                    break;
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            int id = comboBox1.Items.IndexOf(comboBox1.Text);
            object conBobj = dmbsTabControl.SelectedTab.Controls[0];
            IConnectionBuilder conBuild =(IConnectionBuilder)conBobj ;
            if (isNewConnection)
            {
                switch (dmbsTabControl.SelectedIndex)
                {
                    case 0:
                        conBuild = (MySqlconnectionBuilder)conBobj;
                        newConnection = new MySqlConnectionSetting(comboBox1.Text);
                        newConnection.Id = comboBox1.Items.Count;
                        managerConnection.Connections.Add(newConnection);
                        conBuild.ValidateConnection(comboBox1.Text, (MySqlConnectionSetting)newConnection);
                        break;
                    case 1:
                        conBuild = (SQLiteconnectionBuilder)conBobj;
                        newConnection = new SQLiteConnectionSetting(comboBox1.Text);
                        newConnection.Id = comboBox1.Items.Count;
                        managerConnection.Connections.Add(newConnection);
                        conBuild.ValidateConnection(comboBox1.Text, (SQLiteConnectionSetting)newConnection);
                        break;
                    case 2:
                        conBuild = (SQLServerconnectionBuilder)conBobj;
                        newConnection = new SQLServerConnectionSetting(comboBox1.Text);
                        newConnection.Id = comboBox1.Items.Count;
                        managerConnection.Connections.Add(newConnection);
                        conBuild.ValidateConnection(comboBox1.Text, (SQLServerConnectionSetting)newConnection);
                        break;
                }
                comboBox1.Items.Add(comboBox1.Text);
                SaveConnections();
            }
            else
                conBuild.ValidateConnection(comboBox1.Text, (IConnectionSetting)newConnection);
            isNewConnection = false;
            conBuild.Changed = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        private void dmbsTabControl_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void SaveConnections()
        {
            StreamWriter writer = new StreamWriter(Application.UserAppDataPath + "\\config.xml", false, Encoding.UTF8);
            writer.Flush();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationManager), new Type[] { typeof(MySqlConnectionSetting), typeof(SQLiteConnectionSetting), typeof(SQLServerConnectionSetting) });
            Stream s = new MemoryStream();
            xmlSerializer.Serialize(s, managerConnection);
            byte []x = new byte[100000];
            s.Seek(0, SeekOrigin.Begin);
            s.Read(x, 0,(int) s.Length);
            string aa = Encoding.Default.GetString(x);
            //xmlSerializer.Serialize(writer, managerConnection);
            writer.Write(aa);
            writer.Close();
            writer.Dispose();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            object conBobj = dmbsTabControl.SelectedTab.Controls[0];
            IConnectionBuilder conB = (IConnectionBuilder)conBobj;
            if (conB.TestConnection())
            {
                MessageBox.Show("Connection succesfull", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Cannot connect to specified address", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }



    public class ConnectedEventsArgs
    {
        public string connectString;
        public ConnectedEventsArgs(string s)
        {
            this.connectString = s;
        }
    }

}