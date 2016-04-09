using System;
using System.Data.SqlClient;
using System.Drawing;

using DBMS.core;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Data;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace DBMS.SQLServer
{
	/// <summary>
	/// Description of SQLServerBrowser.
	/// </summary>
	public class SQLServerBrowser:DBBrowserControl,IDBBrowser
	{



        private long totalRows = 0;
        private ImageList completionListIcons;

		public SQLServerBrowser(SqlConnection connection)
		{
            InitializeComponent();
            browserIsBusy = false;
            sqlConnection = connection;
            history = new History();

            codeCompletionProvider = GenerateCompletionData();

            asyncOperation = AsyncOperationManager.CreateOperation(this);
            postCallBack = new SendOrPostCallback(OnQueryExecuted);
            workerBinder = new BackgroundWorker();
            workerBinder.WorkerSupportsCancellation = true;
            workerBinder.WorkerReportsProgress = true;
            workerBinder.DoWork += new DoWorkEventHandler(worker_DoWork);
            workerBinder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            workerBinder.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

            workerQueryExecutor = new BackgroundWorker();
            workerQueryExecutor.WorkerReportsProgress = false;
            workerQueryExecutor.DoWork += new DoWorkEventHandler(workerQueryExecutor_DoWork);
            workerQueryExecutor.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerQueryExecutor_RunWorkerCompleted);


            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
		}
		
		private string connectionString="";
        public ContextMenuStrip serverMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem refresServerhMenuItem;
        private ToolStripMenuItem newQueryMenuItem;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem newDatabaseMenuItem;
        private ToolStripMenuItem restoreDatabaseFromSqlDumpMenuItem;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem reconnectMenuItem;
        private ToolStripMenuItem disconnectMenuItem;
        public ContextMenuStrip tableMenu;
        private ToolStripMenuItem refreshTabletoolStripMenuItem;
        private ToolStripMenuItem alterToolStripMenuItem;
        private ToolStripMenuItem manageIndexesToolStripMenuItem;
        private ToolStripMenuItem viewReletionshipforeignKeysToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem1;
        private ToolStripMenuItem reorderColumnsToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripMenuItem truncateToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem dropTabelToolStripMenuItem;
        private ToolStripMenuItem emptyToolStripMenuItem1;
        private ToolStripMenuItem renameToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem asSqldumpToolStripMenuItem;
        private ToolStripMenuItem asCSVFileToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem createTriggerToolStripMenuItem;
        private ToolStripMenuItem copyDDLDefinitionToolStripMenuItem;
        private ToolStripMenuItem viewDDLDefinitionToolStripMenuItem;
        public ContextMenuStrip functionMenu;
        private ToolStripMenuItem refreshFunctionsToolStripMenuItem;
        private ToolStripMenuItem newFunctionToolStripMenuItem;
        private ToolStripMenuItem alterFunctionToolStripMenuItem;
        private ToolStripMenuItem dropFunctionToolStripMenuItem;
        public ContextMenuStrip viewMenu;
        private ToolStripMenuItem refreshViewsToolStripMenuItem;
        private ToolStripMenuItem newViewToolStripMenuItem;
        private ToolStripMenuItem alterViewToolStripMenuItem;
        private ToolStripMenuItem dropViewToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem14;
        private ToolStripMenuItem showToolStripMenuItem;
        public ContextMenuStrip triggerMenu;
        private ToolStripMenuItem refreshTriggerStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem11;
        private ToolStripMenuItem toolStripMenuItem12;
        public ContextMenuStrip dataBaseMenu;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem alterDatabaseToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem dropDatabaseToolStripMenuItem;
        private ToolStripMenuItem truncateToolStripMenuItem;
        private ToolStripMenuItem emptyToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem backupToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem createSchemaToolStripMenuItem;
        public ContextMenuStrip procedureMenu;
        private ToolStripMenuItem refreshProcToolStripMenuItem;
        private ToolStripMenuItem newProcToolStripMenuItem;
        private ToolStripMenuItem alterProcToolStripMenuItem;
        private ToolStripMenuItem dropProcToolStripMenuItem;
        private ContextMenuStrip toolsMenu;
        private ToolStripMenuItem serverStatusToolStripMenuItem;
        private ToolStripMenuItem tableDiangosticToolStripMenuItem;
        private ToolStripMenuItem flushToolStripMenuItem;
        private SqlConnection sqlConnection;


        private SQLServerCodeCompletionProvider codeCompletionProvider;
		public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
		
		
		public event TableBoundedHandler TableBounded;
		
		public event NewDataUserViewRequestedHandler NewDataUserViewRequested;
		
		public event DisconnectRequestedEventHandler DisconnectRequestedEvent;
		
		public event QueryExecutedHandler QueryExecuted;
		
		public System.Windows.Forms.ContextMenuStrip TableMenu {
			get {
                return this.tableMenu;
			}
		}
		
		public System.Windows.Forms.ContextMenuStrip DataBaseMenu {
			get {
                return this.dataBaseMenu;
			}
		}
		
		public System.Windows.Forms.ContextMenuStrip ToolsMenu {
			get {
                return this.toolsMenu;
			}
		}
		
		public object Connection {
            get
            {
                return this.sqlConnection;
            }
            set
            {
                this.sqlConnection = (SqlConnection)value;
            }
		}
		
		public System.Windows.Forms.ImageList ImageList {
			get {
                return imageList1;
			}
		}
		
		
		public System.Drawing.Color TreeViewColor {
			get {
				throw new NotImplementedException();
			}
		}
		
		public CodeCompletionProvider CodeCompletionProvider {
            get { return codeCompletionProvider; }
            set { codeCompletionProvider = (SQLServerCodeCompletionProvider)value; }
		}
		
		public System.ComponentModel.IComponent MenuManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string GetConnectionSource()
		{
            return this.sqlConnection.ServerVersion;
		}
		
		public string GetDDLInformation(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public string GetNewFunctionQuery(string dataBase, string tableName, string name)
		{
			throw new NotImplementedException();
		}
		
		public string GetNewStoredProcedureQuery(string dataBase, string tableName, string name)
		{
			throw new NotImplementedException();
		}
		
		public string GetNewTriggerQuery(string dataBase, string tableName, string name)
		{
			throw new NotImplementedException();
		}
		
		public string GetNewViewQuery(string dataBase, string tableName, string name)
		{
			throw new NotImplementedException();
		}
		
		public System.Windows.Forms.ContextMenuStrip GetMainMenu()
		{
            return serverMenu;
		}
		
		public void DeleteSelectedRowsTable(DataUserView dataUserView)
		{
			throw new NotImplementedException();
		}
		
		public void Disconnect()
		{
			throw new NotImplementedException();
		}
		
		
		public void Reconnect()
		{
			throw new NotImplementedException();
		}
		
		public void RenameTable(string dataBase, string tableName, string newName)
		{
			throw new NotImplementedException();
		}
		
		public void SaveTable(string tableName, DataUserView dataUserView)
		{
			throw new NotImplementedException();
		}
		
		public void AddNewRowToBoundedTable(System.Windows.Forms.DataGridView dgTableView)
		{
			throw new NotImplementedException();
		}

        public void BindDataUserView(DataUserView dataUserView, TreeNode tableNode, string limitT1, string limitT2, bool useLimits, DataGridViewColumn sortColumn)
		{
            if (browserIsBusy)
                return;
            DataGridView dgTableView = dataUserView.DGTableView;
            List<string> TablePrimaryKeys = dataUserView.TablePrimaryKeys;
            object[] tag = (object[])tableNode.Tag;
            SQLNodeType type = (SQLNodeType)tag[1];
            if (type != SQLNodeType.Table)
                return;
            string dataBase = tableNode.Parent.Text;
            string tableName = tableNode.Text;
            if (dataBase == null || tableName == null)
                return;
            ready = new ManualResetEvent(false);
            totalRows = 0;
            List<SqlDataGridViewRow> cachedRows = new List<SqlDataGridViewRow>();
            dgTableView.Tag = cachedRows;
            tableIsBinding = true;



            dgTableView.AutoGenerateColumns = false;
            dgTableView.Rows.Clear();

            if (sortColumn == null)
            {

                TablePrimaryKeys.Clear();
                updateNodeTimer.Enabled = true;
                updateNodeTimer.Start();
                oldImageIndex = tableNode.ImageIndex;
                tableNode.ImageIndex = 12;
                browserIsBusy = true;
                dgTableView.Columns.Clear();

                dgTableView.CellBeginEdit -= dgTableView_CellBeginEdit;
                dgTableView.CellFormatting -= dgTableView_CellFormatting;
                dgTableView.CellContentClick -= dgTableView_CellContentClick;
                dgTableView.CellClick -= dgTableView_CellClick;
                dgTableView.CellBeginEdit -= dgTableView_CellBeginEdit;
                dgTableView.CellValueNeeded -= dgTableView_CellValueNeeded;
                dgTableView.CellValuePushed -= dgTableView_CellValuePushed;
                dgTableView.MouseUp -= dgTableView_MouseUp;

                dgTableView.CellBeginEdit += new DataGridViewCellCancelEventHandler(dgTableView_CellBeginEdit);
                dgTableView.CellFormatting += new DataGridViewCellFormattingEventHandler(dgTableView_CellFormatting);
                dgTableView.CellContentClick += new DataGridViewCellEventHandler(dgTableView_CellContentClick);
                dgTableView.CellClick += new DataGridViewCellEventHandler(dgTableView_CellClick);
                dgTableView.CellValueNeeded += new DataGridViewCellValueEventHandler(dgTableView_CellValueNeeded);
                dgTableView.CellValuePushed += new DataGridViewCellValueEventHandler(dgTableView_CellValuePushed);
                dgTableView.MouseUp += new MouseEventHandler(dgTableView_MouseUp);
                dataUserView.DataBase = dataBase;
                dataUserView.TableName = tableName;

                DataTable dt = new DataTable();

                //QueryResult qR = GetColumnInformation(dataBase, tableName);
                QueryResult qR = new QueryResult(null);
                SqlCommand cmd = new SqlCommand();
                string s = "";
                SqlDataAdapter da = new SqlDataAdapter();
                try
                {
                    s = "SELECT *FROM INFORMATION_SCHEMA.Tables T JOIN INFORMATION_SCHEMA.Columns C ON T.TABLE_NAME = C.TABLE_NAME WHERE T.TABLE_NAME = '" + tableName + "' ORDER BY C.COLUMN_NAME";
                    cmd = new SqlCommand(s, sqlConnection);
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    da.Dispose();
                    cmd.Dispose();
                    qR = new QueryResult(dt);


                    s = @"SELECT        c.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS pk INNER JOIN
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS c ON pk.TABLE_NAME = c.TABLE_NAME AND pk.CONSTRAINT_NAME = c.CONSTRAINT_NAME
                WHERE (pk.TABLE_NAME = '" + tableName + "') AND (pk.CONSTRAINT_TYPE = 'PRIMARY KEY')";
                    cmd = new SqlCommand(s, sqlConnection);
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dtPk = new DataTable();
                    da.Fill(dtPk);
                    TablePrimaryKeys = new List<string>();
                    if (dtPk.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtPk.Rows.Count; i++)
                            TablePrimaryKeys.Add(dtPk.Rows[i]["COLUMN_NAME"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (this.TableBounded != null)
                        this.TableBounded(this, new TableBoundedArg(tableNode.Parent.Text, tableNode.Text, tableNode, qR.Message));
                    updateNodeTimer.Enabled = false;
                    updateNodeTimer.Stop();
                    currentNode.ImageIndex = oldImageIndex;
                    currentNode.SelectedImageIndex = currentNode.ImageIndex;
                    browserIsBusy = false;
                    return;
                }


                if (qR.Message != null)
                {
                    if (this.TableBounded != null)
                        this.TableBounded(this, new TableBoundedArg(tableNode.Parent.Text, tableNode.Text, tableNode, qR.Message));
                    return;
                }
                else
                {
                    dt = (DataTable)qR.Result;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    string fullType = dt.Rows[i]["COLUMN_NAME"].ToString().ToLower();
                    string mainType = dt.Rows[i]["DATA_TYPE"].ToString().ToLower(); ;//ExtractMainTypeFromFullType(fullType);
                    string columnName = dt.Rows[i]["COLUMN_NAME"].ToString().ToLower();
                    //handle text, set,enum and blob columns
                    switch (mainType.ToLower())
                    {
                        case "bit":
                        case "binary":
                        case "binary(n)":
                        case "varbinary":
                        case "varbinary(n)":
                        case "image":
                            DataGridViewButtonColumn c3 = new DataGridViewButtonColumn();
                            c3.DefaultCellStyle.NullValue = "(null)";
                            c3.Resizable = DataGridViewTriState.True;
                            c3.DataPropertyName = columnName;
                            c3.HeaderText = c3.DataPropertyName;
                            dgTableView.Columns.Add(c3);
                            c3.Tag = new object[] { mainType, null };
                            c3.Name = c3.HeaderText;
                            break;
                        case "ntext":
                        case "text":
                        case "tinytext":
                        case "mediumtext":
                        case "longtext":
                            DataGridViewButtonColumn c4 = new DataGridViewButtonColumn();
                            c4.Resizable = DataGridViewTriState.True;
                            c4.DefaultCellStyle.NullValue = "(null)";
                            c4.SortMode = DataGridViewColumnSortMode.NotSortable;
                            c4.DataPropertyName = columnName;
                            c4.HeaderText = c4.DataPropertyName;
                            c4.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            c4.Width = 200;
                            dgTableView.Columns.Add(c4);
                            c4.Tag = new object[] { mainType, null };
                            c4.Name = c4.HeaderText;
                            c4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                            c4.SortMode = DataGridViewColumnSortMode.NotSortable;
                            break;
                        case "date":
                        case "datetime":
                        case "timestamp":
                        case "time":
                            DataGridViewTextBoxColumn c5 = new DataGridViewTextBoxColumn();
                            c5.DefaultCellStyle.NullValue = "(null)";
                            c5.Frozen = false; ;
                            c5.Resizable = DataGridViewTriState.True;
                            c5.DataPropertyName = columnName;
                            c5.HeaderText = c5.DataPropertyName;
                            c5.Name = c5.HeaderText;
                            if (mainType == "datetime" || mainType == "date" || mainType == "timestamp" || mainType == "time")
                                c5.Tag = new object[] { mainType, null };
                            dgTableView.Columns.Add(c5);
                            break;
                        default:
                            DataGridViewTextBoxColumn c6 = new DataGridViewTextBoxColumn();
                            c6.DefaultCellStyle.NullValue = "(null)";
                            c6.Frozen = false; ;
                            c6.Resizable = DataGridViewTriState.True;
                            c6.DataPropertyName = columnName;
                            c6.HeaderText = c6.DataPropertyName;
                            c6.Name = c6.HeaderText;
                            c6.Tag = new object[] { mainType, null };
                            dgTableView.Columns.Add(c6);
                            break;
                    }
                }
                DataGridViewCheckBoxColumn chx = new DataGridViewCheckBoxColumn(false);
                chx.HeaderText = "Checked";
                chx.CellTemplate.Style.BackColor = SystemColors.InactiveCaption;
                dgTableView.Columns.Insert(0, chx);
            }
            string q = "select * from ["+ tableName + "]; ";
            if (sortColumn != null)
            {
                q += " order by `" + sortColumn.HeaderText + "` ";
                if (sortColumn.HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending)
                    q += "asc";
                else
                    if (sortColumn.HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Descending)
                        q += "desc";
            }

            DateTime start = DateTime.Now;
            workerBinder.RunWorkerAsync(new object[] { start, q, dgTableView,useLimits,limitT1, limitT2,tableName, sortColumn });
            tableIsBinding = false;
		}
		
		public void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox)
		{
			//throw new NotImplementedException();
		}
		
		public void SetObjectOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox)
		{
			//throw new NotImplementedException();
		}
		
		public void SetQueryInput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox)
		{
			//throw new NotImplementedException();
		}
		
		public void ShowAlterTableForm(string dataBase, string tableName)
		{
			using (AlterTableForm aTForm = new AlterTableForm( this.sqlConnection, dataBase, tableName, AlterTableForm.Mod.Alter))
            {
                aTForm.ShowDialog();
            }
		}
		
		public void ShowCreateTableForm(System.Windows.Forms.TreeNode node)
		{
			throw new NotImplementedException();
		}
		
		public void ShowExecuteForm(string fileName, System.Windows.Forms.Form mdiParent)
		{
			throw new NotImplementedException();
		}
		
		public void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
		{
			throw new NotImplementedException();
		}
		
		public void ShowExportTableForm(System.Windows.Forms.DataGridView dgView, string[] columns)
		{
			throw new NotImplementedException();
		}
		
		public void ShowExportTableForm(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public void ShowFlushForm()
		{
			throw new NotImplementedException();
		}
		
		public void ShowImportFromCSVForm(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public void ShowIndexesTableForm(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public void ShowNewDbForm(System.Windows.Forms.TreeNode node)
		{
			throw new NotImplementedException();
		}
		
		public void ShowReferenceTableForm(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public void ShowReorderColumnForm(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public void ShowStatusForm()
		{
			throw new NotImplementedException();
		}
		
		public void ShowTableDiagnosticForm()
		{
			throw new NotImplementedException();
		}
		
		public void ShowTableStatus(string dataBase, string table)
		{
			throw new NotImplementedException();
		}
		
    	public void ExecuteQuery(string query, DataUserView output)
		{
            DateTime start = DateTime.Now;
            output.DGResultView.RowCount = 0;
            output.DGResultView.SuspendLayout();
            output.DGResultView.Rows.Clear();
            output.DGResultView.RowCount = 0;
            output.DGResultView.ColumnCount = 0;
            workerQueryExecutor.RunWorkerAsync(new object[] { query, output, start });
		}
		
		
		public QueryResult GetColumnInformation(string dataBase, string tableName)
		{
            SqlCommand cmdC = new SqlCommand();
            try
            {
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    string s = "use " + dataBase + "; SELECT *FROM INFORMATION_SCHEMA.Tables T JOIN INFORMATION_SCHEMA.Columns C ON T.TABLE_NAME = C.TABLE_NAME WHERE T.TABLE_NAME = '" + tableName + "' ORDER BY C.COLUMN_NAME";
                    cmdC = new SqlCommand(s, sqlConnection);
                    SqlDataAdapter  da = new SqlDataAdapter(cmdC);
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    da.Dispose();
                    cmdC.Dispose();
                    return new QueryResult(dt);
                }
            }
            catch (SqlException ex)
            {
                cmdC.Dispose();
                return new QueryResult(ex.Message);
            }
		}
		
		public QueryResult SwitchDataBase(string dataBase)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetStoredProcedureDefinition(string dataBase, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetStoredProcedures(string dataBase)
		{
            string s = @"SELECT        name, object_id, principal_id, schema_id, parent_object_id, type, type_desc, create_date, modify_date, is_ms_shipped, is_published, is_schema_published, 
                         is_auto_executed, is_execution_replicated, is_repl_serializable_only, skips_repl_constraints
FROM            sys.procedures
ORDER BY name";
            SqlCommand cmd = new SqlCommand(s, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i]["name"].ToString();
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
		}
		
		public QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetTable(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetTableStatus(string dataBase)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetTriggerDefinition(string dataBase, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetTriggers(string dataBase)
		{
			string s = @"SELECT        name, OBJECT_NAME(parent_obj) AS Expr1
                        FROM            sys.sysobjects
                        WHERE        (xtype = 'TR')";
            SqlCommand cmd = new SqlCommand(s, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i]["name"].ToString();
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
		}
		
		public QueryResult GetViewDefinition(string dataBase, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetViews(string dataBase)
		{
            string s = "SELECT * FROM sys.views;";
            SqlCommand cmd = new SqlCommand(s, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i]["name"].ToString();
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
		}
		
		public QueryResult ListColumns(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult ListDatabases()
		{
            try
            {
                string[] rez = null;
                string s = "SELECT name   FROM master..sysdatabases   ORDER BY name;";
                using (DataTable dt = new DataTable())
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(s, sqlConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                            rez[i] = dt.Rows[i][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new QueryResult(QueryStatus.Error, ex.Message);
            }
		}

        public QueryResult ListTables(string dataBase)
		{
            string s = "use "+dataBase+";select * from sys.tables;";
            SqlCommand cmd = new SqlCommand(s, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rez[i] = dt.Rows[i][0].ToString();
                    }
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.Message);
            }
            catch (SqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.Message);
            }
		}
		
		public QueryResult TruncateDataBase(string dataBase)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult TruncateTable(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropDataBase(string dataBase)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropFunction(string database, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropProcedure(string database, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropTable(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropTrigger(string database, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult DropView(string database, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult EmptyTable(string dataBase, string tableName)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetFunctionDefinition(string dataBase, string name)
		{
			throw new NotImplementedException();
		}
		
		public QueryResult GetFunctions(string dataBase)
		{
			string s = @"SELECT        ROUTINE_NAME
FROM            INFORMATION_SCHEMA.ROUTINES
WHERE        (ROUTINE_TYPE = 'function')";
            SqlCommand cmd = new SqlCommand(s, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i]["ROUTINE_NAME"].ToString();
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
		}
		
		public QueryResult GetIndexInformation(string dataBase, string tableName)
		{
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (dataBase == null || tableName == null)
                    return null;
                string s = "sp_helpindex '" + tableName + "';";
                cmd = new SqlCommand(s, sqlConnection);
                using (DataTable dt = new DataTable())
                {
                    da = new SqlDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));

                    cmd.Dispose();
                    da.Dispose();
                    return new QueryResult(dt);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SqlException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
		}
		
		public System.Windows.Forms.TreeNode CreateMainTreeNode()
		{
            TreeNode nodS = new TreeNode(GetConnectionSource() + "/" + ServerAddress, 0, 0);
            nodS.Tag = new object[] { this, SQLServerNodeType.Server, SQLServerNodeStatus.Refreshed };
            nodS.ContextMenuStrip = this.GetMainMenu();
            nodS.ImageIndex = 0;
            nodS.SelectedImageIndex = 0;
            nodS.Nodes.Add("-");
            currentNode = nodS;
            return nodS;
		}


        private SQLServerCodeCompletionProvider GenerateCompletionData()
        {
            // We can return code-completion items like this:
            string s = @"ACCESSIBLE,0
ADD,0
ALL,0
ALTER,0
ANALYZE,0
AND,0
AS,0
ASC,0
ASENSITIVE,0
BEFORE,0
BETWEEN,0
BIGINT,1
BINARY,1
BLOB,1
BOTH,0
BY,0
CALL,0
CASCADE,0
CASE,0
CHANGE,0
CHAR,1
CHARACTER,1
CHECK,0
COLLATE,0
COLUMN,0
CONDITION,0
CONSTRAINT,0
CONTINUE,0
CONVERT,0
CREATE,0
CROSS,0
CURRENT_DATE,0
CURRENT_TIME,0
CURRENT_TIMESTAMP,0
CURRENT_USER,0
CURSOR,0
DATABASE,0
DATABASES,0
DAY_HOUR,0
DAY_MICROSECOND,0
DAY_MINUTE,0
DAY_SECOND,0
DEC,0
DECIMAL,0
DECLARE,0
DEFAULT,0
DELAYED,0
DELETE,0
DESC,0
DESCRIBE,0
DETERMINISTIC,0
DISTINCT,0
DISTINCTROW,0
DIV,0
DOUBLE,1
DROP,0
DUAL,0
EACH,0
ELSE,0
ELSEIF,0
ENCLOSED,0
ENUM,1
ESCAPED,0
EXISTS,0
EXIT,0
EXPLAIN,0
FALSE,0
FETCH,0
FLOAT,1
FLOAT4,1
FLOAT8,1
FOR,0
FORCE,0
FOREIGN,0
FROM,0
FULLTEXT,0
GRANT,0
GROUP,0
HAVING,0
HIGH_PRIORITY,0
HOUR_MICROSECOND,0
HOUR_MINUTE,0
HOUR_SECOND,0
IF,0
IGNORE,0
IN,0
INDEX,0
INFILE,0
INNER,0
INOUT,0
INSENSITIVE,0
INSERT,0
INT,1
INT1,1
INT2,1
INT3,1
INT4,1
INT8,1
INTEGER,1
INTERVAL,1
INTO,0
IS,0
ITERATE,0
JOIN,0
KEY,0
KEYS,0
KILL,0
LEADING,0
LEAVE,0
LEFT,0
LIKE,0
LIMIT,0
LINEAR,0
LINES,0
LOAD,0
LOCALTIME,0
LOCALTIMESTAMP,0
LOCK,0
LONG,1
LONGBLOB,1
LONGTEXT,1
LOOP,0
LOW_PRIORITY,0
MASTER_SSL_VERIFY_SERVER_CERT,0
MATCH,0
MAXVALUE,0
MEDIUMBLOB,1
MEDIUMINT,1
MEDIUMTEXT,1
MIDDLEINT,1
MINUTE_MICROSECOND,0
MINUTE_SECOND,0
MOD,0
MODIFIES,0
NATURAL,0
NOT,0
NO_WRITE_TO_BINLOG,0
NULL,0
NUMERIC,1
ON,0
OPTIMIZE,0
OPTION,0
OPTIONALLY,0
OR,0
ORDER,0
OUT,0
OUTER,0
OUTFILE,0
PRECISION,0
PRIMARY,0
PROCEDURE,0
PURGE,0
RANGE,0
READ,0
READS,0
READ_WRITE,0
REAL,1
REFERENCES,0
REGEXP,0
RELEASE,0
RENAME,0
REPEAT,0
REPLACE,0
REQUIRE,0
RESIGNAL,0
RESTRICT,0
RETURN,0
REVOKE,0
RIGHT,0
RLIKE,0
SCHEMA,0
SCHEMAS,0
SECOND_MICROSECOND,0
SELECT,0
SENSITIVE,0
SEPARATOR,0
SET,1
SHOW,0
SIGNAL,0
SMALLINT,1
SPATIAL,0
SPECIFIC,0
SQL,0
SQLEXCEPTION,0
SQLSTATE,0
SQLWARNING,0
SQL_BIG_RESULT,0
SQL_CALC_FOUND_ROWS,0
SQL_SMALL_RESULT,0
SSL,0
STARTING,0
STRAIGHT_JOIN,0
TABLE,0
TERMINATED,0
THEN,0
TINYBLOB,1
TINYINT,1
TINYTEXT,1
TO,0
TRAILING,0
TRIGGER,0
TRUE,0
UNDO,0
UNION,0
UNIQUE,0
UNLOCK,0
UNSIGNED,0
UPDATE,0
USAGE,0
USE,0
USING,0
UTC_DATE,0
UTC_TIME,0
UTC_TIMESTAMP,0
VALUES,0
VARBINARY,1
VARCHAR,1
VARCHARACTER,1
VARYING,0
WHEN,0
WHERE,0
WHILE,0
WITH,0
WRITE,0
XOR,0
YEAR_MONTH,0
ZEROFILL,0";
            string q = s.Replace("\r", "");
            string[] list = q.Trim('\r').Split('\n');
            List<ICompletionData> lc = new List<ICompletionData>();
            for (int i = 0; i < list.Length; i++)
            {
                string[] x = list[i].Split(',');
                lc.Add(new DefaultCompletionData(x[0], int.Parse(x[1])));
            }
            return new SQLServerCodeCompletionProvider(completionListIcons, lc);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] arg = (object[])e.Argument;
            string q = (string)arg[1];
            DataGridView dgTableView = (DataGridView)arg[2]; ////
            bool useLimits = (bool)arg[3];
            string limit1 = (string)arg[4];
            string limit2 = (string)arg[5];
            string table = (string)arg[6];
            DataGridViewColumn sortColumn = (DataGridViewColumn)arg[7];
            //SqlDataAdapter = new SqlDataAdapter(q, sqlConnection);
            SqlDataReader reader = null;
            DataTable data = new DataTable();
            List<SqlDataGridViewRow> rez = new List<SqlDataGridViewRow>();
            SqlCommand cmd = new SqlCommand();
            if (!useLimits)
            {
                try
                {
                    if (sortColumn != null)
                    {
                        object[] oType = (object[])sortColumn.Tag;
                        string type = oType[0].ToString().ToLower();
                        if (type != "ntext" && type != "text" && type != "image")
                            q += " order by [" + sortColumn.HeaderText + "] " + (sortColumn.HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending ? "asc" : "desc") + " "; 
                    }
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    cmd = new SqlCommand(q, sqlConnection);
                    reader = cmd.ExecuteReader();
                    int i = 0;
                    object[] temp = new object[reader.FieldCount];
                    while (reader.Read())
                    {
                        temp = new object[reader.FieldCount];
                        reader.GetValues(temp);
                        rez.Add( new SqlDataGridViewRow(temp));
                        i++;
                       
                    }
                    totalRows = i;
                    reader.Close();
                    e.Result = new object[] { q, rez, dgTableView, null };
                }
                catch (Exception ex)
                {
                    e.Result = new object[] { q, null, dgTableView, ex.Message };
                    if (reader != null)
                        reader.Close();
                    return;
                }
            }
            else
            {
                int l1 = int.Parse(limit1);
                int l2 = int.Parse(limit2);
                string s = @"DECLARE MyCursor SCROLL CURSOR FOR select * from "+table;

                if (sortColumn != null)
                {
                    object[] oType = (object[])sortColumn.Tag;
                    string type = oType[0].ToString().ToLower();
                    if(type!="ntext"&&type!="text" &&type!="image")
                    s += " order by [" + sortColumn.HeaderText + "] " + (sortColumn.HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending ? "asc" : "desc") +" ";
                }
                    if(l1>0)
                    s+=@" OPEN MyCursor FETCH ABSOLUTE "+(l1).ToString()+" FROM MyCursor";
                else
                    s += @" OPEN MyCursor FETCH FROM MyCursor";

                cmd = new SqlCommand(s, sqlConnection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<SqlDataGridViewRow> list = new List<SqlDataGridViewRow>();
                if(dt.Rows.Count>0)
                {
                    list.Add(new SqlDataGridViewRow(dt.Rows[0].ItemArray));

                List<string> test = new List<string>();

                    for (int i = 0; i < l2 ; i++)
                    {
                        cmd.CommandText = "FETCH FROM MyCursor";
                        da = new SqlDataAdapter(cmd);
                        dt = new DataTable();

                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                                list.Add(new SqlDataGridViewRow(dt.Rows[0].ItemArray));
                    }
                }
                cmd = new SqlCommand("CLOSE MyCursor; DEALLOCATE MyCursor;", sqlConnection);
                cmd.ExecuteNonQuery();
                //workerBinder.ReportProgress(0, new object[] { dgTableView, rez });
                e.Result = new object[] { q, list, dgTableView, null };
            }

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string error = null;
            object[] rez = (object[])e.Result;
            DataGridView dgTableView = (DataGridView)rez[2];
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            cachedRows = (List<SqlDataGridViewRow>)rez[1];
            dgTableView.Tag = cachedRows;
            totalRows = cachedRows.Count;
            if (totalRows > 0)
                dgTableView.RowCount = cachedRows.Count;
            if (rez[3] != null)
                error = (string)rez[3];
            if (this.TableBounded != null)
                this.TableBounded(this, new TableBoundedArg(currentDataBase, CurrentTable, currentNode, error));
            DataGridView dataGrid = (DataGridView)rez[2];
            dgTableView.Tag = cachedRows;
            updateNodeTimer.Enabled = false;
            updateNodeTimer.Stop();
            currentNode.ImageIndex = oldImageIndex;
            currentNode.SelectedImageIndex = currentNode.ImageIndex;
            dataGrid.ResumeLayout();
            browserIsBusy = false;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string error = "";
            try
            {
                object[] argp = (object[])e.UserState;
                DataGridView dgTableView = (DataGridView)argp[0];
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                List<object[]> arg = (List<object[]>)argp[1];
                if (arg != null)
                {
                    for (int i = 0; i < arg.Count; i++)
                    {
                        for (int j = 0; j < arg[i].Length; j++)
                        {
                            if (arg[i][j].GetType() == typeof(DateTime))
                            {

                                DateTime dt = (DateTime)arg[i][j];
                                if (arg[i][j].ToString().Length > 10)
                                    // datetime, timestamp
                                    arg[i][j] = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                                else
                                    //only date
                                    arg[i][j] = string.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day);
                            }
                        }
                        cachedRows.Add(new SqlDataGridViewRow(arg[i]));
                    }
                    ready.Set();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                ready.Set(); //return
            }
        }

        #region queryExecution && RESULT table

        protected override void workerQueryExecutor_DoWork(object sender, DoWorkEventArgs e)
        {
            noOfRowsReturnedFromSelectQuery = 0;
            object[] arg = (object[])e.Argument;
            string q = (string)arg[0];
            DataUserView output = (DataUserView)arg[1];
            DataTable dt = new DataTable();
            DateTime start = (DateTime)arg[2];
            Encoding enc = new UTF7Encoding(false);
            byte[] xxx = Encoding.Default.GetBytes(q);

            string x = enc.GetString(xxx);
            char[] fff = x.ToCharArray();
            byte[] yyy = Encoding.Convert(Encoding.ASCII, enc, xxx, 0, xxx.Length);
            string a = enc.GetString(yyy);
            SqlDataAdapter da = new SqlDataAdapter(q, sqlConnection);

            //MySqlCommand cmd = da.SelectCommand;
            //cmd = new MySqlCommand("set names utf8;", MySqlConnection);
            string errMess = null;
            try
            {
                da.Fill(dt);
                //cmd = new MySqlCommand(q, MySqlConnection);
                // cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errMess = ex.Message;
            }
            noOfRowsReturnedFromSelectQuery = dt.Rows.Count;
            e.Result = new object[] { dt, output, start, q, noOfRowsReturnedFromSelectQuery, errMess };
            da.Dispose();
        }

       


        #endregion



        protected override List<TreeNode> RefreshServer(TreeNode node)
        {
            List<TreeNode> ret = new List<TreeNode>();
            ret.Add(node);
            QueryResult qR = this.ListDatabases();
            if (qR.Message != null)
            {
                MessageBox.Show(qR.Message, "Error " + qR.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string[] dataBases = (string[])qR.Result;
                for (int i = 0; i < dataBases.Length; i++)
                {
                    TreeNode nod = new TreeNode(dataBases[i]);
                    nod.Tag = new object[3] { this, SQLNodeType.Database, SQLNodeStatus.Unset };
                    nod.ImageIndex = 1;
                    nod.SelectedImageIndex = 1;
                    nod.Nodes.Add("-");
                    nod.ContextMenuStrip = dataBaseMenu;
                    ret.Add(nod);
                }
            }
            return ret;
        }
        protected override  List<TreeNode> RefreshDataBase(TreeNode dbNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            string database = dbNode.Text;
            codeCompletionProvider.AddDataBase(database);
            ret.Add(dbNode);
            string[] tables = new string[] { };
            using (SqlDataAdapter da = new SqlDataAdapter())
            {
                using (QueryResult qR2 = ListTables(dbNode.Text))
                {
                    if (qR2.Message != null)
                    {
                        MessageBox.Show(qR2.Message, "Error " + qR2.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    else
                    {
                        tables = (string[])qR2.Result;
                        if (tables != null)
                        {
                            codeCompletionProvider.AddTableCompletionToDB(database, tables);
                            for (int i = 0; i < tables.Length; i++)
                            {
                                TreeNode nod = new TreeNode(tables[i], 2, 2);
                                nod.Tag = new object[] { this, SQLServerNodeType.Table, SQLServerNodeStatus.Unset };
                                nod.ContextMenuStrip = tableMenu;
                                TreeNode cNode = new TreeNode("Columns", 3, 3);
                                cNode.Tag = new object[] { this, SQLServerNodeType.Column, SQLServerNodeStatus.Unset };
                                cNode.Nodes.Add("-");
                                nod.Nodes.Add(cNode);
                                TreeNode iNode = new TreeNode("Index", 3, 3);
                                iNode.Tag = new object[] { this, SQLServerNodeType.Index, SQLServerNodeStatus.Unset };
                                iNode.Nodes.Add("-");
                                nod.Nodes.Add(iNode);
                                ret.Add(nod);

                                SqlCommand cmdC = new SqlCommand();
                                using (DataTable dt = new DataTable())
                                {
                                    string s = "use " + dbNode.Text + "; SELECT *FROM INFORMATION_SCHEMA.Tables T JOIN INFORMATION_SCHEMA.Columns C ON T.TABLE_NAME = C.TABLE_NAME WHERE T.TABLE_NAME = '"+tables[i]+"' ORDER BY C.COLUMN_NAME";
                                    cmdC = new SqlCommand(s, sqlConnection);
                                    da.SelectCommand = cmdC;
                                    da.Fill(dt);
                                    string[] cols = new string[] { };
                                    if (dt.Rows.Count > 0)
                                    {
                                        cols = new string[dt.Rows.Count];
                                        for (int k = 0; k < dt.Rows.Count; k++)
                                        {
                                            cols[k] = dt.Rows[k]["column_name"].ToString();
                                        }
                                        codeCompletionProvider.AddColumnCompletionToTableDB(database, tables[i], cols);
                                    }
                                }
                            }
                        }
                    }
                    TreeNode vNode = new TreeNode("Views", 3, 3);
                    vNode.Tag = new object[] { this, SQLServerNodeType.View, SQLServerNodeStatus.Unset };
                    vNode.ContextMenuStrip = viewMenu;
                    vNode.Nodes.Add("-");
                    TreeNode pNode = new TreeNode("Procedures", 3, 3);
                    pNode.ContextMenuStrip = procedureMenu;
                    pNode.Tag = new object[] { this, SQLServerNodeType.Procedure, SQLServerNodeStatus.Unset };
                    pNode.Nodes.Add("-");
                    TreeNode fNode = new TreeNode("Functions", 3, 3);
                    fNode.ContextMenuStrip = functionMenu;
                    fNode.Tag = new object[] { this, SQLServerNodeType.Function, SQLServerNodeStatus.Unset };
                    fNode.Nodes.Add("-");
                    TreeNode tNode = new TreeNode("Triggers", 3, 3);
                    tNode.ContextMenuStrip = triggerMenu;
                    tNode.Tag = new object[] { this, SQLServerNodeType.Trigger, SQLServerNodeStatus.Unset };
                    tNode.Nodes.Add("-");
                    ret.Add(vNode);
                    ret.Add(pNode);
                    ret.Add(fNode);
                    ret.Add(tNode);
                }
            }
            return ret;
        }

        protected override List<TreeNode> RefreshTableNode(TreeNode tableNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            ret.Add(tableNode);
            DataTable cols = new DataTable();
            QueryResult qRc = GetColumnInformation(tableNode.Parent.Text, tableNode.Text);
            if (qRc.Message != null)
            {
                MessageBox.Show(qRc.Message, "Error " + qRc.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                cols = (DataTable)qRc.Result;
            DataTable indexes = new DataTable();
            QueryResult qRind = GetIndexInformation(tableNode.Parent.Text, tableNode.Text);
            if (qRind.ErrorNo != 0)
            {
                MessageBox.Show(qRind.Message, "Error " + qRind.ErrorNo, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
                indexes = (DataTable)qRind.Result;
            TreeNode cNode = new TreeNode("Columns", 3, 3);
            cNode.Tag = new object[] { this, SQLServerNodeType.Column, SQLServerNodeStatus.Refreshed };
            for (int i = 0; i < cols.Rows.Count; i++)
            {
                string id = cols.Rows[i]["column_name"].ToString();
                string type = cols.Rows[i]["data_type"].ToString();
                string nul = cols.Rows[i]["is_nullable"].ToString().ToUpper() == "YES" ? "null" : "not null";
                TreeNode nodCol = new TreeNode(id + " [" + type + " , " + nul + "]", 7, 7);
                nodCol.Tag = new object[] { this, SQLServerNodeType.Column, SQLServerNodeStatus.Refreshed };
                cNode.Nodes.Add(nodCol);
            }
            ret.Add(cNode);

            TreeNode iNode = new TreeNode("Index", 3, 3);
            for (int i = 0; i < indexes.Rows.Count; i++)
            {
                string inx = indexes.Rows[i]["Index_Name"].ToString();
                string cindex = indexes.Rows[i]["index_keys"].ToString();
                string display = inx + "[";
                string[] keys = cindex.Split(',');
                for (int k = 0; k < keys.Length; k++)
                {
                    keys[k] = keys[k].TrimEnd('(', '-', ')');
                    if (k != 0)
                        display += ",";
                    display += keys[k];
                }
                display += "]";
                TreeNode nodInd = new TreeNode(display, 8, 8);
                nodInd.Tag = new object[] { this, SQLNodeType.Index, SQLNodeStatus.Refreshed };
                iNode.Nodes.Add(nodInd);
            }
            iNode.Tag = new object[] { this, SQLNodeType.Index, SQLNodeStatus.Refreshed };
            ret.Add(iNode);
            return ret;
        }

        protected override List<TreeNode> RefreshViews(TreeNode viewNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            ret.Add(viewNode);
            viewNode.ContextMenuStrip = viewMenu;
            using (QueryResult qRv = GetViews(viewNode.Parent.Text))
            {
                if (qRv.Message != null)
                {
                    MessageBox.Show(qRv.Message, "Error " + qRv.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string[] views = (string[])qRv.Result;
                    if (views != null)
                        for (int i = 0; i < views.Length; i++)
                        {
                            TreeNode vNode = new TreeNode(views[i], 4, 4);
                            vNode.Tag = new object[] { this, SQLServerNodeType.View, SQLServerNodeStatus.Refreshed };
                            vNode.ContextMenuStrip = viewMenu;
                            ret.Add(vNode);
                        }
                }
            }
            return ret;
        }

        protected override List<TreeNode> RefreshProcedures(TreeNode procNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            ret.Add(procNode);
            using (QueryResult qRv = this.GetStoredProcedures(procNode.Parent.Text))
            {
                if (qRv.Message != null)
                {
                    MessageBox.Show(qRv.Message, "Error " + qRv.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string[] proc = (string[])qRv.Result;
                    if (proc != null)
                        for (int i = 0; i < proc.Length; i++)
                        {
                            TreeNode node = new TreeNode(proc[i], 5, 5);
                            node.Tag = new object[] { this, SQLServerNodeType.Procedure, SQLServerNodeStatus.Refreshed };
                            node.ContextMenuStrip = procedureMenu;
                            ret.Add(node);
                        }
                }
            }
            return ret;
        }

        protected override List<TreeNode> RefreshFunctions(TreeNode funcNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            funcNode.ContextMenuStrip = functionMenu;
            ret.Add(funcNode);
            using (QueryResult qRv = this.GetFunctions(funcNode.Parent.Text))
            {
                if (qRv.Message != null)
                {
                    MessageBox.Show(qRv.Message, "Error " + qRv.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string[] func = (string[])qRv.Result;
                    if (func != null)
                        for (int i = 0; i < func.Length; i++)
                        {
                            TreeNode node = new TreeNode(func[i], 6, 6);
                            node.Tag = new object[] { this, SQLServerNodeType.Function, SQLServerNodeStatus.Refreshed };
                            node.ContextMenuStrip = functionMenu;
                            ret.Add(node);
                        }
                }
            }
            return ret;
        }

        protected override List<TreeNode> RefreshTriggers(TreeNode triggNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            triggNode.ContextMenuStrip = triggerMenu;
            ret.Add(triggNode);
            using (QueryResult qRv = this.GetTriggers(triggNode.Parent.Text))
            {
                if (qRv.Message != null)
                {
                    MessageBox.Show(qRv.Message, "Error " + qRv.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string[] trigg = (string[])qRv.Result;
                    if (trigg != null)
                        for (int i = 0; i < trigg.Length; i++)
                        {
                            TreeNode node = new TreeNode(trigg[i], 9, 9);
                            node.ContextMenuStrip = triggerMenu;
                            node.Tag = new object[] { this, SQLServerNodeType.Trigger, SQLServerNodeStatus.Refreshed };
                            ret.Add(node);
                        }
                }
            }
            return ret;
        }


        private void InitializeComponent()
        {
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLServerBrowser));
        	this.serverMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refresServerhMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
        	this.newDatabaseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.restoreDatabaseFromSqlDumpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
        	this.reconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.disconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.tableMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshTabletoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.alterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.manageIndexesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.viewReletionshipforeignKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.advancedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.reorderColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.truncateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
        	this.dropTabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.emptyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
        	this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.asSqldumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.asCSVFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
        	this.createTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.copyDDLDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.viewDDLDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.functionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.alterFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.dropFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.alterViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.dropViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
        	this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.triggerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshTriggerStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
        	this.dataBaseMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.alterDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.dropDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.truncateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.emptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
        	this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
        	this.createSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.procedureMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.refreshProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.alterProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.dropProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.serverStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.tableDiangosticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.flushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.completionListIcons = new System.Windows.Forms.ImageList(this.components);
        	this.imageList1 = new System.Windows.Forms.ImageList(this.components);
        	this.serverMenu.SuspendLayout();
        	this.tableMenu.SuspendLayout();
        	this.functionMenu.SuspendLayout();
        	this.viewMenu.SuspendLayout();
        	this.triggerMenu.SuspendLayout();
        	this.dataBaseMenu.SuspendLayout();
        	this.procedureMenu.SuspendLayout();
        	this.toolsMenu.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// serverMenu
        	// 
        	this.serverMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refresServerhMenuItem,
        	        	        	this.newQueryMenuItem,
        	        	        	this.toolStripMenuItem13,
        	        	        	this.newDatabaseMenuItem,
        	        	        	this.restoreDatabaseFromSqlDumpMenuItem,
        	        	        	this.toolStripMenuItem6,
        	        	        	this.reconnectMenuItem,
        	        	        	this.disconnectMenuItem});
        	this.serverMenu.Name = "serverMenu";
        	this.serverMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.serverMenu.Size = new System.Drawing.Size(246, 148);
        	// 
        	// refresServerhMenuItem
        	// 
        	this.refresServerhMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refresServerhMenuItem.Image")));
        	this.refresServerhMenuItem.Name = "refresServerhMenuItem";
        	this.refresServerhMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.refresServerhMenuItem.Text = "Refresh";
        	// 
        	// newQueryMenuItem
        	// 
        	this.newQueryMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newQueryMenuItem.Image")));
        	this.newQueryMenuItem.Name = "newQueryMenuItem";
        	this.newQueryMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.newQueryMenuItem.Text = "New query editor";
        	// 
        	// toolStripMenuItem13
        	// 
        	this.toolStripMenuItem13.Name = "toolStripMenuItem13";
        	this.toolStripMenuItem13.Size = new System.Drawing.Size(242, 6);
        	// 
        	// newDatabaseMenuItem
        	// 
        	this.newDatabaseMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newDatabaseMenuItem.Image")));
        	this.newDatabaseMenuItem.Name = "newDatabaseMenuItem";
        	this.newDatabaseMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.newDatabaseMenuItem.Text = "New database";
        	// 
        	// restoreDatabaseFromSqlDumpMenuItem
        	// 
        	this.restoreDatabaseFromSqlDumpMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreDatabaseFromSqlDumpMenuItem.Image")));
        	this.restoreDatabaseFromSqlDumpMenuItem.Name = "restoreDatabaseFromSqlDumpMenuItem";
        	this.restoreDatabaseFromSqlDumpMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.restoreDatabaseFromSqlDumpMenuItem.Text = "Restore database from sql dump";
        	// 
        	// toolStripMenuItem6
        	// 
        	this.toolStripMenuItem6.Name = "toolStripMenuItem6";
        	this.toolStripMenuItem6.Size = new System.Drawing.Size(242, 6);
        	// 
        	// reconnectMenuItem
        	// 
        	this.reconnectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reconnectMenuItem.Image")));
        	this.reconnectMenuItem.Name = "reconnectMenuItem";
        	this.reconnectMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.reconnectMenuItem.Text = "Reconnect";
        	// 
        	// disconnectMenuItem
        	// 
        	this.disconnectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("disconnectMenuItem.Image")));
        	this.disconnectMenuItem.Name = "disconnectMenuItem";
        	this.disconnectMenuItem.Size = new System.Drawing.Size(245, 22);
        	this.disconnectMenuItem.Text = "Disconnect";
        	// 
        	// tableMenu
        	// 
        	this.tableMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshTabletoolStripMenuItem,
        	        	        	this.alterToolStripMenuItem,
        	        	        	this.manageIndexesToolStripMenuItem,
        	        	        	this.viewReletionshipforeignKeysToolStripMenuItem,
        	        	        	this.advancedToolStripMenuItem1,
        	        	        	this.toolStripMenuItem5,
        	        	        	this.dropTabelToolStripMenuItem,
        	        	        	this.emptyToolStripMenuItem1,
        	        	        	this.renameToolStripMenuItem1,
        	        	        	this.toolStripMenuItem3,
        	        	        	this.exportToolStripMenuItem,
        	        	        	this.importToolStripMenuItem1,
        	        	        	this.toolStripMenuItem4,
        	        	        	this.createTriggerToolStripMenuItem,
        	        	        	this.copyDDLDefinitionToolStripMenuItem,
        	        	        	this.viewDDLDefinitionToolStripMenuItem});
        	this.tableMenu.Name = "tableMenu";
        	this.tableMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.tableMenu.Size = new System.Drawing.Size(209, 330);
        	// 
        	// refreshTabletoolStripMenuItem
        	// 
        	this.refreshTabletoolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshTabletoolStripMenuItem.Image")));
        	this.refreshTabletoolStripMenuItem.Name = "refreshTabletoolStripMenuItem";
        	this.refreshTabletoolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.refreshTabletoolStripMenuItem.Text = "Refresh";
        	// 
        	// alterToolStripMenuItem
        	// 
        	this.alterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterToolStripMenuItem.Image")));
        	this.alterToolStripMenuItem.Name = "alterToolStripMenuItem";
        	this.alterToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.alterToolStripMenuItem.Text = "Alter";
        	this.alterToolStripMenuItem.Click += new System.EventHandler(this.AlterToolStripMenuItemClick);
        	// 
        	// manageIndexesToolStripMenuItem
        	// 
        	this.manageIndexesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("manageIndexesToolStripMenuItem.Image")));
        	this.manageIndexesToolStripMenuItem.Name = "manageIndexesToolStripMenuItem";
        	this.manageIndexesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.manageIndexesToolStripMenuItem.Text = "Indexes";
        	// 
        	// viewReletionshipforeignKeysToolStripMenuItem
        	// 
        	this.viewReletionshipforeignKeysToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewReletionshipforeignKeysToolStripMenuItem.Image")));
        	this.viewReletionshipforeignKeysToolStripMenuItem.Name = "viewReletionshipforeignKeysToolStripMenuItem";
        	this.viewReletionshipforeignKeysToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.viewReletionshipforeignKeysToolStripMenuItem.Text = "Reletionship/foreign keys";
        	// 
        	// advancedToolStripMenuItem1
        	// 
        	this.advancedToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.reorderColumnsToolStripMenuItem,
        	        	        	this.propertiesToolStripMenuItem,
        	        	        	this.truncateToolStripMenuItem1});
        	this.advancedToolStripMenuItem1.Name = "advancedToolStripMenuItem1";
        	this.advancedToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
        	this.advancedToolStripMenuItem1.Text = "Advanced";
        	// 
        	// reorderColumnsToolStripMenuItem
        	// 
        	this.reorderColumnsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reorderColumnsToolStripMenuItem.Image")));
        	this.reorderColumnsToolStripMenuItem.Name = "reorderColumnsToolStripMenuItem";
        	this.reorderColumnsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
        	this.reorderColumnsToolStripMenuItem.Text = "Reorder columns";
        	// 
        	// propertiesToolStripMenuItem
        	// 
        	this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
        	this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
        	this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
        	this.propertiesToolStripMenuItem.Text = "Properties";
        	// 
        	// truncateToolStripMenuItem1
        	// 
        	this.truncateToolStripMenuItem1.Name = "truncateToolStripMenuItem1";
        	this.truncateToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
        	this.truncateToolStripMenuItem1.Text = "Truncate";
        	// 
        	// toolStripMenuItem5
        	// 
        	this.toolStripMenuItem5.Name = "toolStripMenuItem5";
        	this.toolStripMenuItem5.Size = new System.Drawing.Size(205, 6);
        	// 
        	// dropTabelToolStripMenuItem
        	// 
        	this.dropTabelToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropTabelToolStripMenuItem.Image")));
        	this.dropTabelToolStripMenuItem.Name = "dropTabelToolStripMenuItem";
        	this.dropTabelToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.dropTabelToolStripMenuItem.Text = "Drop";
        	// 
        	// emptyToolStripMenuItem1
        	// 
        	this.emptyToolStripMenuItem1.Name = "emptyToolStripMenuItem1";
        	this.emptyToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
        	this.emptyToolStripMenuItem1.Text = "Empty";
        	// 
        	// renameToolStripMenuItem1
        	// 
        	this.renameToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem1.Image")));
        	this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
        	this.renameToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
        	this.renameToolStripMenuItem1.Text = "Rename";
        	// 
        	// toolStripMenuItem3
        	// 
        	this.toolStripMenuItem3.Name = "toolStripMenuItem3";
        	this.toolStripMenuItem3.Size = new System.Drawing.Size(205, 6);
        	// 
        	// exportToolStripMenuItem
        	// 
        	this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.asSqldumpToolStripMenuItem,
        	        	        	this.asCSVFileToolStripMenuItem});
        	this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
        	this.exportToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.exportToolStripMenuItem.Text = "Export";
        	// 
        	// asSqldumpToolStripMenuItem
        	// 
        	this.asSqldumpToolStripMenuItem.Name = "asSqldumpToolStripMenuItem";
        	this.asSqldumpToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
        	this.asSqldumpToolStripMenuItem.Text = "As SQL dump";
        	// 
        	// asCSVFileToolStripMenuItem
        	// 
        	this.asCSVFileToolStripMenuItem.Name = "asCSVFileToolStripMenuItem";
        	this.asCSVFileToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
        	this.asCSVFileToolStripMenuItem.Text = "As ...";
        	// 
        	// importToolStripMenuItem1
        	// 
        	this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
        	this.importToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
        	this.importToolStripMenuItem1.Text = "Import from CSV file";
        	// 
        	// toolStripMenuItem4
        	// 
        	this.toolStripMenuItem4.Name = "toolStripMenuItem4";
        	this.toolStripMenuItem4.Size = new System.Drawing.Size(205, 6);
        	// 
        	// createTriggerToolStripMenuItem
        	// 
        	this.createTriggerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createTriggerToolStripMenuItem.Image")));
        	this.createTriggerToolStripMenuItem.Name = "createTriggerToolStripMenuItem";
        	this.createTriggerToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.createTriggerToolStripMenuItem.Text = "Create Trigger";
        	// 
        	// copyDDLDefinitionToolStripMenuItem
        	// 
        	this.copyDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyDDLDefinitionToolStripMenuItem.Image")));
        	this.copyDDLDefinitionToolStripMenuItem.Name = "copyDDLDefinitionToolStripMenuItem";
        	this.copyDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.copyDDLDefinitionToolStripMenuItem.Text = "Copy DDL definition";
        	// 
        	// viewDDLDefinitionToolStripMenuItem
        	// 
        	this.viewDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewDDLDefinitionToolStripMenuItem.Image")));
        	this.viewDDLDefinitionToolStripMenuItem.Name = "viewDDLDefinitionToolStripMenuItem";
        	this.viewDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
        	this.viewDDLDefinitionToolStripMenuItem.Text = "View DDL definition";
        	// 
        	// functionMenu
        	// 
        	this.functionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshFunctionsToolStripMenuItem,
        	        	        	this.newFunctionToolStripMenuItem,
        	        	        	this.alterFunctionToolStripMenuItem,
        	        	        	this.dropFunctionToolStripMenuItem});
        	this.functionMenu.Name = "functionMenu";
        	this.functionMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.functionMenu.Size = new System.Drawing.Size(149, 92);
        	// 
        	// refreshFunctionsToolStripMenuItem
        	// 
        	this.refreshFunctionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshFunctionsToolStripMenuItem.Image")));
        	this.refreshFunctionsToolStripMenuItem.Name = "refreshFunctionsToolStripMenuItem";
        	this.refreshFunctionsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
        	this.refreshFunctionsToolStripMenuItem.Text = "Refresh";
        	// 
        	// newFunctionToolStripMenuItem
        	// 
        	this.newFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newFunctionToolStripMenuItem.Image")));
        	this.newFunctionToolStripMenuItem.Name = "newFunctionToolStripMenuItem";
        	this.newFunctionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
        	this.newFunctionToolStripMenuItem.Text = "New function";
        	// 
        	// alterFunctionToolStripMenuItem
        	// 
        	this.alterFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterFunctionToolStripMenuItem.Image")));
        	this.alterFunctionToolStripMenuItem.Name = "alterFunctionToolStripMenuItem";
        	this.alterFunctionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
        	this.alterFunctionToolStripMenuItem.Text = "Alter function";
        	// 
        	// dropFunctionToolStripMenuItem
        	// 
        	this.dropFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropFunctionToolStripMenuItem.Image")));
        	this.dropFunctionToolStripMenuItem.Name = "dropFunctionToolStripMenuItem";
        	this.dropFunctionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
        	this.dropFunctionToolStripMenuItem.Text = "Drop function";
        	// 
        	// viewMenu
        	// 
        	this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshViewsToolStripMenuItem,
        	        	        	this.newViewToolStripMenuItem,
        	        	        	this.alterViewToolStripMenuItem,
        	        	        	this.dropViewToolStripMenuItem,
        	        	        	this.toolStripMenuItem14,
        	        	        	this.showToolStripMenuItem});
        	this.viewMenu.Name = "viewMenu";
        	this.viewMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.viewMenu.Size = new System.Drawing.Size(128, 120);
        	// 
        	// refreshViewsToolStripMenuItem
        	// 
        	this.refreshViewsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshViewsToolStripMenuItem.Image")));
        	this.refreshViewsToolStripMenuItem.Name = "refreshViewsToolStripMenuItem";
        	this.refreshViewsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
        	this.refreshViewsToolStripMenuItem.Text = "Refresh";
        	// 
        	// newViewToolStripMenuItem
        	// 
        	this.newViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newViewToolStripMenuItem.Image")));
        	this.newViewToolStripMenuItem.Name = "newViewToolStripMenuItem";
        	this.newViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
        	this.newViewToolStripMenuItem.Text = "New view";
        	// 
        	// alterViewToolStripMenuItem
        	// 
        	this.alterViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterViewToolStripMenuItem.Image")));
        	this.alterViewToolStripMenuItem.Name = "alterViewToolStripMenuItem";
        	this.alterViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
        	this.alterViewToolStripMenuItem.Text = "Alter view";
        	// 
        	// dropViewToolStripMenuItem
        	// 
        	this.dropViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropViewToolStripMenuItem.Image")));
        	this.dropViewToolStripMenuItem.Name = "dropViewToolStripMenuItem";
        	this.dropViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
        	this.dropViewToolStripMenuItem.Text = "Drop view";
        	// 
        	// toolStripMenuItem14
        	// 
        	this.toolStripMenuItem14.Name = "toolStripMenuItem14";
        	this.toolStripMenuItem14.Size = new System.Drawing.Size(124, 6);
        	// 
        	// showToolStripMenuItem
        	// 
        	this.showToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showToolStripMenuItem.Image")));
        	this.showToolStripMenuItem.Name = "showToolStripMenuItem";
        	this.showToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
        	this.showToolStripMenuItem.Text = "Show";
        	// 
        	// triggerMenu
        	// 
        	this.triggerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshTriggerStripMenuItem,
        	        	        	this.toolStripMenuItem11,
        	        	        	this.toolStripMenuItem12});
        	this.triggerMenu.Name = "triggerMenu";
        	this.triggerMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.triggerMenu.Size = new System.Drawing.Size(139, 70);
        	// 
        	// refreshTriggerStripMenuItem
        	// 
        	this.refreshTriggerStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshTriggerStripMenuItem.Image")));
        	this.refreshTriggerStripMenuItem.Name = "refreshTriggerStripMenuItem";
        	this.refreshTriggerStripMenuItem.Size = new System.Drawing.Size(138, 22);
        	this.refreshTriggerStripMenuItem.Text = "Refresh";
        	// 
        	// toolStripMenuItem11
        	// 
        	this.toolStripMenuItem11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem11.Image")));
        	this.toolStripMenuItem11.Name = "toolStripMenuItem11";
        	this.toolStripMenuItem11.Size = new System.Drawing.Size(138, 22);
        	this.toolStripMenuItem11.Text = "Alter trigger";
        	// 
        	// toolStripMenuItem12
        	// 
        	this.toolStripMenuItem12.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem12.Image")));
        	this.toolStripMenuItem12.Name = "toolStripMenuItem12";
        	this.toolStripMenuItem12.Size = new System.Drawing.Size(138, 22);
        	this.toolStripMenuItem12.Text = "Drop trigger";
        	// 
        	// dataBaseMenu
        	// 
        	this.dataBaseMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshToolStripMenuItem,
        	        	        	this.createToolStripMenuItem,
        	        	        	this.alterDatabaseToolStripMenuItem,
        	        	        	this.advancedToolStripMenuItem,
        	        	        	this.toolStripMenuItem1,
        	        	        	this.backupToolStripMenuItem,
        	        	        	this.importToolStripMenuItem,
        	        	        	this.toolStripMenuItem2,
        	        	        	this.createSchemaToolStripMenuItem});
        	this.dataBaseMenu.Name = "dataBaseMenu";
        	this.dataBaseMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.dataBaseMenu.Size = new System.Drawing.Size(196, 170);
        	// 
        	// refreshToolStripMenuItem
        	// 
        	this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
        	this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
        	this.refreshToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.refreshToolStripMenuItem.Text = "Refresh";
        	// 
        	// createToolStripMenuItem
        	// 
        	this.createToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createToolStripMenuItem.Image")));
        	this.createToolStripMenuItem.Name = "createToolStripMenuItem";
        	this.createToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.createToolStripMenuItem.Text = "Create table";
        	// 
        	// alterDatabaseToolStripMenuItem
        	// 
        	this.alterDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterDatabaseToolStripMenuItem.Image")));
        	this.alterDatabaseToolStripMenuItem.Name = "alterDatabaseToolStripMenuItem";
        	this.alterDatabaseToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.alterDatabaseToolStripMenuItem.Text = "Alter";
        	// 
        	// advancedToolStripMenuItem
        	// 
        	this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.dropDatabaseToolStripMenuItem,
        	        	        	this.truncateToolStripMenuItem,
        	        	        	this.emptyToolStripMenuItem});
        	this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
        	this.advancedToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.advancedToolStripMenuItem.Text = "Advanced";
        	// 
        	// dropDatabaseToolStripMenuItem
        	// 
        	this.dropDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropDatabaseToolStripMenuItem.Image")));
        	this.dropDatabaseToolStripMenuItem.Name = "dropDatabaseToolStripMenuItem";
        	this.dropDatabaseToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        	this.dropDatabaseToolStripMenuItem.Text = "Drop database";
        	// 
        	// truncateToolStripMenuItem
        	// 
        	this.truncateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("truncateToolStripMenuItem.Image")));
        	this.truncateToolStripMenuItem.Name = "truncateToolStripMenuItem";
        	this.truncateToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        	this.truncateToolStripMenuItem.Text = "Truncate";
        	// 
        	// emptyToolStripMenuItem
        	// 
        	this.emptyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("emptyToolStripMenuItem.Image")));
        	this.emptyToolStripMenuItem.Name = "emptyToolStripMenuItem";
        	this.emptyToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        	this.emptyToolStripMenuItem.Text = "Empty";
        	// 
        	// toolStripMenuItem1
        	// 
        	this.toolStripMenuItem1.Name = "toolStripMenuItem1";
        	this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 6);
        	// 
        	// backupToolStripMenuItem
        	// 
        	this.backupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("backupToolStripMenuItem.Image")));
        	this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
        	this.backupToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.backupToolStripMenuItem.Text = "Backup as sql dump";
        	// 
        	// importToolStripMenuItem
        	// 
        	this.importToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importToolStripMenuItem.Image")));
        	this.importToolStripMenuItem.Name = "importToolStripMenuItem";
        	this.importToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.importToolStripMenuItem.Text = "Restore from sql dump";
        	// 
        	// toolStripMenuItem2
        	// 
        	this.toolStripMenuItem2.Name = "toolStripMenuItem2";
        	this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 6);
        	// 
        	// createSchemaToolStripMenuItem
        	// 
        	this.createSchemaToolStripMenuItem.Enabled = false;
        	this.createSchemaToolStripMenuItem.Name = "createSchemaToolStripMenuItem";
        	this.createSchemaToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        	this.createSchemaToolStripMenuItem.Text = "Create schema";
        	// 
        	// procedureMenu
        	// 
        	this.procedureMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.refreshProcToolStripMenuItem,
        	        	        	this.newProcToolStripMenuItem,
        	        	        	this.alterProcToolStripMenuItem,
        	        	        	this.dropProcToolStripMenuItem});
        	this.procedureMenu.Name = "procedureMenu";
        	this.procedureMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.procedureMenu.Size = new System.Drawing.Size(163, 92);
        	// 
        	// refreshProcToolStripMenuItem
        	// 
        	this.refreshProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshProcToolStripMenuItem.Image")));
        	this.refreshProcToolStripMenuItem.Name = "refreshProcToolStripMenuItem";
        	this.refreshProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        	this.refreshProcToolStripMenuItem.Text = "Refresh";
        	// 
        	// newProcToolStripMenuItem
        	// 
        	this.newProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProcToolStripMenuItem.Image")));
        	this.newProcToolStripMenuItem.Name = "newProcToolStripMenuItem";
        	this.newProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        	this.newProcToolStripMenuItem.Text = "New procedure";
        	// 
        	// alterProcToolStripMenuItem
        	// 
        	this.alterProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterProcToolStripMenuItem.Image")));
        	this.alterProcToolStripMenuItem.Name = "alterProcToolStripMenuItem";
        	this.alterProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        	this.alterProcToolStripMenuItem.Text = "Alter proccedure";
        	// 
        	// dropProcToolStripMenuItem
        	// 
        	this.dropProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropProcToolStripMenuItem.Image")));
        	this.dropProcToolStripMenuItem.Name = "dropProcToolStripMenuItem";
        	this.dropProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
        	this.dropProcToolStripMenuItem.Text = "Drop procedure";
        	// 
        	// toolsMenu
        	// 
        	this.toolsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.serverStatusToolStripMenuItem,
        	        	        	this.tableDiangosticToolStripMenuItem,
        	        	        	this.flushToolStripMenuItem});
        	this.toolsMenu.Name = "toolsMenu";
        	this.toolsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.toolsMenu.Size = new System.Drawing.Size(162, 70);
        	// 
        	// serverStatusToolStripMenuItem
        	// 
        	this.serverStatusToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("serverStatusToolStripMenuItem.Image")));
        	this.serverStatusToolStripMenuItem.Name = "serverStatusToolStripMenuItem";
        	this.serverStatusToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        	this.serverStatusToolStripMenuItem.Text = "Server status";
        	// 
        	// tableDiangosticToolStripMenuItem
        	// 
        	this.tableDiangosticToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tableDiangosticToolStripMenuItem.Image")));
        	this.tableDiangosticToolStripMenuItem.Name = "tableDiangosticToolStripMenuItem";
        	this.tableDiangosticToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        	this.tableDiangosticToolStripMenuItem.Text = "Table diagnostic";
        	// 
        	// flushToolStripMenuItem
        	// 
        	this.flushToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("flushToolStripMenuItem.Image")));
        	this.flushToolStripMenuItem.Name = "flushToolStripMenuItem";
        	this.flushToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        	this.flushToolStripMenuItem.Text = "Flush";
        	// 
        	// completionListIcons
        	// 
        	this.completionListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("completionListIcons.ImageStream")));
        	this.completionListIcons.TransparentColor = System.Drawing.Color.Transparent;
        	this.completionListIcons.Images.SetKeyName(0, "Icons.16x16.Method.png");
        	this.completionListIcons.Images.SetKeyName(1, "vxclass_icon.png");
        	this.completionListIcons.Images.SetKeyName(2, "table (1).png");
        	this.completionListIcons.Images.SetKeyName(3, "column.png");
        	// 
        	// imageList1
        	// 
        	this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
        	this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
        	this.imageList1.Images.SetKeyName(0, "database_mysql.png");
        	this.imageList1.Images.SetKeyName(1, "database (2).png");
        	this.imageList1.Images.SetKeyName(2, "table (1).png");
        	this.imageList1.Images.SetKeyName(3, "directory.png");
        	this.imageList1.Images.SetKeyName(4, "view (2).png");
        	this.imageList1.Images.SetKeyName(5, "procedure.png");
        	this.imageList1.Images.SetKeyName(6, "function.png");
        	this.imageList1.Images.SetKeyName(7, "column.png");
        	this.imageList1.Images.SetKeyName(8, "index.png");
        	this.imageList1.Images.SetKeyName(9, "trigger (2).png");
        	this.imageList1.Images.SetKeyName(10, "sql-query.png");
        	this.imageList1.Images.SetKeyName(11, "disconnect.png");
        	this.imageList1.Images.SetKeyName(12, "arrow_circle.png");
        	this.imageList1.Images.SetKeyName(13, "arrow_circle_90.png");
        	this.imageList1.Images.SetKeyName(14, "arrow_circle_180.png");
        	this.imageList1.Images.SetKeyName(15, "arrow_circle_270.png");
        	// 
        	// SQLServerBrowser
        	// 
        	this.Name = "SQLServerBrowser";
        	this.serverMenu.ResumeLayout(false);
        	this.tableMenu.ResumeLayout(false);
        	this.functionMenu.ResumeLayout(false);
        	this.viewMenu.ResumeLayout(false);
        	this.triggerMenu.ResumeLayout(false);
        	this.dataBaseMenu.ResumeLayout(false);
        	this.procedureMenu.ResumeLayout(false);
        	this.toolsMenu.ResumeLayout(false);
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.ImageList imageList1;
        
        

        
        void AlterToolStripMenuItemClick(object sender, EventArgs e)
        {
        	 if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowAlterTableForm(currentNode.Parent.Text, currentNode.Text);
        }
	}


    public enum SQLServerNodeType { Server, Database, Table, View, Procedure, Function, Trigger, Column, Index }
    public enum SQLServerNodeStatus { Refreshed, Unset }
}
