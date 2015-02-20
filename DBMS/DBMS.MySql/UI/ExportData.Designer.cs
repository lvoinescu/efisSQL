namespace DBMS.MySQL
{
    partial class exportData
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(exportData));
        	this.tabControl1 = new Crownwood.Magic.Controls.TabControl();
        	this.exportCsvPage = new Crownwood.Magic.Controls.TabPage();
        	this.textBox6 = new System.Windows.Forms.TextBox();
        	this.textBox4 = new System.Windows.Forms.TextBox();
        	this.textBox3 = new System.Windows.Forms.TextBox();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.textBox1 = new System.Windows.Forms.TextBox();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.exportXmlPage = new Crownwood.Magic.Controls.TabPage();
        	this.exportHtmlPage = new Crownwood.Magic.Controls.TabPage();
        	this.listView1 = new System.Windows.Forms.ListView();
        	this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        	this.label5 = new System.Windows.Forms.Label();
        	this.textBox5 = new System.Windows.Forms.TextBox();
        	this.button2 = new System.Windows.Forms.Button();
        	this.button1 = new System.Windows.Forms.Button();
        	this.checkBox1 = new System.Windows.Forms.CheckBox();
        	this.label6 = new System.Windows.Forms.Label();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        	this.progressBar1 = new System.Windows.Forms.ProgressBar();
        	this.status = new System.Windows.Forms.Label();
        	this.tabControl1.SuspendLayout();
        	this.exportCsvPage.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// tabControl1
        	// 
        	this.tabControl1.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
        	this.tabControl1.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.HideAlways;
        	this.tabControl1.IDEPixelArea = true;
        	this.tabControl1.Location = new System.Drawing.Point(263, 75);
        	this.tabControl1.Name = "tabControl1";
        	this.tabControl1.SelectedIndex = 0;
        	this.tabControl1.SelectedTab = this.exportCsvPage;
        	this.tabControl1.Size = new System.Drawing.Size(298, 249);
        	this.tabControl1.TabIndex = 3;
        	this.tabControl1.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
        	        	        	this.exportCsvPage,
        	        	        	this.exportXmlPage,
        	        	        	this.exportHtmlPage});
        	this.tabControl1.SelectionChanged += new System.EventHandler(this.tabControl1_SelectionChanged);
        	// 
        	// exportCsvPage
        	// 
        	this.exportCsvPage.Controls.Add(this.textBox6);
        	this.exportCsvPage.Controls.Add(this.textBox4);
        	this.exportCsvPage.Controls.Add(this.textBox3);
        	this.exportCsvPage.Controls.Add(this.textBox2);
        	this.exportCsvPage.Controls.Add(this.label4);
        	this.exportCsvPage.Controls.Add(this.label3);
        	this.exportCsvPage.Controls.Add(this.textBox1);
        	this.exportCsvPage.Controls.Add(this.label2);
        	this.exportCsvPage.Controls.Add(this.label1);
        	this.exportCsvPage.Location = new System.Drawing.Point(0, 0);
        	this.exportCsvPage.Name = "exportCsvPage";
        	this.exportCsvPage.Size = new System.Drawing.Size(298, 249);
        	this.exportCsvPage.TabIndex = 3;
        	this.exportCsvPage.Title = "CSV";
        	// 
        	// textBox6
        	// 
        	this.textBox6.Location = new System.Drawing.Point(22, 147);
        	this.textBox6.Multiline = true;
        	this.textBox6.Name = "textBox6";
        	this.textBox6.Size = new System.Drawing.Size(261, 73);
        	this.textBox6.TabIndex = 2;
        	this.textBox6.Visible = false;
        	// 
        	// textBox4
        	// 
        	this.textBox4.Location = new System.Drawing.Point(139, 111);
        	this.textBox4.Name = "textBox4";
        	this.textBox4.Size = new System.Drawing.Size(100, 23);
        	this.textBox4.TabIndex = 1;
        	this.textBox4.Text = "\\r\\n";
        	// 
        	// textBox3
        	// 
        	this.textBox3.Location = new System.Drawing.Point(139, 81);
        	this.textBox3.Name = "textBox3";
        	this.textBox3.Size = new System.Drawing.Size(100, 23);
        	this.textBox3.TabIndex = 1;
        	this.textBox3.Text = "\\";
        	// 
        	// textBox2
        	// 
        	this.textBox2.Location = new System.Drawing.Point(139, 52);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(100, 23);
        	this.textBox2.TabIndex = 1;
        	this.textBox2.Text = "\"";
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(22, 114);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(111, 15);
        	this.label4.TabIndex = 0;
        	this.label4.Text = "Lines terminated by";
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(34, 85);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(99, 15);
        	this.label3.TabIndex = 0;
        	this.label3.Text = "Fields escaped by";
        	// 
        	// textBox1
        	// 
        	this.textBox1.Location = new System.Drawing.Point(139, 24);
        	this.textBox1.Name = "textBox1";
        	this.textBox1.Size = new System.Drawing.Size(100, 23);
        	this.textBox1.TabIndex = 1;
        	this.textBox1.Text = ",";
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(30, 56);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(103, 15);
        	this.label2.TabIndex = 0;
        	this.label2.Text = "Fields enclosed by";
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(19, 27);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(114, 15);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "Fields terminated by";
        	// 
        	// exportXmlPage
        	// 
        	this.exportXmlPage.Location = new System.Drawing.Point(0, 0);
        	this.exportXmlPage.Name = "exportXmlPage";
        	this.exportXmlPage.Selected = false;
        	this.exportXmlPage.Size = new System.Drawing.Size(298, 249);
        	this.exportXmlPage.TabIndex = 4;
        	this.exportXmlPage.Title = "XML";
        	// 
        	// exportHtmlPage
        	// 
        	this.exportHtmlPage.Location = new System.Drawing.Point(0, 0);
        	this.exportHtmlPage.Name = "exportHtmlPage";
        	this.exportHtmlPage.Selected = false;
        	this.exportHtmlPage.Size = new System.Drawing.Size(298, 249);
        	this.exportHtmlPage.TabIndex = 5;
        	this.exportHtmlPage.Title = "HTML";
        	// 
        	// listView1
        	// 
        	this.listView1.CheckBoxes = true;
        	this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader1});
        	this.listView1.Location = new System.Drawing.Point(12, 47);
        	this.listView1.Name = "listView1";
        	this.listView1.Size = new System.Drawing.Size(240, 292);
        	this.listView1.TabIndex = 4;
        	this.listView1.UseCompatibleStateImageBehavior = false;
        	this.listView1.View = System.Windows.Forms.View.Details;
        	// 
        	// columnHeader1
        	// 
        	this.columnHeader1.Text = "Fields to export";
        	this.columnHeader1.Width = 222;
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(9, 5);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(52, 13);
        	this.label5.TabIndex = 13;
        	this.label5.Text = "File name";
        	// 
        	// textBox5
        	// 
        	this.textBox5.Location = new System.Drawing.Point(12, 21);
        	this.textBox5.Name = "textBox5";
        	this.textBox5.Size = new System.Drawing.Size(468, 20);
        	this.textBox5.TabIndex = 12;
        	// 
        	// button2
        	// 
        	this.button2.Location = new System.Drawing.Point(486, 19);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 11;
        	this.button2.Text = "...";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.button2_Click);
        	// 
        	// button1
        	// 
        	this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button1.Location = new System.Drawing.Point(466, 330);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(96, 37);
        	this.button1.TabIndex = 14;
        	this.button1.Text = "Export";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.button1_Click);
        	// 
        	// checkBox1
        	// 
        	this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.checkBox1.AutoSize = true;
        	this.checkBox1.Checked = true;
        	this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.checkBox1.Location = new System.Drawing.Point(13, 354);
        	this.checkBox1.Name = "checkBox1";
        	this.checkBox1.Size = new System.Drawing.Size(114, 17);
        	this.checkBox1.TabIndex = 15;
        	this.checkBox1.Text = "Select/deselect all";
        	this.checkBox1.UseVisualStyleBackColor = true;
        	this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
        	// 
        	// label6
        	// 
        	this.label6.AutoSize = true;
        	this.label6.Location = new System.Drawing.Point(260, 51);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(69, 13);
        	this.label6.TabIndex = 16;
        	this.label6.Text = "Select format";
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Items.AddRange(new object[] {
        	        	        	"CSV",
        	        	        	"XML",
        	        	        	"HTML"});
        	this.comboBox1.Location = new System.Drawing.Point(335, 48);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(226, 21);
        	this.comboBox1.TabIndex = 17;
        	this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        	// 
        	// backgroundWorker1
        	// 
        	this.backgroundWorker1.WorkerReportsProgress = true;
        	this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
        	this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
        	// 
        	// progressBar1
        	// 
        	this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.progressBar1.Location = new System.Drawing.Point(0, 373);
        	this.progressBar1.Name = "progressBar1";
        	this.progressBar1.Size = new System.Drawing.Size(574, 13);
        	this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
        	this.progressBar1.TabIndex = 18;
        	this.progressBar1.Visible = false;
        	// 
        	// status
        	// 
        	this.status.AutoSize = true;
        	this.status.Location = new System.Drawing.Point(263, 354);
        	this.status.Name = "status";
        	this.status.Size = new System.Drawing.Size(40, 13);
        	this.status.TabIndex = 19;
        	this.status.Text = "Status.";
        	this.status.Visible = false;
        	// 
        	// exportData
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(574, 386);
        	this.Controls.Add(this.status);
        	this.Controls.Add(this.progressBar1);
        	this.Controls.Add(this.comboBox1);
        	this.Controls.Add(this.label6);
        	this.Controls.Add(this.checkBox1);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.label5);
        	this.Controls.Add(this.textBox5);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.listView1);
        	this.Controls.Add(this.tabControl1);
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.Name = "exportData";
        	this.ShowInTaskbar = false;
        	this.Text = "Export data";
        	this.Load += new System.EventHandler(this.ExportData_Load);
        	this.tabControl1.ResumeLayout(false);
        	this.exportCsvPage.ResumeLayout(false);
        	this.exportCsvPage.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private Crownwood.Magic.Controls.TabControl tabControl1;
        private Crownwood.Magic.Controls.TabPage exportCsvPage;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Crownwood.Magic.Controls.TabPage exportXmlPage;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private Crownwood.Magic.Controls.TabPage exportHtmlPage;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label status;
    }
}