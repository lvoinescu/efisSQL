namespace efisSQL
{
    partial class SelectDBEngineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDBEngineForm));
        	this.connectButton = new System.Windows.Forms.Button();
        	this.mySqlConnexionBuilder1 = new DBMS.MySQL.MySqlconnectionBuilder();
        	this.sqLiteconnectionBuilder1 = new DBMS.SQLite.SQLiteconnectionBuilder();
        	this.radioButton1 = new System.Windows.Forms.RadioButton();
        	this.radioButton2 = new System.Windows.Forms.RadioButton();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.newButton = new System.Windows.Forms.Button();
        	this.deleteButton = new System.Windows.Forms.Button();
        	this.saveButton = new System.Windows.Forms.Button();
        	this.testButton = new System.Windows.Forms.Button();
        	this.dmbsTabControl = new System.Windows.Forms.TabControl();
        	this.mysqlPage = new System.Windows.Forms.TabPage();
        	this.sqliteTab = new System.Windows.Forms.TabPage();
        	this.tabPage1 = new System.Windows.Forms.TabPage();
        	this.sqlServerconnectionBuilder1 = new DBMS.SQLServer.SQLServerconnectionBuilder();
        	this.dmbsTabControl.SuspendLayout();
        	this.mysqlPage.SuspendLayout();
        	this.sqliteTab.SuspendLayout();
        	this.tabPage1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// connectButton
        	// 
        	this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.connectButton.Location = new System.Drawing.Point(440, 287);
        	this.connectButton.Name = "connectButton";
        	this.connectButton.Size = new System.Drawing.Size(75, 26);
        	this.connectButton.TabIndex = 1;
        	this.connectButton.Text = "Connect";
        	this.connectButton.UseVisualStyleBackColor = true;
        	this.connectButton.Click += new System.EventHandler(this.button1_Click);
        	// 
        	// mySqlConnexionBuilder1
        	// 
        	this.mySqlConnexionBuilder1.Changed = false;
        	this.mySqlConnexionBuilder1.ConnectionSetting = null;
        	this.mySqlConnexionBuilder1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.mySqlConnexionBuilder1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.mySqlConnexionBuilder1.Location = new System.Drawing.Point(3, 3);
        	this.mySqlConnexionBuilder1.Name = "mySqlConnexionBuilder1";
        	this.mySqlConnexionBuilder1.Size = new System.Drawing.Size(486, 213);
        	this.mySqlConnexionBuilder1.TabIndex = 0;
        	// 
        	// sqLiteconnectionBuilder1
        	// 
        	this.sqLiteconnectionBuilder1.Changed = false;
        	this.sqLiteconnectionBuilder1.ConnectionSetting = null;
        	this.sqLiteconnectionBuilder1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.sqLiteconnectionBuilder1.Location = new System.Drawing.Point(3, 3);
        	this.sqLiteconnectionBuilder1.Name = "sqLiteconnectionBuilder1";
        	this.sqLiteconnectionBuilder1.Size = new System.Drawing.Size(486, 213);
        	this.sqLiteconnectionBuilder1.TabIndex = 0;
        	// 
        	// radioButton1
        	// 
        	this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.radioButton1.AutoSize = true;
        	this.radioButton1.Checked = true;
        	this.radioButton1.Location = new System.Drawing.Point(15, 292);
        	this.radioButton1.Name = "radioButton1";
        	this.radioButton1.Size = new System.Drawing.Size(155, 17);
        	this.radioButton1.TabIndex = 3;
        	this.radioButton1.TabStop = true;
        	this.radioButton1.Text = "Integrate in existing window";
        	this.radioButton1.UseVisualStyleBackColor = true;
        	// 
        	// radioButton2
        	// 
        	this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.radioButton2.AutoSize = true;
        	this.radioButton2.Location = new System.Drawing.Point(204, 292);
        	this.radioButton2.Name = "radioButton2";
        	this.radioButton2.Size = new System.Drawing.Size(86, 17);
        	this.radioButton2.TabIndex = 3;
        	this.radioButton2.Text = "New window";
        	this.radioButton2.UseVisualStyleBackColor = true;
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Location = new System.Drawing.Point(79, 7);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(162, 21);
        	this.comboBox1.TabIndex = 4;
        	this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(12, 10);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(61, 13);
        	this.label1.TabIndex = 5;
        	this.label1.Text = "Connection";
        	// 
        	// newButton
        	// 
        	this.newButton.Location = new System.Drawing.Point(247, 5);
        	this.newButton.Name = "newButton";
        	this.newButton.Size = new System.Drawing.Size(76, 23);
        	this.newButton.TabIndex = 7;
        	this.newButton.Text = "New";
        	this.newButton.UseVisualStyleBackColor = true;
        	this.newButton.Click += new System.EventHandler(this.button2_Click);
        	// 
        	// deleteButton
        	// 
        	this.deleteButton.Location = new System.Drawing.Point(416, 5);
        	this.deleteButton.Name = "deleteButton";
        	this.deleteButton.Size = new System.Drawing.Size(85, 23);
        	this.deleteButton.TabIndex = 7;
        	this.deleteButton.Text = "Delete";
        	this.deleteButton.UseVisualStyleBackColor = true;
        	this.deleteButton.Click += new System.EventHandler(this.button3_Click);
        	// 
        	// saveButton
        	// 
        	this.saveButton.Location = new System.Drawing.Point(327, 5);
        	this.saveButton.Name = "saveButton";
        	this.saveButton.Size = new System.Drawing.Size(85, 23);
        	this.saveButton.TabIndex = 8;
        	this.saveButton.Text = "Save";
        	this.saveButton.UseVisualStyleBackColor = true;
        	this.saveButton.Click += new System.EventHandler(this.button4_Click);
        	// 
        	// testButton
        	// 
        	this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.testButton.Location = new System.Drawing.Point(359, 287);
        	this.testButton.Name = "testButton";
        	this.testButton.Size = new System.Drawing.Size(75, 26);
        	this.testButton.TabIndex = 9;
        	this.testButton.Text = "Test";
        	this.testButton.UseVisualStyleBackColor = true;
        	this.testButton.Click += new System.EventHandler(this.button5_Click);
        	// 
        	// dmbsTabControl
        	// 
        	this.dmbsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.dmbsTabControl.Controls.Add(this.mysqlPage);
        	this.dmbsTabControl.Controls.Add(this.sqliteTab);
        	this.dmbsTabControl.Controls.Add(this.tabPage1);
        	this.dmbsTabControl.Location = new System.Drawing.Point(15, 36);
        	this.dmbsTabControl.Name = "dmbsTabControl";
        	this.dmbsTabControl.SelectedIndex = 0;
        	this.dmbsTabControl.Size = new System.Drawing.Size(500, 245);
        	this.dmbsTabControl.TabIndex = 10;
        	// 
        	// mysqlPage
        	// 
        	this.mysqlPage.Controls.Add(this.mySqlConnexionBuilder1);
        	this.mysqlPage.Location = new System.Drawing.Point(4, 22);
        	this.mysqlPage.Name = "mysqlPage";
        	this.mysqlPage.Padding = new System.Windows.Forms.Padding(3);
        	this.mysqlPage.Size = new System.Drawing.Size(492, 219);
        	this.mysqlPage.TabIndex = 0;
        	this.mysqlPage.Text = "MySQL";
        	this.mysqlPage.UseVisualStyleBackColor = true;
        	// 
        	// sqliteTab
        	// 
        	this.sqliteTab.Controls.Add(this.sqLiteconnectionBuilder1);
        	this.sqliteTab.Location = new System.Drawing.Point(4, 22);
        	this.sqliteTab.Name = "sqliteTab";
        	this.sqliteTab.Padding = new System.Windows.Forms.Padding(3);
        	this.sqliteTab.Size = new System.Drawing.Size(492, 219);
        	this.sqliteTab.TabIndex = 1;
        	this.sqliteTab.Text = "SQLite";
        	this.sqliteTab.UseVisualStyleBackColor = true;
        	// 
        	// tabPage1
        	// 
        	this.tabPage1.Controls.Add(this.sqlServerconnectionBuilder1);
        	this.tabPage1.Location = new System.Drawing.Point(4, 22);
        	this.tabPage1.Name = "tabPage1";
        	this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
        	this.tabPage1.Size = new System.Drawing.Size(492, 219);
        	this.tabPage1.TabIndex = 2;
        	this.tabPage1.Text = "SQL Server";
        	this.tabPage1.UseVisualStyleBackColor = true;
        	// 
        	// sqlServerconnectionBuilder1
        	// 
        	this.sqlServerconnectionBuilder1.Changed = false;
        	this.sqlServerconnectionBuilder1.ConnectionSetting = null;
        	this.sqlServerconnectionBuilder1.Location = new System.Drawing.Point(27, 11);
        	this.sqlServerconnectionBuilder1.Name = "sqlServerconnectionBuilder1";
        	this.sqlServerconnectionBuilder1.Size = new System.Drawing.Size(433, 202);
        	this.sqlServerconnectionBuilder1.TabIndex = 0;
        	// 
        	// SelectDBEngineForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(527, 325);
        	this.Controls.Add(this.dmbsTabControl);
        	this.Controls.Add(this.testButton);
        	this.Controls.Add(this.radioButton2);
        	this.Controls.Add(this.radioButton1);
        	this.Controls.Add(this.connectButton);
        	this.Controls.Add(this.saveButton);
        	this.Controls.Add(this.deleteButton);
        	this.Controls.Add(this.newButton);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.comboBox1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.Name = "SelectDBEngineForm";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.Text = "Connect to a database";
        	this.Load += new System.EventHandler(this.SelectDBEngineForm_Load);
        	this.dmbsTabControl.ResumeLayout(false);
        	this.mysqlPage.ResumeLayout(false);
        	this.sqliteTab.ResumeLayout(false);
        	this.tabPage1.ResumeLayout(false);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private DBMS.SQLServer.SQLServerconnectionBuilder sqlServerconnectionBuilder1;
        private System.Windows.Forms.TabPage tabPage1;

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button saveButton;
        private DBMS.MySQL.MySqlconnectionBuilder mySqlConnexionBuilder1;
        //private DBMS.SQL.SqlConnectionBuilder sqlConnexionBuilder1;
        private DBMS.SQLite.SQLiteconnectionBuilder sqLiteconnectionBuilder1;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.TabControl dmbsTabControl;
        private System.Windows.Forms.TabPage mysqlPage;
        private System.Windows.Forms.TabPage sqliteTab;

    }
}