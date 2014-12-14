namespace efisSQL
{
    partial class DBManagerForm
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
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBManagerForm));
        	this.imageList1 = new System.Windows.Forms.ImageList(this.components);
        	this.tabBrowser = new Crownwood.Magic.Controls.TabControl();
        	this.tabPage3 = new Crownwood.Magic.Controls.TabPage();
        	this.timer1 = new System.Windows.Forms.Timer(this.components);
        	this.statusStrip1 = new System.Windows.Forms.StatusStrip();
        	this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        	this.browseTree = new System.Windows.Forms.TreeView();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.label1 = new System.Windows.Forms.Label();
        	this.splitContainer1.Panel1.SuspendLayout();
        	this.splitContainer1.Panel2.SuspendLayout();
        	this.splitContainer1.SuspendLayout();
        	this.panel1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// imageList1
        	// 
        	this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
        	this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
        	this.imageList1.Images.SetKeyName(0, "gnome_list_remove.png");
        	this.imageList1.Images.SetKeyName(1, "gnome_list_add.png");
        	// 
        	// tabBrowser
        	// 
        	this.tabBrowser.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
        	this.tabBrowser.AutoSize = true;
        	this.tabBrowser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.tabBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tabBrowser.IDEPixelArea = true;
        	this.tabBrowser.IDEPixelBorder = false;
        	this.tabBrowser.Location = new System.Drawing.Point(0, 0);
        	this.tabBrowser.Name = "tabBrowser";
        	this.tabBrowser.Size = new System.Drawing.Size(763, 447);
        	this.tabBrowser.TabIndex = 3;
        	// 
        	// tabPage3
        	// 
        	this.tabPage3.Location = new System.Drawing.Point(0, 25);
        	this.tabPage3.Name = "tabPage3";
        	this.tabPage3.Selected = false;
        	this.tabPage3.Size = new System.Drawing.Size(671, 282);
        	this.tabPage3.TabIndex = 3;
        	// 
        	// statusStrip1
        	// 
        	this.statusStrip1.Location = new System.Drawing.Point(0, 447);
        	this.statusStrip1.Name = "statusStrip1";
        	this.statusStrip1.Size = new System.Drawing.Size(928, 22);
        	this.statusStrip1.TabIndex = 7;
        	this.statusStrip1.Text = "statusStrip1";
        	// 
        	// splitContainer1
        	// 
        	this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer1.Location = new System.Drawing.Point(0, 0);
        	this.splitContainer1.Name = "splitContainer1";
        	// 
        	// splitContainer1.Panel1
        	// 
        	this.splitContainer1.Panel1.Controls.Add(this.browseTree);
        	this.splitContainer1.Panel1.Controls.Add(this.panel1);
        	this.splitContainer1.Panel1MinSize = 0;
        	// 
        	// splitContainer1.Panel2
        	// 
        	this.splitContainer1.Panel2.Controls.Add(this.tabBrowser);
        	this.splitContainer1.Size = new System.Drawing.Size(928, 447);
        	this.splitContainer1.SplitterDistance = 161;
        	this.splitContainer1.TabIndex = 8;
        	// 
        	// browseTree
        	// 
        	this.browseTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.browseTree.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.browseTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
        	this.browseTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.browseTree.ImageIndex = 0;
        	this.browseTree.ImageList = this.imageList1;
        	this.browseTree.LabelEdit = true;
        	this.browseTree.Location = new System.Drawing.Point(0, 18);
        	this.browseTree.Name = "browseTree";
        	this.browseTree.SelectedImageIndex = 0;
        	this.browseTree.Size = new System.Drawing.Size(161, 429);
        	this.browseTree.TabIndex = 0;
        	this.browseTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.browseTree_DrawNode);
        	this.browseTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.browseTree_AfterLabelEdit);
        	this.browseTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.browseTree_BeforeExpand);
        	this.browseTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.browseTree_AfterSelect);
        	this.browseTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.browseTree_MouseDown);
        	this.browseTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.browseTree_BeforeSelect);
        	this.browseTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.browseTree_AfterExpand);
        	// 
        	// panel1
        	// 
        	this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
        	this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.panel1.Controls.Add(this.label1);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(161, 18);
        	this.panel1.TabIndex = 1;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Dock = System.Windows.Forms.DockStyle.Left;
        	this.label1.Location = new System.Drawing.Point(0, 0);
        	this.label1.Name = "label1";
        	this.label1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 0);
        	this.label1.Size = new System.Drawing.Size(72, 15);
        	this.label1.TabIndex = 1;
        	this.label1.Text = "Data browser";
        	// 
        	// DBManagerForm
        	// 
        	this.ClientSize = new System.Drawing.Size(928, 469);
        	this.Controls.Add(this.splitContainer1);
        	this.Controls.Add(this.statusStrip1);
        	this.Cursor = System.Windows.Forms.Cursors.Default;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.KeyPreview = true;
        	this.Name = "DBManagerForm";
        	this.Text = "DBManager";
        	this.ResizeBegin += new System.EventHandler(this.DBManagerForm_ResizeBegin);
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DBManagerForm_FormClosing);
        	this.ResizeEnd += new System.EventHandler(this.DBManagerForm_ResizeEnd);
        	this.splitContainer1.Panel1.ResumeLayout(false);
        	this.splitContainer1.Panel2.ResumeLayout(false);
        	this.splitContainer1.Panel2.PerformLayout();
        	this.splitContainer1.ResumeLayout(false);
        	this.panel1.ResumeLayout(false);
        	this.panel1.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private Crownwood.Magic.Controls.TabControl tabBrowser;
        private Crownwood.Magic.Controls.TabPage tabPage3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView browseTree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;




    }
}

