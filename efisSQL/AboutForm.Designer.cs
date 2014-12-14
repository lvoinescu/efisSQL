namespace efisSQL
{
    partial class AboutForm
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
        	this.label1 = new System.Windows.Forms.Label();
        	this.linkLabel1 = new System.Windows.Forms.LinkLabel();
        	this.label2 = new System.Windows.Forms.Label();
        	this.button1 = new System.Windows.Forms.Button();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.linkLabel2 = new System.Windows.Forms.LinkLabel();
        	this.label5 = new System.Windows.Forms.Label();
        	this.SuspendLayout();
        	// 
        	// label1
        	// 
        	this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(30, 49);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(112, 13);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "This application uses :";
        	// 
        	// linkLabel1
        	// 
        	this.linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.linkLabel1.AutoSize = true;
        	this.linkLabel1.Location = new System.Drawing.Point(205, 72);
        	this.linkLabel1.Name = "linkLabel1";
        	this.linkLabel1.Size = new System.Drawing.Size(146, 13);
        	this.linkLabel1.TabIndex = 1;
        	this.linkLabel1.TabStop = true;
        	this.linkLabel1.Text = "http://www.dotnetmagic.com";
        	// 
        	// label2
        	// 
        	this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(147, 23);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(206, 13);
        	this.label2.TabIndex = 0;
        	this.label2.Text = "efisSQL (Easy Fast Innovative Smart SQL)";
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.button1.Location = new System.Drawing.Point(228, 128);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 2;
        	this.button1.Text = "OK";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.button1_Click);
        	// 
        	// label3
        	// 
        	this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(53, 72);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(146, 13);
        	this.label3.TabIndex = 0;
        	this.label3.Text = "- Magic User Interface Library";
        	// 
        	// label4
        	// 
        	this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(53, 95);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(81, 13);
        	this.label4.TabIndex = 0;
        	this.label4.Text = "- SharpDevelop";
        	// 
        	// linkLabel2
        	// 
        	this.linkLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.linkLabel2.AutoSize = true;
        	this.linkLabel2.Location = new System.Drawing.Point(205, 95);
        	this.linkLabel2.Name = "linkLabel2";
        	this.linkLabel2.Size = new System.Drawing.Size(296, 13);
        	this.linkLabel2.TabIndex = 1;
        	this.linkLabel2.TabStop = true;
        	this.linkLabel2.Text = "http://www.sharpdevelop.net/OpenSource/SD/Default.aspx";
        	// 
        	// label5
        	// 
        	this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(347, 153);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(191, 13);
        	this.label5.TabIndex = 3;
        	this.label5.Text = "    Copyright (C) 2011  Lucian Voinescu";
        	// 
        	// AboutForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(544, 176);
        	this.Controls.Add(this.label5);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.linkLabel2);
        	this.Controls.Add(this.linkLabel1);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.label4);
        	this.Controls.Add(this.label3);
        	this.Controls.Add(this.label1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "AboutForm";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.Text = "About";
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label5;
    }
}