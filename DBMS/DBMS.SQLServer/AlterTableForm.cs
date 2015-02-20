/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 5/22/2012
 * Time: 9:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DBMS.SQLServer
{
	/// <summary>
	/// Description of AlterTableForm.
	/// </summary>
	public partial class AlterTableForm : Form
	{
        protected static SqlConnection connection;
        private string dataBase ,tableName;
        private List<PropertyExporter> columns;
        private List<string> rowsDeleted;
        public enum Mod { Alter, Create };
       	private Mod mod;
        public AlterTableForm(SqlConnection con, string dataBase, string tableName, Mod mod)
        {
            this.mod = mod;
            connection = con;
            this.dataBase = dataBase;
            this.tableName = tableName;
            rowsDeleted = new List<string>();
            InitializeComponent();
           // SwitchLanguage();
        }
		
		void AlterTableFormLoad(object sender, EventArgs e)
		{
			PropertyExporter pe = new PropertyExporter();
			propertyGrid1.SelectedObject = pe;
			columns = new List<AlterTableForm.PropertyExporter>();
		}
		
		
		
		private class StringListConverter : TypeConverter
		{
			
			public override bool	GetStandardValuesSupported(ITypeDescriptorContext context)
			{
			return true; // display drop
			}
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
			return true; // drop-down vs combo
			}
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				string [] types = new string[]{} ;
				SqlCommand cmd = new SqlCommand("SELECT name FROM sys.types", connection);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataTable dt = new DataTable();
				da.Fill(dt);
				if(dt.Rows.Count>0)
				{
					types = new string[dt.Rows.Count];
					for(int i=0;i<dt.Rows.Count;i++)
					{
						types[i] = dt.Rows[i]["name"].ToString();
					}
				}
			return new StandardValuesCollection(types);
			}
		} 
		
		
		[DefaultPropertyAttribute("BackupType")]
	    public class PropertyExporter
	    {
	    	
	    	
	
	        private string name;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("(name)")]
			public string Name {
				get { return name; }
				set { name = value; }
			}
	        
	        
	        private bool allowNull;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Allow null")]
			public bool AllowNull {
				get { return allowNull; }
				set { allowNull = value; }
			}
	        
	        private string dataType;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Data type"),TypeConverter(typeof(StringListConverter))]
			public string DataType {
				get { return dataType; }
				set { dataType = value; }
			}
	        private string defaultValue;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Default value")]
			public string DefaultValue {
				get { return defaultValue; }
				set { defaultValue = value; }
			}
	        private int length;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Length")]
			public int Length {
				get { return length; }
				set { length = value; }
			}
	        private string collation;
	        [CategoryAttribute("Table Designer"), DescriptionAttribute(""), DisplayName("Collation")]
			public string Collation {
				get { return collation; }
				set { collation = value; }
			}
	
	
	        private string description;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Description")]
			public string Description {
				get { return description; }
				set { description = value; }
			}
	        private bool deterministic;
	        [CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("Deterministic")]
			public bool Deterministic {
				get { return deterministic; }
				set { deterministic = value; }
			}
	        
	        private bool dtsPublished;
			[CategoryAttribute("(General)"), DescriptionAttribute(""), DisplayName("DTS-published")]
	        public bool DtsPublished {
				get { return dtsPublished; }
				set { dtsPublished = value; }
			}
			
	        private bool isFullTextIndexed;
	        [CategoryAttribute("Full-text specification"), DescriptionAttribute(""), DisplayName("(Is full-text indexed)")]
			public bool IsFullTextIndexed {
				get { return isFullTextIndexed; }
				set { isFullTextIndexed = value; }
			}
	        
	        private bool fullTextTypeColumn;
	        [CategoryAttribute("Full-text specification"), DescriptionAttribute(""), DisplayName("Full-text type column")]
			public bool FullTextTypeColumn {
				get { return fullTextTypeColumn; }
				set { fullTextTypeColumn = value; }
			}
	        private bool language;
	        [CategoryAttribute("Full-text specification"), DescriptionAttribute(""), DisplayName("Language")]
			public bool Language {
				get { return language; }
				set { language = value; }
			}
	        
	        private bool hasNonSQLSubscriber;
	        [CategoryAttribute(""), DescriptionAttribute(""), DisplayName("Has NonSQL Subscriber")]
			public bool HasNonSQLSubscriber {
				get { return hasNonSQLSubscriber; }
				set { hasNonSQLSubscriber = value; }
			}
	
	        private string isIdentity;
	        [CategoryAttribute("Identity specification"), DescriptionAttribute(""), DisplayName("Is Identity")]
			public string IsIdentity {
				get { return isIdentity; }
				set { isIdentity = value; }
			}
	        private int identityIncrement;
	        [CategoryAttribute("Identity specification"), DescriptionAttribute(""), DisplayName("Identity increment")]
			public int IdentityIncrement {
				get { return identityIncrement; }
				set { identityIncrement = value; }
			}
	        private int identitySeed;
	        [CategoryAttribute("Identity specification"), DescriptionAttribute(""), DisplayName("Identity seed")]
			public int IdentitySeed {
				get { return identitySeed; }
				set { identitySeed = value; }
			}
			
	        private bool indexable;
	        [ DescriptionAttribute(""), DisplayName("Indexable")]
			public bool Indexable {
				get { return indexable; }
				set { indexable = value; }
			}
			private bool mergedPublished;
	        [DescriptionAttribute(""), DisplayName("Merge-published")]
			public bool MergedPublished {
				get { return mergedPublished; }
				set { mergedPublished = value; }
			}
	        
			private bool notForReplication;
	        [DescriptionAttribute(""), DisplayName("Not for replication")]
			public bool NotForReplication {
				get { return notForReplication; }
				set { notForReplication = value; }
			}
	        
			private bool replicated;
	        [DescriptionAttribute(""), DisplayName("Replicated")]
			public bool Replicated {
				get { return replicated; }
				set { replicated = value; }
			}
	        
			private bool rowGuid;
	        [ DescriptionAttribute(""), DisplayName("RowGuid")]
			public bool RowGuid {
				get { return rowGuid; }
				set { rowGuid = value; }
			}
	        
			private bool size;
	        [DescriptionAttribute(""), DisplayName("Size")]
			public bool Size {
				get { return size; }
				set { size = value; }
			}
	
	
	        public PropertyExporter()
	        {
	
	        }
	
	
	
	
	
	    }
		
	
		
		void Dg1UserAddedRow(object sender, DataGridViewRowEventArgs e)
		{
			columns.Add(new PropertyExporter());
		}
		
		void Dg1UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
            if (e.Row.Index < 0) return;
			columns.RemoveAt(e.Row.Index);
		}
		
		void Dg1CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if(e.RowIndex<dg1.Rows.Count-1)
				propertyGrid1.SelectedObject = columns[e.RowIndex];
		}
		
		void Dg1SelectionChanged(object sender, EventArgs e)
		{
			
		}
		
		void Dg1RowEnter(object sender, DataGridViewCellEventArgs e)
		{
		}
		
		void Dg1RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
		{
			if (e.StateChanged != DataGridViewElementStates.Selected) 
				return;
			if(e.Row.Index<dg1.Rows.Count-1)
				propertyGrid1.SelectedObject = columns[e.Row.Index];
		}
	}
		
	

	
}
