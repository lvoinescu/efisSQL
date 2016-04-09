using System;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Data.SQLite;
using DBMS.core;

namespace DBMS.SQLite
{
    public class SQLiteconnectionBuilder : UserControl, IConnectionBuilder
    {
        private Label label1;
        private Label label3;
        private TextBox fileNameText;
        private TextBox password;
        private SQLiteConnection conection;
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
        private SQLiteDBBrowser browser;
        private CheckBox useUTF16;
        private CheckBox usePre33Format;
        private CheckBox readOnly;
        private CheckBox newCheck;
        private Button button1;

        public SQLiteDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        private SQLiteConnectionSetting connectionSetting;

        public SQLiteConnectionSetting ConnectionSetting
        {
            get { return connectionSetting; }
            set 
            {
                connectionSetting = value;
                if (connectionSetting != null)
                {
                    wait4change = true;
                    usePre33Format.Checked = connectionSetting.UsePre33xFormat;
                    readOnly.Checked = connectionSetting.ReadOnly;
                    fileNameText.Text = connectionSetting.DataSource;
                    password.Text =Krypto.DecryptPassword(connectionSetting.Password);
                    useUTF16.Checked = connectionSetting.UseUTF16;
                    //newCheck.Checked = connectionSetting.IsNew;
                    wait4change = false;
                }
                this.ResumeLayout();
            }
        }



        #region IconnectionBuilder Members

        public SQLiteconnectionBuilder( )
        {
            InitializeComponent();
            browser = new SQLiteDBBrowser(conection);
        }


        public void ValidateConnection(string connectionName , IConnectionSetting sett)
        {
            if (!File.Exists(fileNameText.Text) && !newCheck.Checked)
            {
                MessageBox.Show("Error opening file",Application.ProductName,MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            SQLiteConnectionSetting connectionSetting = (SQLiteConnectionSetting)sett;
            connectionSetting.DataSource = fileNameText.Text;
            connectionSetting.UsePre33xFormat = usePre33Format.Checked;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.ReadOnly = readOnly.Checked;
            connectionSetting.ConnectionName = connectionName;
            connectionSetting.UseUTF16 = useUTF16.Checked;
            connectionSetting.IsNew = newCheck.Checked;
        }

        public IConnectionSetting GetConnectionSettings(string name)
        {
            SQLiteConnectionSetting connectionSetting = new SQLiteConnectionSetting(name);
            connectionSetting.DataSource = fileNameText.Text;
            connectionSetting.UsePre33xFormat = usePre33Format.Checked;
            connectionSetting.Password = Krypto.EncryptPassword(password.Text);
            connectionSetting.ReadOnly = readOnly.Checked;
            connectionSetting.UseUTF16 = useUTF16.Checked;
            connectionSetting.IsNew = newCheck.Checked;
            return connectionSetting;
        }

        public string GetString()
        {
            server = fileNameText.Text;
            string ret = "Data source=" + fileNameText.Text + ";" + (newCheck.Checked ? "New=True" : "") + ";Version=3; UseUTF16Encoding=" + useUTF16.Checked.ToString() + ";Legacy Format=" + usePre33Format.Checked.ToString() + ";Read Only=" + readOnly.Checked.ToString() + "; password=\"" + password.Text + "\";Compress=True;"; 
            return ret;
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

        public IDBBrowser CreateBrowser()
        {
            SQLiteDBBrowser browser = new SQLiteDBBrowser(conection);
            browser.FilePath = fileNameText.Text;
            return browser;
        }

        public object GetConnection()
        {

            return this.conection;
        }

        public bool OpenConnection()
        {
            this.conection = new SQLiteConnection(GetString());
            try
            {
                if(password.Text!="")
                    conection.SetPassword(password.Text);
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
            this.conection = new SQLiteConnection(GetString());
            try
            {
                if (password.Text != "")
                    conection.SetPassword(password.Text);
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
            this.label3 = new System.Windows.Forms.Label();
            this.fileNameText = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.useUTF16 = new System.Windows.Forms.CheckBox();
            this.usePre33Format = new System.Windows.Forms.CheckBox();
            this.readOnly = new System.Windows.Forms.CheckBox();
            this.newCheck = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data source (filename)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            // 
            // fileNameText
            // 
            this.fileNameText.Location = new System.Drawing.Point(125, 30);
            this.fileNameText.Name = "fileNameText";
            this.fileNameText.Size = new System.Drawing.Size(230, 20);
            this.fileNameText.TabIndex = 0;
            this.fileNameText.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(125, 56);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(230, 20);
            this.password.TabIndex = 2;
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.fields_Changed);
            // 
            // useUTF16
            // 
            this.useUTF16.AutoSize = true;
            this.useUTF16.Location = new System.Drawing.Point(125, 105);
            this.useUTF16.Name = "useUTF16";
            this.useUTF16.Size = new System.Drawing.Size(81, 17);
            this.useUTF16.TabIndex = 3;
            this.useUTF16.Text = "Use UTF16";
            this.useUTF16.UseVisualStyleBackColor = true;
            this.useUTF16.CheckedChanged += new System.EventHandler(this.readOnly_CheckedChanged);
            // 
            // usePre33Format
            // 
            this.usePre33Format.AutoSize = true;
            this.usePre33Format.Location = new System.Drawing.Point(125, 82);
            this.usePre33Format.Name = "usePre33Format";
            this.usePre33Format.Size = new System.Drawing.Size(165, 17);
            this.usePre33Format.TabIndex = 3;
            this.usePre33Format.Text = "Use pre 3.3x database format";
            this.usePre33Format.UseVisualStyleBackColor = true;
            this.usePre33Format.CheckedChanged += new System.EventHandler(this.readOnly_CheckedChanged);
            // 
            // readOnly
            // 
            this.readOnly.AutoSize = true;
            this.readOnly.Location = new System.Drawing.Point(125, 128);
            this.readOnly.Name = "readOnly";
            this.readOnly.Size = new System.Drawing.Size(71, 17);
            this.readOnly.TabIndex = 3;
            this.readOnly.Text = "Readonly";
            this.readOnly.UseVisualStyleBackColor = true;
            this.readOnly.CheckedChanged += new System.EventHandler(this.readOnly_CheckedChanged);
            // 
            // newCheck
            // 
            this.newCheck.AutoSize = true;
            this.newCheck.Location = new System.Drawing.Point(392, 33);
            this.newCheck.Name = "newCheck";
            this.newCheck.Size = new System.Drawing.Size(48, 17);
            this.newCheck.TabIndex = 4;
            this.newCheck.Text = "New";
            this.newCheck.UseVisualStyleBackColor = true;
            this.newCheck.CheckedChanged += new System.EventHandler(this.readOnly_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(358, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 20);
            this.button1.TabIndex = 5;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SQLiteconnectionBuilder
            // 
            this.Controls.Add(this.button1);
            this.Controls.Add(this.newCheck);
            this.Controls.Add(this.usePre33Format);
            this.Controls.Add(this.readOnly);
            this.Controls.Add(this.useUTF16);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fileNameText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.password);
            this.Name = "SQLiteconnectionBuilder";
            this.Size = new System.Drawing.Size(445, 184);
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
        }

        private void charSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!wait4change)
                changed = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (newCheck.Checked)
            {
                SaveFileDialog opfd = new SaveFileDialog();
                opfd.AddExtension = true;
                opfd.DefaultExt = "db3";
                opfd.Filter = "db3 file|*.db3|all files|*.*";
                opfd.ShowDialog();
                if (opfd.FileName != "")
                    fileNameText.Text = opfd.FileName;
            }
            else
            {
                OpenFileDialog opfd = new OpenFileDialog();
                opfd.Filter = "db3 file|*.db3|all files|*.*";
                opfd.ShowDialog();
                if (opfd.FileName != "")
                    fileNameText.Text = opfd.FileName;
            }
        }

        private void readOnly_CheckedChanged(object sender, EventArgs e)
        {
            changed = true;
        }





    }





}
