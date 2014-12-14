namespace DBMS.SQLite
{
    partial class ExecuteBatchForm
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecuteBatchForm));
        	System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        	this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        	this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
        	this.textBox1 = new System.Windows.Forms.ToolStripTextBox();
        	this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
        	this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
        	this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        	this.mode = new System.Windows.Forms.ToolStripComboBox();
        	this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
        	this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
        	this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
        	this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
        	this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
        	this.fileSizeText = new System.Windows.Forms.ToolStripStatusLabel();
        	this.statusStrip1 = new System.Windows.Forms.StatusStrip();
        	this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        	this.Column3 = new System.Windows.Forms.DataGridViewButtonColumn();
        	this.Column4 = new System.Windows.Forms.DataGridViewButtonColumn();
        	this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.dg = new System.Windows.Forms.DataGridView();
        	this.toolStrip1.SuspendLayout();
        	this.statusStrip1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// toolStrip1
        	// 
        	this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        	this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.toolStripLabel1,
        	        	        	this.textBox1,
        	        	        	this.toolStripButton3,
        	        	        	this.toolStripSeparator1,
        	        	        	this.toolStripButton1,
        	        	        	this.toolStripButton4,
        	        	        	this.toolStripButton2,
        	        	        	this.toolStripSeparator2,
        	        	        	this.mode});
        	this.toolStrip1.Location = new System.Drawing.Point(0, 0);
        	this.toolStrip1.Name = "toolStrip1";
        	this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.toolStrip1.Size = new System.Drawing.Size(860, 35);
        	this.toolStrip1.TabIndex = 18;
        	this.toolStrip1.Text = "toolStrip1";
        	// 
        	// toolStripLabel1
        	// 
        	this.toolStripLabel1.Name = "toolStripLabel1";
        	this.toolStripLabel1.Size = new System.Drawing.Size(55, 32);
        	this.toolStripLabel1.Text = "File path:";
        	// 
        	// textBox1
        	// 
        	this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.textBox1.Name = "textBox1";
        	this.textBox1.Size = new System.Drawing.Size(400, 35);
        	// 
        	// toolStripButton3
        	// 
        	this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        	this.toolStripButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.toolStripButton3.Name = "toolStripButton3";
        	this.toolStripButton3.Size = new System.Drawing.Size(39, 32);
        	this.toolStripButton3.Text = "  ...  ";
        	this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
        	this.toolStripButton3.ToolTipText = "Browse for file";
        	this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
        	// 
        	// toolStripButton1
        	// 
        	this.toolStripButton1.AutoSize = false;
        	this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
        	this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.toolStripButton1.Name = "toolStripButton1";
        	this.toolStripButton1.Size = new System.Drawing.Size(32, 32);
        	this.toolStripButton1.Text = "Run";
        	this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
        	// 
        	// toolStripButton4
        	// 
        	this.toolStripButton4.AutoSize = false;
        	this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
        	this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.toolStripButton4.Name = "toolStripButton4";
        	this.toolStripButton4.Size = new System.Drawing.Size(32, 32);
        	this.toolStripButton4.Text = "Stop";
        	this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
        	// 
        	// toolStripButton2
        	// 
        	this.toolStripButton2.AutoSize = false;
        	this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
        	this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
        	this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.toolStripButton2.Name = "toolStripButton2";
        	this.toolStripButton2.Size = new System.Drawing.Size(32, 32);
        	this.toolStripButton2.Text = "Clear";
        	this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
        	// 
        	// toolStripSeparator2
        	// 
        	this.toolStripSeparator2.Name = "toolStripSeparator2";
        	this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
        	// 
        	// mode
        	// 
        	this.mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.mode.Items.AddRange(new object[] {
        	        	        	"Read & execute",
        	        	        	"Read only"});
        	this.mode.Name = "mode";
        	this.mode.Size = new System.Drawing.Size(121, 35);
        	// 
        	// progressBar1
        	// 
        	this.progressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        	this.progressBar1.Name = "progressBar1";
        	this.progressBar1.Size = new System.Drawing.Size(200, 16);
        	// 
        	// toolStripStatusLabel1
        	// 
        	this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
        	this.toolStripStatusLabel1.Size = new System.Drawing.Size(98, 17);
        	this.toolStripStatusLabel1.Text = "Executed queries:";
        	this.toolStripStatusLabel1.Visible = false;
        	// 
        	// toolStripStatusLabel3
        	// 
        	this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
        	this.toolStripStatusLabel3.Size = new System.Drawing.Size(0, 17);
        	this.toolStripStatusLabel3.Visible = false;
        	// 
        	// toolStripStatusLabel4
        	// 
        	this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
        	this.toolStripStatusLabel4.Size = new System.Drawing.Size(73, 17);
        	this.toolStripStatusLabel4.Text = "Current read";
        	this.toolStripStatusLabel4.Visible = false;
        	// 
        	// toolStripStatusLabel2
        	// 
        	this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
        	this.toolStripStatusLabel2.Size = new System.Drawing.Size(124, 17);
        	this.toolStripStatusLabel2.Text = "                                       ";
        	this.toolStripStatusLabel2.Visible = false;
        	// 
        	// fileSizeText
        	// 
        	this.fileSizeText.Name = "fileSizeText";
        	this.fileSizeText.Size = new System.Drawing.Size(43, 17);
        	this.fileSizeText.Text = "fileSize";
        	this.fileSizeText.Visible = false;
        	// 
        	// statusStrip1
        	// 
        	this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.progressBar1,
        	        	        	this.toolStripStatusLabel1,
        	        	        	this.toolStripStatusLabel3,
        	        	        	this.toolStripStatusLabel4,
        	        	        	this.toolStripStatusLabel2,
        	        	        	this.fileSizeText});
        	this.statusStrip1.Location = new System.Drawing.Point(0, 424);
        	this.statusStrip1.Name = "statusStrip1";
        	this.statusStrip1.Size = new System.Drawing.Size(860, 22);
        	this.statusStrip1.TabIndex = 20;
        	this.statusStrip1.Text = "statusStrip1";
        	// 
        	// backgroundWorker1
        	// 
        	this.backgroundWorker1.WorkerReportsProgress = true;
        	this.backgroundWorker1.WorkerSupportsCancellation = true;
        	this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
        	// 
        	// Column3
        	// 
        	this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
        	this.Column3.HeaderText = "";
        	this.Column3.MinimumWidth = 50;
        	this.Column3.Name = "Column3";
        	this.Column3.Text = "Run";
        	this.Column3.Width = 50;
        	// 
        	// Column4
        	// 
        	this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        	dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        	this.Column4.DefaultCellStyle = dataGridViewCellStyle1;
        	this.Column4.HeaderText = "Show/Preview";
        	this.Column4.Name = "Column4";
        	// 
        	// Column2
        	// 
        	this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
        	this.Column2.Frozen = true;
        	this.Column2.HeaderText = "Length (bytes)";
        	this.Column2.Name = "Column2";
        	this.Column2.ReadOnly = true;
        	this.Column2.Width = 205;
        	// 
        	// Column1
        	// 
        	this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
        	this.Column1.Frozen = true;
        	this.Column1.HeaderText = "No.";
        	this.Column1.Name = "Column1";
        	this.Column1.ReadOnly = true;
        	this.Column1.Width = 205;
        	// 
        	// dg
        	// 
        	this.dg.AllowUserToAddRows = false;
        	this.dg.AllowUserToDeleteRows = false;
        	this.dg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        	this.dg.BorderStyle = System.Windows.Forms.BorderStyle.None;
        	this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        	        	        	this.Column1,
        	        	        	this.Column2,
        	        	        	this.Column4,
        	        	        	this.Column3});
        	this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.dg.Location = new System.Drawing.Point(0, 35);
        	this.dg.Name = "dg";
        	this.dg.Size = new System.Drawing.Size(860, 389);
        	this.dg.TabIndex = 19;
        	this.dg.VirtualMode = true;
        	this.dg.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dg_CellValueNeeded);
        	this.dg.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_CellContentClick);
        	// 
        	// ExecuteBatchForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(860, 446);
        	this.Controls.Add(this.dg);
        	this.Controls.Add(this.statusStrip1);
        	this.Controls.Add(this.toolStrip1);
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.Name = "ExecuteBatchForm";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Execute batch";
        	this.Load += new System.EventHandler(this.ExecuteBatchForm_Load);
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExecuteBatchForm_FormClosed);
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExecuteBatchForm_FormClosing);
        	this.toolStrip1.ResumeLayout(false);
        	this.toolStrip1.PerformLayout();
        	this.statusStrip1.ResumeLayout(false);
        	this.statusStrip1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox textBox1;
        private System.Windows.Forms.ToolStripComboBox mode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel fileSizeText;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.DataGridViewButtonColumn Column3;
        private System.Windows.Forms.DataGridViewButtonColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridView dg;
    }
}