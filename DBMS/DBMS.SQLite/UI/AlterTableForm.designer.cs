namespace DBMS.SQLite
{
    partial class AlterTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlterTableForm));
            this.dg1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.table = new System.Windows.Forms.TextBox();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.collation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.unsigned = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).BeginInit();
            this.SuspendLayout();
            // 
            // dg1
            // 
            this.dg1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dg1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column6,
            this.collation,
            this.Column4,
            this.Column7,
            this.Column5,
            this.unsigned,
            this.Column8});
            this.dg1.Location = new System.Drawing.Point(12, 50);
            this.dg1.Name = "dg1";
            this.dg1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg1.Size = new System.Drawing.Size(734, 367);
            this.dg1.TabIndex = 0;
            this.dg1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dg1_UserDeletingRow);
            this.dg1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg1_CellEndEdit);
            this.dg1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg1_CellContentClick);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(654, 423);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Alter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 423);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 33);
            this.button2.TabIndex = 2;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Table name";
            // 
            // table
            // 
            this.table.Location = new System.Drawing.Point(86, 12);
            this.table.Name = "table";
            this.table.Size = new System.Drawing.Size(180, 20);
            this.table.TabIndex = 2;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "id";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 21;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "type";
            this.Column2.Items.AddRange(new object[] {
            "",
            "BIGINT",
            "BINARY",
            "BLOB",
            "BLOB_TEXT",
            "BOOL",
            "BOOLEAN",
            "CHAR",
            "CLOB",
            "CURRENCY",
            "DATE",
            "DATETIME",
            "DEC",
            "DECIMAL",
            "DOUBLE",
            "DOUBLE PRECISION",
            "FLOAT",
            "GRAPHIC",
            "GUID",
            "IMAGE",
            "INT",
            "INT64",
            "INTEGER",
            "LARGEINT",
            "MEMO",
            "MONEY",
            "NCHAR",
            "NTEXT",
            "NUMBER",
            "NUMERIC",
            "NVARCHAR",
            "NVARCHAR2",
            "PHOTO",
            "PICTURE",
            "RAW",
            "REAL",
            "SMALLINT",
            "SMALLMONEY",
            "TEXT",
            "TIME",
            "TIMESTAMP",
            "TINYINT",
            "VARBINARY",
            "VARCHAR",
            "VARCHAR2",
            "WORD"});
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column2.Width = 52;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "length";
            this.Column3.Name = "Column3";
            this.Column3.Width = 61;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "default";
            this.Column6.Name = "Column6";
            this.Column6.Width = 64;
            // 
            // collation
            // 
            this.collation.HeaderText = "collation";
            this.collation.Items.AddRange(new object[] {
            "BINARY",
            "NOCASE",
            "RTRIM"});
            this.collation.Name = "collation";
            this.collation.Width = 52;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "primary key";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column4.Width = 85;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "auto increment";
            this.Column7.Name = "Column7";
            this.Column7.Width = 83;
            // 
            // Column5
            // 
            this.Column5.FalseValue = "false";
            this.Column5.HeaderText = "not null";
            this.Column5.Name = "Column5";
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column5.TrueValue = "bool";
            this.Column5.Width = 66;
            // 
            // unsigned
            // 
            this.unsigned.HeaderText = "unsigned";
            this.unsigned.Name = "unsigned";
            this.unsigned.Width = 56;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Foreign key";
            this.Column8.Name = "Column8";
            this.Column8.Text = "Foreign key";
            this.Column8.Width = 68;
            // 
            // AlterTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 462);
            this.Controls.Add(this.table);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dg1);
            this.Controls.Add(this.label10);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AlterTableForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alter";
            this.Load += new System.EventHandler(this.AlterTableForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox table;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewComboBoxColumn collation;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn unsigned;
        private System.Windows.Forms.DataGridViewButtonColumn Column8;
    }
}