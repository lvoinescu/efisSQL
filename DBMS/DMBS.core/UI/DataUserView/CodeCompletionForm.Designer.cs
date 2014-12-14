namespace DBMS.core
{
    partial class CodeCompletionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeCompletionForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.codeCompletionListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "connect.png");
            this.imageList1.Images.SetKeyName(1, "exec all.png");
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 256;
            // 
            // codeCompletionListView
            // 
            this.codeCompletionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.codeCompletionListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeCompletionListView.FullRowSelect = true;
            this.codeCompletionListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.codeCompletionListView.HideSelection = false;
            this.codeCompletionListView.LargeImageList = this.imageList1;
            this.codeCompletionListView.Location = new System.Drawing.Point(0, 0);
            this.codeCompletionListView.MultiSelect = false;
            this.codeCompletionListView.Name = "codeCompletionListView";
            this.codeCompletionListView.OwnerDraw = true;
            this.codeCompletionListView.ShowItemToolTips = true;
            this.codeCompletionListView.Size = new System.Drawing.Size(303, 211);
            this.codeCompletionListView.SmallImageList = this.imageList1;
            this.codeCompletionListView.TabIndex = 3;
            this.codeCompletionListView.UseCompatibleStateImageBehavior = false;
            this.codeCompletionListView.View = System.Windows.Forms.View.Details;
            this.codeCompletionListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.codeCompletionListView_MouseDoubleClick);
            this.codeCompletionListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.codeCompletionListView_DrawItem);
            this.codeCompletionListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.codeCompletionListView_MouseDown);
            // 
            // CodeCompletionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 211);
            this.Controls.Add(this.codeCompletionListView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CodeCompletionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CodeCompletionForm";
            this.Load += new System.EventHandler(this.CodeCompletionForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView codeCompletionListView;
    }
}