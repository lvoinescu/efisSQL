/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 5/22/2012
 * Time: 9:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace DBMS.SQLServer
{
	partial class AlterTableForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.dg1 = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.dg1)).BeginInit();
			this.SuspendLayout();
			// 
			// dg1
			// 
			this.dg1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.dg1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dg1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.Column1,
									this.Column2});
			this.dg1.Location = new System.Drawing.Point(7, 12);
			this.dg1.Name = "dg1";
			this.dg1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dg1.Size = new System.Drawing.Size(282, 457);
			this.dg1.TabIndex = 1;
			this.dg1.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Dg1UserAddedRow);
			this.dg1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dg1RowEnter);
			this.dg1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Dg1UserDeletedRow);
			this.dg1.SelectionChanged += new System.EventHandler(this.Dg1SelectionChanged);
			this.dg1.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.Dg1RowStateChanged);
			this.dg1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dg1CellContentClick);
			// 
			// Column1
			// 
			this.Column1.HeaderText = "id";
			this.Column1.Name = "Column1";
			this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Column2
			// 
			this.Column2.HeaderText = "type";
			this.Column2.Items.AddRange(new object[] {
									"bigint",
									"binary",
									"bit",
									"blob",
									"bool",
									"boolean",
									"char",
									"date",
									"datetime",
									"decimal",
									"double",
									"enum",
									"float",
									"int",
									"longblob",
									"longtext",
									"mediumblob",
									"mediumint",
									"mediumtext",
									"numeric",
									"real",
									"set",
									"smallint",
									"text",
									"time",
									"timestamp",
									"tinyblob",
									"tinyint",
									"tinytext",
									"varbinary",
									"varchar",
									"year"});
			this.Column2.Name = "Column2";
			this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(299, 12);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(296, 456);
			this.propertyGrid1.TabIndex = 2;
			// 
			// AlterTableForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(597, 533);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.dg1);
			this.Name = "AlterTableForm";
			this.Text = "AlterTableForm";
			this.Load += new System.EventHandler(this.AlterTableFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.dg1)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridView dg1;
	}
}
