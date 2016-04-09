using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.SQLite;
using DBMS.core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System.Text.RegularExpressions;


namespace DBMS.SQLite
{
    public class SQLiteDBBrowser : DBBrowserControl, IDBBrowser
    {

        private SQLiteConnection  SQLiteConnection;
        private SQLiteDataAdapter SQLiteDataAdapter;
        private bool tableIsBinding = true;
        private long totalRows = 0;
        private long nrRowsExecSelect = 0;
        private IComponent menuManager;
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public Color TreeViewColor
        {
            get { return Color.FromArgb(220, 255, 220); }
        }


        private ContextMenuStrip tableMenu;
        public ContextMenuStrip ToolsMenu
        {
            get { return this.toolsMenu; }
        }
        private CodeCompletionProvider codeCompletionProvider;
        #region menu

        private IContainer components;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem alterToolStripMenuItem;
        private ToolStripMenuItem manageIndexesToolStripMenuItem;
        private ToolStripMenuItem viewReletionshipforeignKeysToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem1;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem dropTabelToolStripMenuItem;
        private ToolStripMenuItem emptyToolStripMenuItem1;
        private ToolStripMenuItem renameToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem createTriggerToolStripMenuItem;
        public ContextMenuStrip triggerMenu;
        private ToolStripMenuItem refreshTriggerMenuItem;
        private ToolStripMenuItem newTriggerToolStripMenuItem;
        private ToolStripMenuItem alterTriggerMenuItem;
        private ToolStripMenuItem dropTriggerMenuItem;
        public ContextMenuStrip viewMenu;
        private ToolStripMenuItem refreshViewsToolStripMenuItem;
        private ToolStripMenuItem newViewToolStripMenuItem;
        private ToolStripMenuItem alterViewToolStripMenuItem;
        private ToolStripMenuItem dropViewToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem14;
        private ToolStripMenuItem swowToolStripMenuItem;
        public ContextMenuStrip procedureMenu;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripSeparator toolStripMenuItem19;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem9;
        public ContextMenuStrip functionMenu;
        private ToolStripMenuItem toolStripMenuItem24;
        private ToolStripSeparator toolStripMenuItem23;
        private ToolStripMenuItem newFunctionToolStripMenuItem;
        private ToolStripMenuItem alterFunctionToolStripMenuItem;
        private ToolStripMenuItem dropFunctionToolStripMenuItem;
        private ImageList imageList1;
        public ContextMenuStrip dataBaseMenu;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem dropDatabaseToolStripMenuItem;
        private ToolStripMenuItem truncateToolStripMenuItem;
        private ToolStripMenuItem emptyToolStripMenuItem;
        private ToolStripMenuItem backupToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;

        public ContextMenuStrip serverMenu;
        private ToolStripMenuItem refreshMenuItem;
        private ToolStripMenuItem newQueryMenuItem;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem restoreDatabaseFromSqlDumpMenuItem;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem reconnectMenuItem;
        private ToolStripMenuItem disconnectMenuItem;
        private ContextMenuStrip toolsMenu;

        #endregion
        public ImageList completionListIcons;
        private ToolStripMenuItem attachDatabaseToolStripMenuItem;
        private ToolStripMenuItem deattachDatabaseToolStripMenuItem;
        private ToolStripMenuItem viewDDLDefinitionToolStripMenuItem;
        private ToolStripMenuItem copyDDLDefinitionToolStripMenuItem;
        public CodeCompletionProvider CodeCompletionProvider
        {
            get { return codeCompletionProvider; }
            set { codeCompletionProvider = value; }
        }
        public ImageList ImageList
        {
            get
            {
                return this.imageList1;
            }
        }
        public ImageList CompletionListIcons
        {
            get
            {
                return completionListIcons;
            }
        }
        public SQLiteDBBrowser(SQLiteConnection connection) : base()
        {
            history = new History();
         
            InitializeComponent();
            asyncOperation = AsyncOperationManager.CreateOperation(this);
            postCallBack = new SendOrPostCallback(OnQueryExecuted);
            TablePrimaryKeys = new List<string>();
            codeCompletionProvider = GenerateCompletionData();

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            workerBinder = new BackgroundWorker();
            workerBinder.WorkerSupportsCancellation = true;
            workerBinder.WorkerReportsProgress = true;
            workerBinder.DoWork += new DoWorkEventHandler(worker_DoWork);
            workerBinder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            workerBinder.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

            workerQueryExecutor = new BackgroundWorker();
            workerQueryExecutor.WorkerReportsProgress = false;
            workerQueryExecutor.DoWork+=new DoWorkEventHandler(workerQueryExecutor_DoWork);
            workerQueryExecutor.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(workerQueryExecutor_RunWorkerCompleted);

        }

        public IComponent MenuManager
        {
            get { return menuManager; }
        }



        #region DBBrowser Members

        public event TableBoundedHandler TableBounded;
        public event NewDataUserViewRequestedHandler NewDataUserViewRequested;
        public event DisconnectRequestedEventHandler DisconnectRequestedEvent;


       

        private List<string> tablePrimaryKeys;
        public List<string> TablePrimaryKeys
        {
            get { return tablePrimaryKeys; }
            set { tablePrimaryKeys = value; }
        }
       
        
        public bool TableHasPrimaryKeys
        {
            get { return tablePrimaryKeys.Count > 0; }
        }
        public ContextMenuStrip TableMenu
        {
            get { return this.tableMenu; }
        }
        public ContextMenuStrip DataBaseMenu
        {
            get { return this.dataBaseMenu; }
        }

        public   Object Connection
        {
            get
            {
                return this.SQLiteConnection;
            }
            set
            {
                this.SQLiteConnection = (SQLiteConnection)value;
            }
        }

        public   QueryResult ListDatabases()
        {
        	
        	using(SQLiteDataAdapter ad = new SQLiteDataAdapter("PRAGMA database_list;", SQLiteConnection))
        	{
        		DataTable dt = new DataTable();
        		ad.Fill(dt);
        		string [] ret = new string[dt.Rows.Count];
        		if(dt.Rows.Count>0)
        		{
        			for(int i=0;i<dt.Rows.Count;i++)
        			{
        				ret[i] = dt.Rows[i]["name"].ToString();
        			}
        			return  new QueryResult(ret);
        		}
        	}
            return new QueryResult(new string[] { "database" });

        }

        public   QueryResult GetViews(string database)
        {
            string s = "SELECT name FROM `" + database +"`.sqlite_master WHERE type = \"view\";";
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
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
                        rez[i] = dt.Rows[i][0].ToString();
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
            catch (SQLiteException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetFunctions(string database)
        {
            string s = "SELECT name FROM sqlite_master WHERE type = \"function\";"; 
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
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
                        rez[i] = dt.Rows[i][0].ToString();
                    cmd.Dispose();
                    da.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetStoredProcedures(string database)
        {
            string s = "SELECT name FROM sqlite_master WHERE type = \"procedure\";"; 
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i][0].ToString();
                    cmd.Dispose();
                    da.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetTriggers(string database)
        {
            string s = "SELECT name FROM `" + database +"`.sqlite_master WHERE type = \"trigger\";";
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
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
                        rez[i] = dt.Rows[i][0].ToString();
                    cmd.Dispose();
                    da.Dispose();
                    return new QueryResult(rez);
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new QueryResult(ex.Message);
            }
            finally
            {
                da.Dispose();
                cmd.Dispose();
            }
        }

        public   void ShowTableStatus(string database, string tableName)
        {

        }

        public   string GetConnectionSource()
        {
            return this.SQLiteConnection.ServerVersion;
        }

        public   QueryResult GetViewDefinition(string dataBase, string name)
        {
            string s = "SELECT sql FROM sqlite_master WHERE type = \"view\" and name =\""+name+"\";";
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            try
            {
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez =  dt.Rows[0][0].ToString();

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
            catch (SQLiteException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
        }

        public   QueryResult GetStoredProcedureDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE specific_name = '" + name + "' AND ROUTINE_SCHEMA='" + dataBase + "';";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(s, SQLiteConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        rez =@"DELIMITER $$

CREATE
    /*[DEFINER = { user | CURRENT_USER }]*/
    PROCEDURE `" + dataBase + "`.`" + name + @"`()
    /*LANGUAGE SQL
    | [NOT] DETERMINISTIC
    | { CONTAINS SQL | NO SQL | READS SQL DATA | MODIFIES SQL DATA }
    | SQL SECURITY { DEFINER | INVOKER }
    | COMMENT 'string'*/" + Environment.NewLine + rez + @"$$

DELIMITER ;";
                        return new QueryResult((object)rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetFunctionDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE specific_name = '" + name + "' AND ROUTINE_SCHEMA='" + dataBase + "';";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(s, SQLiteConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        rez =  @"DELIMITER $$
CREATE
/*[DEFINER = { user | CURRENT_USER }]*/
FUNCTION `"+dataBase+"`.`"+name+@"`()
RETURNS TYPE
/*LANGUAGE SQL
| [NOT] DETERMINISTIC
| { CONTAINS SQL | NO SQL | READS SQL DATA | MODIFIES SQL DATA }
| SQL SECURITY { DEFINER | INVOKER }
| COMMENT 'string'*/
    " + Environment.NewLine +rez +"$$" + Environment.NewLine+@"
DELIMITER ;";
                        return new QueryResult((object)rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetTriggerDefinition(string dataBase, string name)
        {
            string s = "SELECT sql FROM `" + dataBase +"`.sqlite_master WHERE type = \"trigger\" and name =\"" + name + "\";";
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            try
            {
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    if (dt.Rows.Count < 1)
                        return new QueryResult(null);
                    rez = dt.Rows[0][0].ToString();

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
            catch (SQLiteException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }

        }

        public   string GetNewTriggerQuery(string dataBase, string tableName, string name)
        {
            string s = @"CREATE
/*TEMP|TEMPORARY */
/*IF NOT EXISTS */
TRIGGER `" + name + @"`
BEFORE|AFTER|INSTEAD OF
DELETE|INSERT|UPDATE ON TABLE `"+tableName +@"`
BEGIN


END;";
            return s;
        }

        public   string GetNewFunctionQuery(string dataBase, string tableName, string name)
        {
            string s = @"DELIMITER $$
CREATE
/*[DEFINER = { user | CURRENT_USER }]*/
FUNCTION `"+dataBase+"`.`"+name+@"`()
RETURNS TYPE
/*LANGUAGE SQL
| [NOT] DETERMINISTIC
| { CONTAINS SQL | NO SQL | READS SQL DATA | MODIFIES SQL DATA }
| SQL SECURITY { DEFINER | INVOKER }
| COMMENT 'string'*/
    BEGIN

    END$$

DELIMITER ;";
            return s;
        }

        public   string GetNewStoredProcedureQuery(string dataBase, string tableName, string name)
        {

            string s = @"DELIMITER $$

CREATE
    /*[DEFINER = { user | CURRENT_USER }]*/
    PROCEDURE `"+dataBase+"`.`"+name+@"`()
    /*LANGUAGE SQL
    | [NOT] DETERMINISTIC
    | { CONTAINS SQL | NO SQL | READS SQL DATA | MODIFIES SQL DATA }
    | SQL SECURITY { DEFINER | INVOKER }
    | COMMENT 'string'*/
    BEGIN

    END$$

DELIMITER ;";
            return s;
        }

        public   string GetNewViewQuery(string dataBase, string tableName, string name)
        {
            string s = @"CREATE
/*TEMP|TEMPORARY */
/*IF NOT EXISTS */
VIEW `" + name + @"` 
AS
    (SELECT * FROM ...);";
            return s;
        }

        public   void ExecuteQuery(string query, DataUserView outPut)
        {
                DateTime start = DateTime.Now;
                outPut.DGResultView.RowCount = 0;
                outPut.DGResultView.SuspendLayout();
                outPut.DGResultView.Rows.Clear();
                outPut.DGResultView.RowCount = 0;
                outPut.DGResultView.ColumnCount = 0;
                workerQueryExecutor.RunWorkerAsync(new object[] { query, outPut,start});
        }

        public   QueryResult ListColumns(string dataBase, string tableName)
        {
            SQLiteDataAdapter da = new SQLiteDataAdapter() ;
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                if (tableName == null)
                    return null;
                using (DataTable dt = new DataTable())
                {
                    string s = "pragma `"+dataBase+"`.table_info (`" + tableName + "`);";
                    cmd = new SQLiteCommand(s, SQLiteConnection);
                    da = new SQLiteDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    string[] rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i]["name"].ToString();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult ListTables(string database)
        {
            string s = "SELECT name FROM \""+database+"\".sqlite_master WHERE type = \"table\"";
            SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

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
            catch (SQLiteException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (Exception ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
        }

        public   QueryResult SwitchDataBase(string dataBase)
        {
            try
            {
                string s = "use `" + dataBase + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, this.SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    return new QueryResult(null);
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (Exception ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public   Object GetOdbcConnection()
        {
            return this.SQLiteConnection;
        }

        public   void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl shtb)
        {
//            //this.history.SetOutputText(shtb);
//            this.historyText = shtb;

//            shtb.Seperators.Add(';');
//            shtb.Seperators.Add(' ');
//            shtb.Seperators.Add('\r');
//            shtb.Seperators.Add('\n');
//            shtb.Seperators.Add(',');
//            shtb.Seperators.Add('.');
//            shtb.Seperators.Add('-');
//            shtb.Seperators.Add('+');

//            //shtb.Seperators.Add('*');
//            //shtb.Seperators.Add('/');
//            shtb.FilterAutoComplete = false;
//            /*shtb.HighlightDescriptors.Add(new HighlightDescriptor("<", Color.Gray, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("<<", ">>", Color.DarkGreen, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));
//*/
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("create", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("update", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("delete", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("select", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("from", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true)); 
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("table", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("where", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("having", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("between", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("database", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("show", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("index", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("column", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("count", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("avg", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true)); 
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("min", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("max", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("int", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("double", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("varchar", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("default", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("null", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("enum", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));




//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("/*", "*/", Color.Magenta, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));


        }

        public   void SetObjectOutput(TextEditorControl shtb)
        {
            //this.objectText = shtb;
            //shtb.Seperators.Add(' ');
            //shtb.Seperators.Add('\r');
            //shtb.Seperators.Add('\n');
            //shtb.Seperators.Add(',');
            //shtb.Seperators.Add('.');
            //shtb.Seperators.Add('-');
            //shtb.Seperators.Add('+');
            //shtb.Seperators.Add('=');
            //shtb.Seperators.Add('(');
            //shtb.Seperators.Add(')');
            ////shtb.Seperators.Add('*');
            ////shtb.Seperators.Add('/');
            //shtb.FilterAutoComplete = false;
            ///*shtb.HighlightDescriptors.Add(new HighlightDescriptor("<", Color.Gray, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("<<", ">>", Color.DarkGreen, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));*/
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("'", "'", Color.Red, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));


            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("create", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("update", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("delete", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("select", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("from", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("table", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("where", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("having", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("between", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("database", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("show", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("index", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("column", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("on", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("cascade", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("foreign", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("constraint", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("count", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("avg", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("min", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("max", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("not", Color.Red, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("int", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("text", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("unique", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("primary", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("key", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("engine", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("Innodb", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("MyISAM", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("double", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("varchar", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("default", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("null", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("enum", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));




            //shtb.HighlightDescriptors.Add(new HighlightDescriptor("/*", "*/", Color.Magenta, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));

        }

        public   void SetQueryInput(TextEditorControl shtb)
        {
//            shtb.Seperators.Add(' ');
//            shtb.Seperators.Add('(');
//            shtb.Seperators.Add(')');
//            shtb.Seperators.Add('\r');
//            shtb.Seperators.Add('\n');
//            shtb.Seperators.Add(',');
//            shtb.Seperators.Add('.');
//            shtb.Seperators.Add('-');
//            shtb.Seperators.Add('+');

//            shtb.FilterAutoComplete = false;
//            /*shtb.HighlightDescriptors.Add(new HighlightDescriptor("<", Color.Gray, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("<<", ">>", Color.DarkGreen, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));
//*/
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("create", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("update", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("delete", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("select", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("from", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("table", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("where", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("having", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("between", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("database", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("show", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("index", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("column", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("count", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("avg", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("min", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("max", Color.Pink, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("int", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("double", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("varchar", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("default", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("null", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
//            shtb.HighlightDescriptors.Add(new HighlightDescriptor("enum", Color.Green, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

        }

        public   QueryResult GetTable(string dataBase, string tableName)
        {

            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "`;";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(s, SQLiteConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        
                        return new QueryResult(dt);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit)
        {
            try
            {
                using (DataSet tableSet = new DataSet())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "` limit " + lowLimit + "," + hiLimit + ";";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(s, SQLiteConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(tableSet, tableName);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        QueryResult qr = new QueryResult(tableSet.Tables[tableName]);
                        qr.StartTime = start;
                        qr.Duration = Convert.ToInt64(time.TotalMilliseconds);
                        return qr ;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetTableStatus(string dataBase)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "show table status from " + dataBase + ";";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(s, SQLiteConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        
                        return new QueryResult(dt);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
        }

        public   QueryResult GetColumnInformation(string dataBase, string tableName)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                if (tableName == null)
                    return null;
                string s = "pragma `"+dataBase+"`.table_info( `" + tableName + "`);";
                cmd = new SQLiteCommand(s, SQLiteConnection);
                using (DataTable dt = new DataTable())
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    dt.Columns["name"].ColumnName = "Field";
                    dt.Columns.Add("null", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["notnull"].ToString() == "0")
                            dt.Rows[i]["null"] = "YES";
                        else
                            dt.Rows[i]["null"] = "NO";
                    }
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    da.Dispose();
                    cmd.Dispose();
                    return new QueryResult(dt);
                }
            }
            catch (Exception ex)
            {
                cmd.Dispose();
                return new QueryResult(ex.Message);
            }
        }

        public   QueryResult GetIndexInformation(string dataBase, string tableName)
        {
            SQLiteDataAdapter da = new SQLiteDataAdapter() ;
            SQLiteCommand cmd = new SQLiteCommand() ;
            try
            {
                if (dataBase == null || tableName == null)
                    return null;
                string s = "pragma `" +dataBase +"`.index_list(`" +tableName + "`);";
                cmd = new SQLiteCommand(s, SQLiteConnection);
                using (DataTable dt = new DataTable())
                {
                    da = new SQLiteDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    DataTable dtIndex = new DataTable();
                    dtIndex.Columns.Add("key_name");
                    dtIndex.Columns.Add("Column_name");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        s = "pragma index_info(`" + dt.Rows[i]["name"].ToString() + "`);";
                        cmd = new SQLiteCommand(s, SQLiteConnection);
                        SQLiteDataAdapter dai = new SQLiteDataAdapter(cmd);
                        DataTable dtii = new DataTable();
                        dai.Fill(dtii);
                        for( int j=0;j<dtii.Rows.Count;j++)
                            dtIndex.Rows.Add(new string[] { dt.Rows[i]["name"].ToString(), dtii.Rows[j]["name"].ToString() });
                    }
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    cmd.Dispose();
                    da.Dispose();
                        return new QueryResult(dtIndex);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (Exception ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult( ex.Message);
            }
    }

        public   string GetDDLInformation(string dataBase, string tableName)
        {
            SQLiteDataAdapter da = new SQLiteDataAdapter();
            SQLiteCommand cmd = new SQLiteCommand();
            DataTable dt = new DataTable();
            string rez = null;
            string s = "select `sql` from `"+dataBase+"`.sqlite_master WHERE tbl_name = '" + tableName + "'";
            cmd = new SQLiteCommand(s,SQLiteConnection);
            da = new SQLiteDataAdapter(cmd); 
            DateTime start = DateTime.Now;
            da.Fill(dt);
            TimeSpan time = DateTime.Now.Subtract(start);
            asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
            
            if (dt.Rows.Count > 0)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (dt.Rows[0][0].GetType() == typeof(byte[]))
                    rez = Encoding.Default.GetString((byte[])dt.Rows[0][0]).ToLower();
                else
                    rez = dt.Rows[0][0].ToString();
                da.Dispose();
                cmd.Dispose();
                return rez +";";
            }
            da.Dispose();
            cmd.Dispose();
            return null;
        }

        public   QueryResult DropTable(string database, string tableName)
        {
            try
            {
                string s = "drop table `"+database+"`.`" + tableName + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public   QueryResult EmptyTable(string dataBase, string tableName)
        {
            try
            {
                string s = "delete from `" + dataBase + "`.`" + tableName + "`; VACUUM";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult DropDataBase(string database)
        {
            try
            {
                string s = "drop database `" + database + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult DropProcedure(string database, string name)
        {
            try
            {
                string s ="drop procedure`" + database + "`.`" +name+"`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                }

                return new QueryResult("OK");
            }
            catch (SQLiteException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult DropView(string database, string name)
        {
            try
            {
                string s="drop view `" +database+"`.`" + name + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                }
                return new QueryResult("OK");
            }
            catch (SQLiteException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult DropFunction(string database, string name)
        {
            try
            {
                string s = "drop function `" + database + "`.`" + name + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    return new QueryResult("OK");
                }

            }
            catch (SQLiteException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult DropTrigger(string database, string name)
        {
            try
            {
                string s = "drop trigger `" + database+"`.`"+ name + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    return new QueryResult("OK");
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   QueryResult TruncateTable(string database, string tableName)
        {
            try
            {
                string s = "truncate table `" + database + "`.`" + tableName + "`;";
                using (SQLiteCommand cmd = new SQLiteCommand(s, SQLiteConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    return new QueryResult("OK");
                }

            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public   QueryResult TruncateDataBase(string database)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter("show tables from `" + database + "`;", SQLiteConnection))
                    {
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                            TruncateTable(database, dt.Rows[i][0].ToString());
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return new QueryResult( ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public   void Disconnect()
        {
            this.SQLiteConnection.Close();
            this.SQLiteConnection.Dispose();
            this.updateNodeTimer.Stop();
            this.Dispose();
        }

        public   void ShowAlterTableForm(string dataBase, string tableName)
        {
            AlterTableForm aForm = new AlterTableForm(this.SQLiteConnection, dataBase, tableName, AlterTableForm.Mod.Alter);
            aForm.ShowDialog();
        }

        public   void ShowReorderColumnForm(string dataBase, string tableName)
        {

        }

        public   void ShowCreateTableForm(TreeNode nod)
        {
            string dataBase = nod.Text;
            AlterTableForm aForm = new AlterTableForm(this.SQLiteConnection, dataBase, null, AlterTableForm.Mod.Create);
            aForm.ShowDialog();
        }

        public   void ShowIndexesTableForm(string dataBase, string tableName)
        {
            IndexForm iForm = new IndexForm(this.SQLiteConnection, dataBase, tableName);
            iForm.ShowDialog();
        }

        public   void RenameTable(string dataBase, string tableName, string newName)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand("alter table `"+ dataBase + "`.`" +tableName + "` rename to `" + newName + "`;", SQLiteConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public   void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
        {
            BackUpDataForm bf = new BackUpDataForm(this.SQLiteConnection, this);
            //bf.Browser = this;
            bf.ShowDialog();
        }

        public   void ShowExportTableForm(string dataBase, string tableName)
        {
            using (QueryResult qR = this.ListColumns(dataBase, tableName))
            {
                ExportData bf = new ExportData(this.SQLiteConnection, dataBase, tableName, (string[])qR.Result);
                bf.ShowDialog();
            }
        }

        public void ShowExportTableForm(DataGridView dgView, string[] columns)
        {
            ExportData bf = new ExportData(dgView, columns);
            bf.ShowDialog();
        }

        public void ShowExecuteForm(string fileName, Form mdiParent)
        {
            ExecuteBatchForm eForm = new ExecuteBatchForm(fileName, this.SQLiteConnection);
            eForm.Show(mdiParent);
        }

        public void ShowReferenceTableForm(string dataBase, string tableName)
        {
            ForeignKeyForm fForm = new ForeignKeyForm(this.SQLiteConnection, dataBase, tableName);
            fForm.ShowDialog();
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
                dgTableView.CellClick -= dgTableView_CellClick;
                dgTableView.CellBeginEdit -= dgTableView_CellBeginEdit;
                dgTableView.CellValueNeeded -= dgTableView_CellValueNeeded;
                dgTableView.CellValuePushed -= dgTableView_CellValuePushed;

                dgTableView.CellBeginEdit += new DataGridViewCellCancelEventHandler(dgTableView_CellBeginEdit);
                dgTableView.CellFormatting += new DataGridViewCellFormattingEventHandler(dgTableView_CellFormatting);
                dgTableView.CellClick += new DataGridViewCellEventHandler(dgTableView_CellClick);
                dgTableView.CellValueNeeded += new DataGridViewCellValueEventHandler(dgTableView_CellValueNeeded);
                dgTableView.CellValuePushed += new DataGridViewCellValueEventHandler(dgTableView_CellValuePushed);
                dataUserView.DataBase = dataBase;
                dataUserView.TableName = tableName;

                DataTable dt = new DataTable();

                //QueryResult qR = GetColumnInformation(dataBase, tableName);
                QueryResult qR = new QueryResult(null);
                SQLiteCommand cmd = new SQLiteCommand();
                try
                {
                    string s = "pragma table_info( `" + tableName + "`)";
                    cmd = new SQLiteCommand(s, SQLiteConnection);
                    SQLiteDataAdapter da = new SQLiteDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    da.Dispose();
                    cmd.Dispose();
                    qR = new QueryResult(dt);
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

                    if (dt.Rows[i]["pk"].ToString() == "1")
                    {
                        TablePrimaryKeys.Add(dt.Rows[i]["name"].ToString());
                    }
                    string fullType = dt.Rows[i]["type"].ToString().ToLower();
                    string mainType = ExtractMainTypeFromFullType(fullType);

                    //handle text, set,enum and blob columns
                    switch (mainType)
                    {
                        case "blob":
                        case "image":
                        case "picture":
                        case "photo":
                        case "raw":
                        case "graphic":
                            DataGridViewButtonColumn c3 = new DataGridViewButtonColumn();
                            c3.DefaultCellStyle.NullValue = "(null)";
                            c3.Resizable = DataGridViewTriState.True;
                            c3.DataPropertyName = dt.Rows[i]["name"].ToString();
                            c3.HeaderText = c3.DataPropertyName;
                            dgTableView.Columns.Add(c3);
                            c3.Tag = new object[] { mainType, null };
                            c3.Name = c3.HeaderText;
                            break;
                        case "text":
                        case "blob_text":
                            DataGridViewButtonColumn c4 = new DataGridViewButtonColumn();
                            c4.Resizable = DataGridViewTriState.True;
                            c4.DefaultCellStyle.NullValue = "(null)";
                            c4.DataPropertyName = dt.Rows[i]["name"].ToString();
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
                            c5.DataPropertyName = dt.Rows[i]["name"].ToString();
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
                            c6.DataPropertyName = dt.Rows[i]["name"].ToString();
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
            string q = "select * from `" + dataBase + "`.`" + tableName + "` ";
            if (sortColumn != null)
            {
                q += " order by `" + sortColumn.HeaderText + "` ";
                if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                    q += "asc";
                else
                    if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                        q += "desc";
            }
            if (useLimits)
                q += " limit " + limitT1 + ", " + limitT2 + "";

            DateTime start = DateTime.Now;
            workerBinder.RunWorkerAsync(new object[] { start, q, dgTableView });
            tableIsBinding = false;
        }


        public   void SaveTable(string tableName, DataUserView dataUserView)
        {
            ExecuteGeneratedUpdateString(dataUserView);
            ExecuteGeneratedInsertString(dataUserView);

        }

        public   void AddNewRowToBoundedTable(DataGridView dgTableView)
        {
            if (dgTableView != null)
            {
                if (dgTableView.ColumnCount < 1)
                    return;
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                cachedRows.Add(new SqlDataGridViewRow(new object[dgTableView.ColumnCount - 1], new UpdateInfo(UpdateType.Insert)));
                dgTableView.Rows.Add();
            }
        }

        public   void DeleteSelectedRowsTable(DataUserView dataUserView)
        {
            bool atLeastOne = false;
            int nrPar = 0;
            StringBuilder cmdText = new StringBuilder();
            DataGridView dgTableView = dataUserView.DGTableView;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            dgTableView.EndEdit();
            SQLiteCommand cmd = new SQLiteCommand("", SQLiteConnection);
            List<int> remItems = new List<int>();
            for (int k = 0; k < cachedRows.Count; k++)
            {
                bool mustDelete = false;
                bool wasUpdated = false;
                if (cachedRows[k].Checked)
                {
                    UpdateInfo ui = (UpdateInfo)dgTableView.Rows[k].Tag;
                    if (ui == null)
                        mustDelete = true;
                    else
                    {
                        if (ui.tipUpdate == UpdateType.Update)
                        {
                            mustDelete = true;
                            wasUpdated = true;
                        }
                        else
                            mustDelete = false;
                    }

                    if (mustDelete)
                    {

                        string rem = "delete from `"  + dataUserView.TableName + "` where ";
                        int nr = 0;
                        string and = "";
                        for (int i = 0; i < dgTableView.Columns.Count - 1; i++)
                        {
                            if (cachedRows[k].Data[i] != null)
                            {
                                if (cachedRows[k].Data[i].GetType() != typeof(byte[]))
                                {
                                    if (!wasUpdated)
                                    {
                                        if (cachedRows[k].Data[i] != DBNull.Value)
                                        {
                                            rem += and + "`" + dgTableView.Columns[i + 1].Name + "`=@p" + nrPar.ToString();
                                            cmd.Parameters.AddWithValue("@p" + nrPar.ToString(), cachedRows[k].Data[i]);
                                            nrPar++;
                                        }
                                        else
                                        {
                                            rem += and + "`" + dgTableView.Columns[i + 1].Name + "` is null ";
                                        }
                                    }
                                    else
                                    {
                                        bool found = false;
                                        for (int j = 0; j < ui.elements.Count && !found; j++)
                                        {
                                            if (ui.elements[j].columnName == dgTableView.Columns[i].Name)
                                            {
                                                found = true;
                                                if (ui.elements[j].oldValue != DBNull.Value)
                                                {
                                                    rem += and + "`" + dgTableView.Columns[i + 1].Name + "`=@p" + nrPar.ToString();
                                                    cmd.Parameters.AddWithValue("@p" + nrPar.ToString(), ui.elements[j].oldValue);
                                                    nrPar++;
                                                }
                                                else
                                                {
                                                    rem += and + "`" + dgTableView.Columns[i + 1].Name + "` is null ";
                                                }
                                            }
                                        }
                                        if (!found)
                                        {
                                            rem += and + "`" + dgTableView.Columns[i + 1].Name + "`=@p" + nrPar.ToString();
                                            cmd.Parameters.AddWithValue("@p" + nrPar.ToString(), cachedRows[k].Data[i]);
                                            nrPar++;
                                        }

                                    }
                                }
                                atLeastOne = true;
                            }
                            nr++;
                            and = " and ";
                        }
                        cmdText.Append(rem);
                        cmdText.Append(";");
                    }
                    remItems.Add(k);
                }
            }
            if (atLeastOne)
            {
                cmd.CommandText = cmdText.ToString();
                try
                {

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public   void ShowFlushForm()
        {
            //FlushForm fForm = new FlushForm(SQLiteConnection);
            //fForm.ShowDialog();
        }

        public   void ShowStatusForm()
        {
            //StatusForm fForm = new StatusForm(SQLiteConnection);
            //fForm.ShowDialog();
        }

        public   void ShowTableDiagnosticForm()
        {
            //TableDiagnosticForm fForm = new TableDiagnosticForm(SQLiteConnection,this,CurrentDataBase);
            //fForm.ShowDialog();
        }

        public   void ShowNewDbForm(TreeNode node)
        {
            //NewDbForm ndbForm = new NewDbForm(SQLiteConnection);
            //ndbForm.ShowDialog();
        }

        public   void ShowImportFromCSVForm(string dataBase, string tableName)
        {
            ImportFromCSV fForm = new ImportFromCSV(SQLiteConnection, dataBase, tableName);
            fForm.ShowDialog();
        }

        public   void Reconnect()
        {
            try
            {
                this.SQLiteConnection.Close();
                this.SQLiteConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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
                    nod.ImageIndex =  1;
                    nod.SelectedImageIndex =  1;
                    nod.Nodes.Add("-");
                    nod.ContextMenuStrip = dataBaseMenu;
                    ret.Add(nod);
                }
            }
            return ret;
        }

        protected override List<TreeNode> RefreshDataBase(TreeNode dbNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            ret.Add(dbNode);
            string[] tables = new string[] { };
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
                        for (int i = 0; i < tables.Length; i++)
                        {
                            TreeNode nod = new TreeNode(tables[i],  2,  2);
                            nod.Tag = new object[] { this, SQLNodeType.Table, SQLNodeStatus.Unset };
                            nod.ContextMenuStrip = tableMenu;
                            TreeNode cNode = new TreeNode("Columns",  3,  3);
                            cNode.Tag = new object[] { this, SQLNodeType.Column, SQLNodeStatus.Unset };
                            cNode.Nodes.Add("-");
                            nod.Nodes.Add(cNode);
                            TreeNode iNode = new TreeNode("Index",  3,  3);
                            iNode.Tag = new object[] { this, SQLNodeType.Index, SQLNodeStatus.Unset };
                            iNode.Nodes.Add("-");
                            nod.Nodes.Add(iNode);
                            ret.Add(nod);
                        }
                    TreeNode vNode = new TreeNode("Views",  3,  3);
                    vNode.Tag = new object[] { this, SQLNodeType.View, SQLNodeStatus.Unset };
                    vNode.ContextMenuStrip = viewMenu;
                    vNode.Nodes.Add("-");
                    //TreeNode pNode = new TreeNode("Procedures",  3,  3);
                    //pNode.ContextMenuStrip = procedureMenu;
                    //pNode.Tag = new object[] { this, SQLiteNodeType.Procedure, SQLNodeStatus.Unset };
                    //pNode.Nodes.Add("-");
                    //TreeNode fNode = new TreeNode("Functions",  3,  3);
                    //fNode.ContextMenuStrip = functionMenu;
                    //fNode.Tag = new object[] { this, SQLiteNodeType.Function, SQLNodeStatus.Unset };
                    //fNode.Nodes.Add("-");
                    TreeNode tNode = new TreeNode("Triggers",  3,  3);
                    tNode.ContextMenuStrip = triggerMenu;
                    tNode.Tag = new object[] { this, SQLNodeType.Trigger, SQLNodeStatus.Unset };
                    tNode.Nodes.Add("-");
                    ret.Add(vNode);
                    //ret.Add(pNode);
                    //ret.Add(fNode);
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
            cNode.Tag = new object[] { this, SQLNodeType.Column, SQLNodeStatus.Refreshed };
            for (int i = 0; i < cols.Rows.Count; i++)
            {
                string id = cols.Rows[i]["Field"].ToString();
                string type = cols.Rows[i]["Type"].ToString();
                string nul = cols.Rows[i]["Null"].ToString().ToUpper() == "YES" ? "null" : "not null";
                TreeNode nodCol = new TreeNode(id + " [" + type + " , " + nul + "]",  7,  7);
                nodCol.Tag = new object[] { this, SQLNodeType.Column, SQLNodeStatus.Refreshed };
                cNode.Nodes.Add(nodCol);
            }
            ret.Add(cNode);

            TreeNode iNode = new TreeNode("Index", 3, 3);

            List<string> indexNodes = new List<string>();
            for (int i = 0; i < indexes.Rows.Count; i++)
            {
                string col = indexes.Rows[i]["Column_name"].ToString();
                string type = indexes.Rows[i]["Key_name"].ToString();
                if (!indexNodes.Contains(type))
                {
                    indexNodes.Add(type);
                    TreeNode nodInd = new TreeNode(type + " ,[" + col + "]",  8,  8);
                    nodInd.Tag = new object[] { this, SQLNodeType.Index, SQLNodeStatus.Refreshed };
                    iNode.Nodes.Add(nodInd);
                }
                else
                {
                    int ii = indexNodes.IndexOf(type);
                    iNode.Nodes[ii].Text = iNode.Nodes[ii].Text.Replace("]", "," + col + "]");
                }
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
                            vNode.Tag = new object[] { this, SQLNodeType.View, SQLNodeStatus.Refreshed };
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
                            node.Tag = new object[] { this, SQLNodeType.Procedure, SQLNodeStatus.Refreshed };
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
                            node.Tag = new object[] { this, SQLNodeType.Function, SQLNodeStatus.Refreshed };
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
                            TreeNode node = new TreeNode(trigg[i], 7, 7);
                            node.ContextMenuStrip = triggerMenu;
                            node.Tag = new object[] { this, SQLNodeType.Trigger, SQLNodeStatus.Refreshed };
                            ret.Add(node);
                        }
                }
            }
            return ret;
        }


        public   ContextMenuStrip GetMainMenu()
        {
            return this.serverMenu;
        }


        public TreeNode CreateMainTreeNode()
        {
            TreeNode nodS = new TreeNode(GetConnectionSource() + "/" + Path.GetFileName(ServerAddress), 0, 0);
            nodS.Tag = new object[] { this, SQLNodeType.Server, SQLNodeStatus.Unset };
            nodS.ContextMenuStrip = this.GetMainMenu();
            nodS.ImageIndex = 0;
            nodS.SelectedImageIndex =0;
            nodS.Nodes.Add("-");
            currentNode = nodS;
            return nodS;
        }

        #region treeView




        public override void TreeNodeMouseDown(object sender, MouseEventArgs e)
        {
            if (browserIsBusy)
                return;
            currentNode = browseTree.GetNodeAt(e.X, e.Y);
            if (currentNode == null)
                return;
            switch (currentNode.Level)
            {
                case 1:
                    currentDataBase = currentNode.Text;
                    break;
                case 2:
                    currentDataBase = currentNode.Parent.Text;
                    currentTable = currentNode.Text;
                    break;
            }
        }
        #endregion


    #endregion



        #region privates

        #region queryExecution

        #endregion

        #region dgBoundedTable

        private void worker_DoWork( object sender, DoWorkEventArgs e)
        {
            object[] arg = (object[])e.Argument;
            string q = (string)arg[1];
            DataGridView dgTableView = (DataGridView)arg[2]; ////
            SQLiteDataAdapter = new SQLiteDataAdapter(q, SQLiteConnection);
            SQLiteCommand cmd = new SQLiteCommand(q, SQLiteConnection);
            SQLiteDataReader reader = null;
            DataTable data = new DataTable();
            try
            {
                if (SQLiteConnection.State == ConnectionState.Closed)
                    SQLiteConnection.Open();
                reader = cmd.ExecuteReader();
                int nr = 50;
                List<object[]> rez = new List<object[]>();
                int i = 0;
                int k = 0;
                object[] temp = new object[reader.FieldCount];
                while (reader.Read())
                {
                    temp = new object[reader.FieldCount];
                    for (int j = 0; j < reader.FieldCount; j++)
                        try
                        {
                            temp[j] = reader.GetValue(j);
                        }
                        catch (FormatException fex)
                        {
                        }
                    //reader.GetValues(temp);
                    rez.Add(temp);
                    k++;
                    i++;
                    if (i % nr == 0)
                    {
                        workerBinder.ReportProgress(0, new object[] { dgTableView, rez });
                        rez = new List<object[]>();
                        k = 0;
                        ready.WaitOne();
                    }
                }
                totalRows = i;
                if (k < nr && k > 0)
                    workerBinder.ReportProgress(0, new object[] { dgTableView, rez });
                reader.Close();
            }
            catch (Exception ex)
            {
                e.Result = new object[] { q, null, dgTableView, ex.Message };
                if (reader != null)
                    reader.Close();
                return;
            }
            e.Result = new object[] { q, data, dgTableView, null };
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateNodeTimer.Stop();
            updateNodeTimer.Enabled = false;
            string error = null;
            object[] rez = (object[])e.Result;
            DataGridView dgTableView = (DataGridView)rez[2];
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (totalRows > 0)
                dgTableView.RowCount = cachedRows.Count;
            if (rez[3] != null)
                error = (string)rez[3];
            if (this.TableBounded != null)
                this.TableBounded(this, new TableBoundedArg(currentDataBase, CurrentTable, currentNode, error));
            DataGridView dataGrid = (DataGridView)rez[2];
            dgTableView.Tag = cachedRows;
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
                ready.Set();
            }
        }

        protected override void dgTableView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgTableView = (DataGridView)sender;
            int x = e.RowIndex;
            int y = e.ColumnIndex;
            if (e.Value != DBNull.Value && e.Value != null)
            {
                if (dgTableView.Columns[e.ColumnIndex].Tag != null)
                {
                    object[] tag = (object[])dgTableView.Columns[e.ColumnIndex].Tag;
                    string type = (string)tag[0];
                    switch (type)
                    {
                        case "blob":
                        case "blob_text":
                        case "graphic":
                        case "clob":
                            long blobLen = ((byte[])e.Value).Length;
                            e.Value = Utils.FormatBytes(blobLen);
                            e.FormattingApplied = true;
                            break;
                        case "text":
                            long fieldLength = e.Value.ToString().Length;
                            if (fieldLength > 300)
                            {
                                e.Value = dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 300);
                                e.FormattingApplied = true;
                            }
                            break;
                        case "binary":
                        case "varbinary":
                            string text = "";
                            if (e.Value.GetType() == typeof(byte[]))
                                text = Encoding.Default.GetString((byte[])e.Value);
                            else
                                text = e.Value.ToString();
                            e.Value = text;
                            e.FormattingApplied = true;
                            break;
                    }
                }
            }
        }




        protected override void dgTableView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgTableView = (DataGridView)sender;
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex].GetType() == typeof(DataGridViewComboBoxCell))
            {
                dgTableView.BeginEdit(false);
                DataGridViewComboBoxCell c = dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                ComboBox comboBox = (ComboBox)dgTableView.EditingControl;
                comboBox.DroppedDown = true;
            }
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (dgTableView.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn))
            {
                if (dgTableView.Columns[e.ColumnIndex].Tag != null)
                {
                    object[] tg = (object[])dgTableView.Columns[e.ColumnIndex].Tag;
                    string type = (string)tg[0];
                    switch (type)
                    {
                        case "text":
                        case "blob_text":
                            TextForm txtForm = new TextForm(cachedRows[e.RowIndex].Data[e.ColumnIndex - 1] != null ? cachedRows[e.RowIndex].Data[e.ColumnIndex - 1].ToString() : null, e.RowIndex, e.ColumnIndex);
                            txtForm.Text = "text";
                            txtForm.Tag = dgTableView;
                            dgTableView.BeginEdit(false);
                            txtForm.FormClosed += new FormClosedEventHandler(txtForm_FormClosed);
                            txtForm.ShowInTaskbar = true;
                            txtForm.TopMost = true;
                            txtForm.ShowDialog();
                            break;
                        case "blob":
                        case "image":
                        case "picture":
                        case "photo":
                        case "raw":
                        case "graphic":
                            BlobViewForm bForm = new BlobViewForm(e.RowIndex, e.ColumnIndex, true);
                            if (cachedRows[e.RowIndex].Data[e.ColumnIndex - 1] != DBNull.Value)
                                bForm = new BlobViewForm((byte[])cachedRows[e.RowIndex].Data[e.ColumnIndex - 1], e.RowIndex, e.ColumnIndex, true);
                            bForm.Tag = dgTableView;
                            dgTableView.BeginEdit(false);
                            bForm.FormClosed += new FormClosedEventHandler(blobForm_FormClosed);
                            bForm.ShowInTaskbar = true;
                            bForm.TopMost = true;
                            bForm.ShowDialog();
                            break;
                    }
                }
            }
            else
            {
                if (dgTableView.Columns[e.ColumnIndex].Tag != null)
                {
                    object[] tg = (object[])dgTableView.Columns[e.ColumnIndex].Tag;
                    string type = (string)tg[0];
                    if (type == "date" || type == "datetime" || type == "timestamp" || type == "time")
                    {
                        TypeCalendarForm.Mode mod = TypeCalendarForm.Mode.Date;
                        switch (type)
                        {
                            case "date":
                                mod = TypeCalendarForm.Mode.Date;
                                break;
                            case "timestamp":
                            case "datetime":
                                mod = TypeCalendarForm.Mode.DateAndTime;
                                break;
                            case "time":
                                mod = TypeCalendarForm.Mode.Time;
                                break;
                        }
                        DataGridViewCell cell = dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        Rectangle rectDate = dgTableView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, true);
                        string crtDate = null;
                        if (cell.Value == DBNull.Value || cell.Value == null)
                            crtDate = null;
                        else
                            crtDate = cell.Value.ToString();
                        TypeCalendarForm dateForm = new TypeCalendarForm(e.RowIndex, e.ColumnIndex, crtDate, rectDate.Width, mod);
                        dateForm.Tag = dgTableView;
                        dateForm.Height = 22;
                        Point pDate = dgTableView.PointToScreen(rectDate.Location);
                        dateForm.Show(this.MasterForm);
                        dateForm.Top = pDate.Y; ;
                        dateForm.Left = pDate.X;
                        dateForm.FormClosed += new FormClosedEventHandler(DateForm_FormClosed);
                    }
                }
            }


        }

       #endregion

        #region ForTableSaving

        private void ExecuteGeneratedInsertString(DataUserView dataUserView)
        {


                    int par = 0;
                    bool atLeastOne = false;
                    DataGridView dgTableView = dataUserView.DGTableView;
                    SQLiteCommand cmd = new SQLiteCommand("", SQLiteConnection);
                    StringBuilder strb = new StringBuilder();
                    DataTable dtc = new DataTable();
                    DataGridView dView = dataUserView.DGTableView;
                    List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dView.Tag;

                    //adapter.Fill(dtc);
                    dtc = (DataTable)this.GetColumnInformation(currentDataBase, CurrentTable).Result;

                    List<string> autoIncrementedColumns = new List<string>();
                    for (int k = 0; k < dtc.Rows.Count; k++)
                    {
                        if (dtc.Rows[k]["type"].ToString().Contains("auto_increment"))
                            autoIncrementedColumns.Add(dtc.Rows[k]["Field"].ToString());
                    }


                    for (int i = 0; i < dgTableView.RowCount; i++)
                    {
                        try
                        {
                            if (cachedRows[i].Tag != null)
                            {
                                UpdateInfo ui = (UpdateInfo)cachedRows[i].Tag;
                                if (ui.tipUpdate == UpdateType.Insert)
                                {
                                    strb.Length = 0;
                                    atLeastOne = true;
                                    string vstr = "";
                                    strb.Append("insert into `");
                                    strb.Append(CurrentTable);
                                    strb.Append("` (");
                                    vstr = "";
                                    for (int j = 1; j < dgTableView.Columns.Count; j++)
                                    {
                                        strb.Append(vstr);
                                        //if (!autoIncrementedColumns.Contains(dgTableView.Columns[j].Name))
                                        //{
                                        strb.Append(dgTableView.Columns[j].Name);
                                        vstr = ",";
                                        //}
                                    }
                                    strb.Append(" ) values (");
                                    vstr = "";
                                    for (int j = 1; j < dgTableView.Columns.Count; j++)
                                    {
                                        //if (!autoIncrementedColumns.Contains(dgTableView.Columns[j].Name))
                                        //{
                                        strb.Append(vstr);
                                        strb.Append("@par" + par.ToString());
                                        vstr = ",";
                                        cmd.Parameters.AddWithValue("@par" + par.ToString(), dgTableView.Rows[i].Cells[j].Value);
                                        par++;
                                        //}
                                    }
                                    strb.Append(");");
                                    cachedRows[i].Tag = null;
                                    cmd.CommandText = strb.ToString();
                                    cmd.ExecuteNonQuery();

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }


            }


        private void ExecuteGeneratedUpdateString(DataUserView dataUserView)
        {
            DataGridView dgTableView = dataUserView.DGTableView;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            int par = 0;
            bool atLeastOne = false;
            SQLiteCommand cmd = new SQLiteCommand("", SQLiteConnection);
            StringBuilder strb = new StringBuilder();
            for (int i = 0; i < dgTableView.RowCount; i++)
            {
                if (cachedRows[i].Tag != null)
                {
                    UpdateInfo ui = (UpdateInfo)cachedRows[i].Tag;
                    if (ui.tipUpdate == UpdateType.Update)
                    {
                        strb.Length=0;
                        strb.Append("update `" + dataUserView.TableName + "` set ");

                        atLeastOne = true;
                        for (int k = 0; k < ui.elements.Count; k++)
                        {
                            strb.Append("`");
                            strb.Append(ui.elements[k].columnName);
                            strb.Append("`");
                            strb.Append("=@par" + par.ToString());
                            strb.Append(",");
                            cmd.Parameters.AddWithValue("@par" + par.ToString(), cachedRows[i].Data[ui.elements[k].columnIndex - 1]);
                            par++;
                        }
                        strb.Remove(strb.Length - 1, 1);
                        strb.Append(" where ");
                        // we update based only on primary keys 
                        if (!dataUserView.TableHasPrimaryKeys)
                        {
                            string andstr = "";
                            for (int k = 1; k < dgTableView.Columns.Count; k++)
                            {
                                if (dgTableView.Columns[k].ValueType != typeof(byte[]))
                                {
                                    bool found = false;
                                    int id = -1;
                                    for (int j = 0; j < ui.elements.Count && !found; j++)
                                    {
                                        if (dgTableView.Columns[k].Name == ui.elements[j].columnName)
                                        {
                                            found = true;
                                            id = j;
                                        }
                                    }
                                    if (!found)
                                    {
                                        strb.Append(andstr);
                                        strb.Append("`");
                                        strb.Append(dgTableView.Columns[k].Name);
                                        strb.Append("`");
                                        if (cachedRows[i].Data[k - 1] != DBNull.Value)
                                        {
                                            strb.Append("=@par" + par.ToString());
                                            cmd.Parameters.AddWithValue("@par" + par.ToString(), cachedRows[i].Data[k - 1]);
                                        }
                                        else
                                        {
                                            strb.Append(" is null");
                                        }
                                    }
                                    else
                                    {
                                        strb.Append(andstr);
                                        strb.Append("`");
                                        strb.Append(dgTableView.Columns[k].Name);
                                        strb.Append("`");
                                        if (ui.elements[id].oldValue != DBNull.Value)
                                        {
                                            strb.Append("=@par" + par.ToString());
                                            cmd.Parameters.AddWithValue("@par" + par.ToString(), ui.elements[id].oldValue);
                                        }
                                        else
                                            strb.Append(" is null");

                                    }
                                    par++;
                                    andstr = " and ";
                                }
                            }
                        }
                        else
                        {
                            string andstr = "";
                            for (int k = 0; k < dataUserView.TablePrimaryKeys.Count; k++)
                            {

                                bool updatePrimaryKey = false;
                                for (int l = 0; l < ui.elements.Count && !updatePrimaryKey; l++)
                                {
                                    if (dataUserView.TablePrimaryKeys[k] == ui.elements[l].columnName)
                                    {
                                        updatePrimaryKey = true;
                                        strb.Append(andstr);
                                        strb.Append("`");
                                        strb.Append(dataUserView.TablePrimaryKeys[k]);
                                        strb.Append("`");
                                        if (ui.elements[l].oldValue != DBNull.Value && ui.elements[l].oldValue != null)
                                        {
                                            strb.Append("=@par" + par.ToString());
                                            cmd.Parameters.AddWithValue("@par" + par.ToString(), ui.elements[l].oldValue);
                                        }
                                        else
                                            strb.Append(" is null");
                                        par++;
                                        andstr = " and ";
                                    }
                                }
                                if (!updatePrimaryKey)
                                {
                                    bool found = false;
                                    int id = -1;
                                    for (int j = 1; j < dgTableView.Columns.Count && !found; j++)
                                    {
                                        if (dataUserView.TablePrimaryKeys[k] == dgTableView.Columns[j].Name)
                                        {
                                            found = true;
                                            strb.Append(andstr);
                                            strb.Append("`");
                                            strb.Append(dataUserView.TablePrimaryKeys[k]);
                                            strb.Append("`");
                                            if (dgTableView.Rows[i].Cells[j].Value != DBNull.Value)
                                            {
                                                strb.Append("=@par" + par.ToString());
                                                cmd.Parameters.AddWithValue("@par" + par.ToString(), dgTableView.Rows[i].Cells[j].Value);
                                            }
                                            else
                                                strb.Append(" is null");
                                            par++;
                                            andstr = " and ";

                                        }
                                    }
                                }

                            }
                        }
                        strb.Append(";");
                        try
                        {
                            cmd.CommandText = strb.ToString();
                            cmd.ExecuteNonQuery();
                            cachedRows[i].Tag = null;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        cachedRows[i].Tag = null;
                    }
                }
            }
        }

        private void ExecuteGeneratedDeleteString(DataUserView dataUserView)
        {
            DeleteSelectedRowsTable(dataUserView);
        }
        #endregion

        #region tools
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowCreateTableForm(currentNode);
        }


        private void dropDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to drop `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            this.DropDataBase(this.currentDataBase);
        }

        private void truncateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to drop `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            this.TruncateDataBase(currentDataBase);
        }

        private void emptyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = currentNode;
            string[] dataBases = (string[])ListDatabases().Result;
            string[] tables;
            if (n == null)
                return;
            if (n.Level == 1)
            {
                QueryResult qR = ListTables(n.Text);
                if (qR.Message != null)
                {
                    MessageBox.Show(qR.Message, "Error " + qR.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                    tables = (string[])qR.Result;
                ShowExportForm(dataBases, tables, n.Text, null);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = currentNode;
            if (n.Level == 1)
                ShowExecuteForm("", this.ParentForm);
        }

        private void alterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowAlterTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void manageIndexesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowIndexesTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void viewReletionshipforeignKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowReferenceTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowTableStatus(currentNode.Parent.Text, currentNode.Text);
        }

        private void truncateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.TruncateTable(currentNode.Parent.Text, currentNode.Text);
        }

        private void dropTabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to drop the table `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            QueryResult qres = this.DropTable(currentNode.Parent.Text, currentNode.Text);
            if (qres.Status == QueryStatus.Error)
            {
                MessageBox.Show(qres.Message, "Error" + qres.ErrorNo);
            }
            else
            {
                currentNode.Remove();
            }
        }

        private void emptyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to empty the table `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            QueryResult qres = this.EmptyTable(currentNode.Parent.Text, currentNode.Text);
            if (qres != null)
                MessageBox.Show(qres.Message, "Error" + qres.ErrorNo);
        }

        private void asSqldumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] dataBases = (string[])ListDatabases().Result;
            TreeNode n = currentNode;
            if (n == null)
                return;
            if (n.Level != 2 && n.Tag != null)
                return;
            string[] tables;
            QueryResult qR = ListTables(n.Parent.Text);
            if (qR.Message != null)
            {
                MessageBox.Show(qR.Message, "Error " + qR.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                tables = (string[])qR.Result;
            ShowExportForm(dataBases, tables, n.Parent.Text, n.Text);
        }

        private void asCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void createTriggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = GetNewTriggerQuery(currentNode.Parent.Text, currentNode.Text, "NewTrigger");
        }

        private void updateNodeTimer_Tick(object sender, EventArgs e)
        {
            if (!updateNodeTimer.Enabled)
                return;
            currentNode.ImageIndex++;
            if (currentNode.ImageIndex >  15)
            {
                currentNode.ImageIndex =  12;
            }
            currentNode.SelectedImageIndex = currentNode.ImageIndex;
        }

        private void refreshMenuItem_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void newQueryMenuItem_Click(object sender, EventArgs e)
        {
            NewDataUserViewRequested(this, null);
        }

        private void restoreDatabaseFromSqlDumpMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowExecuteForm("", this.ParentForm);
        }
      
        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            if (this.DisconnectRequestedEvent != null)
                DisconnectRequestedEvent(this, null);
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TreeNodeIsTableNode(currentNode))
            {
                currentNode.TreeView.LabelEdit = true;
                currentNode.BeginEdit();
            }
        }
        
        private void tableMenu_Opening(object sender, CancelEventArgs e)
        {
            if(TreeNodeIsTableNode(currentNode))
                tableMenu.Enabled=true;
            else
                tableMenu.Enabled=false;
        }
        private void alterViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP VIEW \"" + currentNode.Text + "\";";
                sql+=Environment.NewLine+GetViewDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this,sql );
            }
        }

        private void swowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewViewQuery(currentNode.Parent.Text, currentTable, "newView"));
        }

        private void dropViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (browserIsBusy)
            {
                MessageBox.Show("Browser is busy at the moment", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Sure you want to drop view " + currentNode.Text + "?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            QueryResult rez = DropView(currentNode.Parent.Parent.Text, currentNode.Text);
            currentNode.Remove();
            currentNode = null;
            if (rez.Message == "OK")
                return;
            if (rez.ErrorNo != 0)
                MessageBox.Show(rez.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewTriggerQuery(currentNode.Parent.Text, currentTable, "newTrigger"));
        }

        private void alterTriggerMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP TRIGGER \"" + currentNode.Text + "\";";
                sql += Environment.NewLine + GetTriggerDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this, sql);
            }
        }

        private void dropTriggerMenuItem_Click(object sender, EventArgs e)
        {
            if (browserIsBusy)
            {
                MessageBox.Show("Browser is busy at the moment", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Sure you want to drop view " + currentNode.Text + "?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            QueryResult rez = DropTrigger(currentNode.Parent.Parent.Text, currentNode.Text);
            currentNode.Remove();
            currentNode = null;
            if (rez.Message == "OK")
                return;
            if (rez.ErrorNo != 0)
                MessageBox.Show(rez.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void refreshTriggerMenuItem_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void viewMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                newViewToolStripMenuItem.Visible = true;
                alterViewToolStripMenuItem.Visible = false;
                dropViewToolStripMenuItem.Visible = false;
                refreshViewsToolStripMenuItem.Visible = true;
            }
            else
            {
                newViewToolStripMenuItem.Visible = false;
                alterViewToolStripMenuItem.Visible = true;
                dropViewToolStripMenuItem.Visible = true;
                refreshViewsToolStripMenuItem.Visible = false;
            }
        }

        private void triggerMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                alterTriggerMenuItem.Visible = false;
                dropTriggerMenuItem.Visible = false;
                refreshTriggerMenuItem.Visible = true;
                newTriggerToolStripMenuItem.Visible = true;
            }
            else
            {
                alterTriggerMenuItem.Visible = true;
                dropTriggerMenuItem.Visible = true;
                refreshTriggerMenuItem.Visible = false;
                newTriggerToolStripMenuItem.Visible = false;
            }
        }

        #endregion
     
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TreeNode node = (TreeNode)e.Argument;
            object[] arg = (object[])node.Tag;
            SQLNodeType nodType = (SQLNodeType)arg[1];
            SQLNodeStatus nodStatus = (SQLNodeStatus)arg[2];
            switch (nodType)
            {
                case SQLNodeType.Server:
                    e.Result = RefreshServer(node);
                    break;
                case SQLNodeType.Database:
                    e.Result = RefreshDataBase(node);
                    break;
                case SQLNodeType.Table:
                    e.Result = RefreshTableNode(node);
                    break;
                case SQLNodeType.Function:
                    e.Result = RefreshFunctions(node);
                    break;
                case SQLNodeType.Procedure:
                    e.Result = RefreshProcedures(node);
                    break;
                case SQLNodeType.View:
                    e.Result = RefreshViews(node);
                    break;
                case SQLNodeType.Trigger:
                    e.Result = RefreshTriggers(node);
                    break;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<TreeNode> rez = (List<TreeNode>)e.Result;
            TreeNode mainNode = (TreeNode)rez[0];
            mainNode.Nodes.Clear();

            for (int i = 1; i < rez.Count; i++)
            {
                mainNode.Nodes.Add(rez[i]);
            }

            object[] tag = (object[])mainNode.Tag;
            SQLNodeType type = (SQLNodeType)tag[1];
            mainNode.Tag = new object[] { this, type, SQLNodeStatus.Refreshed };
            browserIsBusy = false;
            updateNodeTimer.Enabled = false;
            updateNodeTimer.Stop();
            if (currentNode == null)
                return;
            currentNode.ImageIndex = oldImageIndex;
            currentNode.SelectedImageIndex = currentNode.ImageIndex;
        }

        private string ExtractMainTypeFromFullType(string fullType)
        {
            Match m = Regex.Match(fullType, "^[\\w-]+");
            if (m.Success)
                return m.Value.ToLower();
            return "";
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLiteDBBrowser));
            this.tableMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.alterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageIndexesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewReletionshipforeignKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.dropTabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.createTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDDLDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyDDLDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshTriggerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterTriggerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropTriggerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.swowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.procedureMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.functionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem24 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem23 = new System.Windows.Forms.ToolStripSeparator();
            this.newFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dataBaseMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.truncateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deattachDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.attachDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreDatabaseFromSqlDumpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.reconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.vacuumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.integrityCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.completionListIcons = new System.Windows.Forms.ImageList(this.components);
            this.tableMenu.SuspendLayout();
            this.triggerMenu.SuspendLayout();
            this.viewMenu.SuspendLayout();
            this.procedureMenu.SuspendLayout();
            this.functionMenu.SuspendLayout();
            this.dataBaseMenu.SuspendLayout();
            this.serverMenu.SuspendLayout();
            this.toolsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMenu
            // 
            this.tableMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem15,
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
            this.viewDDLDefinitionToolStripMenuItem,
            this.copyDDLDefinitionToolStripMenuItem});
            this.tableMenu.Name = "tableMenu";
            this.tableMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tableMenu.Size = new System.Drawing.Size(209, 308);
            this.tableMenu.Opening += new System.ComponentModel.CancelEventHandler(this.tableMenu_Opening);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem15.Image")));
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(208, 22);
            this.toolStripMenuItem15.Text = "Refresh";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.toolStripMenuItem15_Click);
            // 
            // alterToolStripMenuItem
            // 
            this.alterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterToolStripMenuItem.Image")));
            this.alterToolStripMenuItem.Name = "alterToolStripMenuItem";
            this.alterToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.alterToolStripMenuItem.Text = "Alter";
            this.alterToolStripMenuItem.Click += new System.EventHandler(this.alterToolStripMenuItem_Click);
            // 
            // manageIndexesToolStripMenuItem
            // 
            this.manageIndexesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("manageIndexesToolStripMenuItem.Image")));
            this.manageIndexesToolStripMenuItem.Name = "manageIndexesToolStripMenuItem";
            this.manageIndexesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.manageIndexesToolStripMenuItem.Text = "Indexes";
            this.manageIndexesToolStripMenuItem.Click += new System.EventHandler(this.manageIndexesToolStripMenuItem_Click);
            // 
            // viewReletionshipforeignKeysToolStripMenuItem
            // 
            this.viewReletionshipforeignKeysToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewReletionshipforeignKeysToolStripMenuItem.Image")));
            this.viewReletionshipforeignKeysToolStripMenuItem.Name = "viewReletionshipforeignKeysToolStripMenuItem";
            this.viewReletionshipforeignKeysToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewReletionshipforeignKeysToolStripMenuItem.Text = "Reletionship/foreign keys";
            this.viewReletionshipforeignKeysToolStripMenuItem.Click += new System.EventHandler(this.viewReletionshipforeignKeysToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem1
            // 
            this.advancedToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem});
            this.advancedToolStripMenuItem1.Name = "advancedToolStripMenuItem1";
            this.advancedToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.advancedToolStripMenuItem1.Text = "Advanced";
            this.advancedToolStripMenuItem1.Visible = false;
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
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
            this.dropTabelToolStripMenuItem.Click += new System.EventHandler(this.dropTabelToolStripMenuItem_Click);
            // 
            // emptyToolStripMenuItem1
            // 
            this.emptyToolStripMenuItem1.Name = "emptyToolStripMenuItem1";
            this.emptyToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.emptyToolStripMenuItem1.Text = "Empty";
            this.emptyToolStripMenuItem1.Click += new System.EventHandler(this.emptyToolStripMenuItem1_Click);
            // 
            // renameToolStripMenuItem1
            // 
            this.renameToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem1.Image")));
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.renameToolStripMenuItem1.Text = "Rename";
            this.renameToolStripMenuItem1.Click += new System.EventHandler(this.renameToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(205, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exportToolStripMenuItem.Text = "Export as...";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.importToolStripMenuItem1.Text = "Import from CSV file";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.ImportToolStripMenuItem1Click);
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
            this.createTriggerToolStripMenuItem.Click += new System.EventHandler(this.createTriggerToolStripMenuItem_Click);
            // 
            // viewDDLDefinitionToolStripMenuItem
            // 
            this.viewDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewDDLDefinitionToolStripMenuItem.Image")));
            this.viewDDLDefinitionToolStripMenuItem.Name = "viewDDLDefinitionToolStripMenuItem";
            this.viewDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewDDLDefinitionToolStripMenuItem.Text = "View DDL definition";
            this.viewDDLDefinitionToolStripMenuItem.Click += new System.EventHandler(this.viewDDLDefinitionToolStripMenuItem_Click);
            // 
            // copyDDLDefinitionToolStripMenuItem
            // 
            this.copyDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyDDLDefinitionToolStripMenuItem.Image")));
            this.copyDDLDefinitionToolStripMenuItem.Name = "copyDDLDefinitionToolStripMenuItem";
            this.copyDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.copyDDLDefinitionToolStripMenuItem.Text = "Copy DDL definition";
            this.copyDDLDefinitionToolStripMenuItem.Click += new System.EventHandler(this.copyDDLDefinitionToolStripMenuItem_Click);
            // 
            // triggerMenu
            // 
            this.triggerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshTriggerMenuItem,
            this.newTriggerToolStripMenuItem,
            this.alterTriggerMenuItem,
            this.dropTriggerMenuItem});
            this.triggerMenu.Name = "viewMenu";
            this.triggerMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.triggerMenu.Size = new System.Drawing.Size(139, 92);
            this.triggerMenu.Opening += new System.ComponentModel.CancelEventHandler(this.triggerMenu_Opening);
            // 
            // refreshTriggerMenuItem
            // 
            this.refreshTriggerMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshTriggerMenuItem.Image")));
            this.refreshTriggerMenuItem.Name = "refreshTriggerMenuItem";
            this.refreshTriggerMenuItem.Size = new System.Drawing.Size(138, 22);
            this.refreshTriggerMenuItem.Text = "Refresh";
            this.refreshTriggerMenuItem.Click += new System.EventHandler(this.refreshTriggerMenuItem_Click);
            // 
            // newTriggerToolStripMenuItem
            // 
            this.newTriggerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newTriggerToolStripMenuItem.Image")));
            this.newTriggerToolStripMenuItem.Name = "newTriggerToolStripMenuItem";
            this.newTriggerToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.newTriggerToolStripMenuItem.Text = "New trigger";
            this.newTriggerToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem10_Click);
            // 
            // alterTriggerMenuItem
            // 
            this.alterTriggerMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterTriggerMenuItem.Image")));
            this.alterTriggerMenuItem.Name = "alterTriggerMenuItem";
            this.alterTriggerMenuItem.Size = new System.Drawing.Size(138, 22);
            this.alterTriggerMenuItem.Text = "Alter trigger";
            this.alterTriggerMenuItem.Click += new System.EventHandler(this.alterTriggerMenuItem_Click);
            // 
            // dropTriggerMenuItem
            // 
            this.dropTriggerMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropTriggerMenuItem.Image")));
            this.dropTriggerMenuItem.Name = "dropTriggerMenuItem";
            this.dropTriggerMenuItem.Size = new System.Drawing.Size(138, 22);
            this.dropTriggerMenuItem.Text = "Drop trigger";
            this.dropTriggerMenuItem.Click += new System.EventHandler(this.dropTriggerMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshViewsToolStripMenuItem,
            this.newViewToolStripMenuItem,
            this.alterViewToolStripMenuItem,
            this.dropViewToolStripMenuItem,
            this.toolStripMenuItem14,
            this.swowToolStripMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.viewMenu.Size = new System.Drawing.Size(128, 120);
            this.viewMenu.Opening += new System.ComponentModel.CancelEventHandler(this.viewMenu_Opening);
            // 
            // refreshViewsToolStripMenuItem
            // 
            this.refreshViewsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshViewsToolStripMenuItem.Image")));
            this.refreshViewsToolStripMenuItem.Name = "refreshViewsToolStripMenuItem";
            this.refreshViewsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.refreshViewsToolStripMenuItem.Text = "Refresh";
            this.refreshViewsToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem17_Click);
            // 
            // newViewToolStripMenuItem
            // 
            this.newViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newViewToolStripMenuItem.Image")));
            this.newViewToolStripMenuItem.Name = "newViewToolStripMenuItem";
            this.newViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.newViewToolStripMenuItem.Text = "New view";
            this.newViewToolStripMenuItem.Click += new System.EventHandler(this.newViewToolStripMenuItem_Click);
            // 
            // alterViewToolStripMenuItem
            // 
            this.alterViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterViewToolStripMenuItem.Image")));
            this.alterViewToolStripMenuItem.Name = "alterViewToolStripMenuItem";
            this.alterViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.alterViewToolStripMenuItem.Text = "Alter view";
            this.alterViewToolStripMenuItem.Click += new System.EventHandler(this.alterViewToolStripMenuItem_Click);
            // 
            // dropViewToolStripMenuItem
            // 
            this.dropViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropViewToolStripMenuItem.Image")));
            this.dropViewToolStripMenuItem.Name = "dropViewToolStripMenuItem";
            this.dropViewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.dropViewToolStripMenuItem.Text = "Drop view";
            this.dropViewToolStripMenuItem.Click += new System.EventHandler(this.dropViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(124, 6);
            // 
            // swowToolStripMenuItem
            // 
            this.swowToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("swowToolStripMenuItem.Image")));
            this.swowToolStripMenuItem.Name = "swowToolStripMenuItem";
            this.swowToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.swowToolStripMenuItem.Text = "Show";
            this.swowToolStripMenuItem.Click += new System.EventHandler(this.swowToolStripMenuItem_Click);
            // 
            // procedureMenu
            // 
            this.procedureMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem20,
            this.toolStripMenuItem19,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9});
            this.procedureMenu.Name = "functionMenu";
            this.procedureMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.procedureMenu.Size = new System.Drawing.Size(163, 98);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem20.Image")));
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuItem20.Text = "Refresh";
            this.toolStripMenuItem20.Click += new System.EventHandler(this.toolStripMenuItem20_Click);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(159, 6);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem7.Image")));
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuItem7.Text = "New procedure";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem8.Image")));
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuItem8.Text = "Alter proccedure";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem9.Image")));
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuItem9.Text = "Drop procedure";
            // 
            // functionMenu
            // 
            this.functionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem24,
            this.toolStripMenuItem23,
            this.newFunctionToolStripMenuItem,
            this.alterFunctionToolStripMenuItem,
            this.dropFunctionToolStripMenuItem});
            this.functionMenu.Name = "functionMenu";
            this.functionMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.functionMenu.Size = new System.Drawing.Size(149, 98);
            // 
            // toolStripMenuItem24
            // 
            this.toolStripMenuItem24.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem24.Image")));
            this.toolStripMenuItem24.Name = "toolStripMenuItem24";
            this.toolStripMenuItem24.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem24.Text = "Refresh";
            this.toolStripMenuItem24.Click += new System.EventHandler(this.toolStripMenuItem24_Click);
            // 
            // toolStripMenuItem23
            // 
            this.toolStripMenuItem23.Name = "toolStripMenuItem23";
            this.toolStripMenuItem23.Size = new System.Drawing.Size(145, 6);
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "sqlite.png");
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
            this.imageList1.Images.SetKeyName(12, "loading1.png");
            this.imageList1.Images.SetKeyName(13, "loading2.png");
            this.imageList1.Images.SetKeyName(14, "loading3.png");
            this.imageList1.Images.SetKeyName(15, "loading4.png");
            // 
            // dataBaseMenu
            // 
            this.dataBaseMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.createToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.backupToolStripMenuItem,
            this.importToolStripMenuItem,
            this.deattachDatabaseToolStripMenuItem});
            this.dataBaseMenu.Name = "dataBaseMenu";
            this.dataBaseMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.dataBaseMenu.Size = new System.Drawing.Size(196, 136);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createToolStripMenuItem.Image")));
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.createToolStripMenuItem.Text = "Create table";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
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
            this.dropDatabaseToolStripMenuItem.Click += new System.EventHandler(this.dropDatabaseToolStripMenuItem_Click);
            // 
            // truncateToolStripMenuItem
            // 
            this.truncateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("truncateToolStripMenuItem.Image")));
            this.truncateToolStripMenuItem.Name = "truncateToolStripMenuItem";
            this.truncateToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.truncateToolStripMenuItem.Text = "Truncate";
            this.truncateToolStripMenuItem.Click += new System.EventHandler(this.truncateToolStripMenuItem_Click);
            // 
            // emptyToolStripMenuItem
            // 
            this.emptyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("emptyToolStripMenuItem.Image")));
            this.emptyToolStripMenuItem.Name = "emptyToolStripMenuItem";
            this.emptyToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.emptyToolStripMenuItem.Text = "Empty";
            this.emptyToolStripMenuItem.Click += new System.EventHandler(this.emptyToolStripMenuItem_Click);
            // 
            // backupToolStripMenuItem
            // 
            this.backupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("backupToolStripMenuItem.Image")));
            this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            this.backupToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.backupToolStripMenuItem.Text = "Backup as sql dump";
            this.backupToolStripMenuItem.Visible = false;
            this.backupToolStripMenuItem.Click += new System.EventHandler(this.backupToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importToolStripMenuItem.Image")));
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.importToolStripMenuItem.Text = "Restore from sql dump";
            this.importToolStripMenuItem.Visible = false;
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // deattachDatabaseToolStripMenuItem
            // 
            this.deattachDatabaseToolStripMenuItem.Name = "deattachDatabaseToolStripMenuItem";
            this.deattachDatabaseToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.deattachDatabaseToolStripMenuItem.Text = "Deattach database";
            this.deattachDatabaseToolStripMenuItem.Click += new System.EventHandler(this.deattachDatabaseToolStripMenuItem_Click);
            // 
            // serverMenu
            // 
            this.serverMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshMenuItem,
            this.newQueryMenuItem,
            this.toolStripMenuItem13,
            this.attachDatabaseToolStripMenuItem,
            this.restoreDatabaseFromSqlDumpMenuItem,
            this.toolStripMenuItem6,
            this.reconnectMenuItem,
            this.disconnectMenuItem});
            this.serverMenu.Name = "tableMenu";
            this.serverMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.serverMenu.Size = new System.Drawing.Size(246, 148);
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshMenuItem.Image")));
            this.refreshMenuItem.Name = "refreshMenuItem";
            this.refreshMenuItem.Size = new System.Drawing.Size(245, 22);
            this.refreshMenuItem.Text = "Refresh";
            this.refreshMenuItem.Click += new System.EventHandler(this.refreshMenuItem_Click);
            // 
            // newQueryMenuItem
            // 
            this.newQueryMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newQueryMenuItem.Image")));
            this.newQueryMenuItem.Name = "newQueryMenuItem";
            this.newQueryMenuItem.Size = new System.Drawing.Size(245, 22);
            this.newQueryMenuItem.Text = "New query editor";
            this.newQueryMenuItem.Click += new System.EventHandler(this.newQueryMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(242, 6);
            // 
            // attachDatabaseToolStripMenuItem
            // 
            this.attachDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("attachDatabaseToolStripMenuItem.Image")));
            this.attachDatabaseToolStripMenuItem.Name = "attachDatabaseToolStripMenuItem";
            this.attachDatabaseToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.attachDatabaseToolStripMenuItem.Text = "Attach database";
            this.attachDatabaseToolStripMenuItem.Click += new System.EventHandler(this.attachDatabaseToolStripMenuItem_Click);
            // 
            // restoreDatabaseFromSqlDumpMenuItem
            // 
            this.restoreDatabaseFromSqlDumpMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreDatabaseFromSqlDumpMenuItem.Image")));
            this.restoreDatabaseFromSqlDumpMenuItem.Name = "restoreDatabaseFromSqlDumpMenuItem";
            this.restoreDatabaseFromSqlDumpMenuItem.Size = new System.Drawing.Size(245, 22);
            this.restoreDatabaseFromSqlDumpMenuItem.Text = "Restore database from sql dump";
            this.restoreDatabaseFromSqlDumpMenuItem.Visible = false;
            this.restoreDatabaseFromSqlDumpMenuItem.Click += new System.EventHandler(this.restoreDatabaseFromSqlDumpMenuItem_Click);
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
            this.disconnectMenuItem.Click += new System.EventHandler(this.disconnectMenuItem_Click);
            // 
            // toolsMenu
            // 
            this.toolsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vacuumToolStripMenuItem,
            this.integrityCheckToolStripMenuItem});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolsMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // vacuumToolStripMenuItem
            // 
            this.vacuumToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("vacuumToolStripMenuItem.Image")));
            this.vacuumToolStripMenuItem.Name = "vacuumToolStripMenuItem";
            this.vacuumToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.vacuumToolStripMenuItem.Text = "Vacuum";
            this.vacuumToolStripMenuItem.Click += new System.EventHandler(this.vacuumToolStripMenuClick);
            // 
            // integrityCheckToolStripMenuItem
            // 
            this.integrityCheckToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("integrityCheckToolStripMenuItem.Image")));
            this.integrityCheckToolStripMenuItem.Name = "integrityCheckToolStripMenuItem";
            this.integrityCheckToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.integrityCheckToolStripMenuItem.Text = "Integrity check";
            this.integrityCheckToolStripMenuItem.Click += new System.EventHandler(this.IntegrityCheckToolStripMenuItemClick);
            // 
            // completionListIcons
            // 
            this.completionListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("completionListIcons.ImageStream")));
            this.completionListIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.completionListIcons.Images.SetKeyName(0, "Icons.16x16.Method.png");
            this.completionListIcons.Images.SetKeyName(1, "vxclass_icon.png");
            this.completionListIcons.Images.SetKeyName(2, "table (1).png");
            this.completionListIcons.Images.SetKeyName(3, "column.png");
            this.completionListIcons.Images.SetKeyName(4, "gear.png");
            // 
            // SQLiteDBBrowser
            // 
            this.Name = "SQLiteDBBrowser";
            this.Size = new System.Drawing.Size(177, 52);
            this.tableMenu.ResumeLayout(false);
            this.triggerMenu.ResumeLayout(false);
            this.viewMenu.ResumeLayout(false);
            this.procedureMenu.ResumeLayout(false);
            this.functionMenu.ResumeLayout(false);
            this.dataBaseMenu.ResumeLayout(false);
            this.serverMenu.ResumeLayout(false);
            this.toolsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ToolStripMenuItem integrityCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vacuumToolStripMenuItem;


#endregion


        private CodeCompletionProvider GenerateCompletionData()
        {
            // We can return code-completion items like this:
            string s = @"ABORT,1
ACTION,1
ADD,0
AFTER,0
ALL,0
ALTER,0
ANALYZE,1
AND,1
AS,0
ASC,0
ATTACH,1
AUTOINCREMENT,1
BEFORE,1
BEGIN,0
BETWEEN,0
BY,0
CASCADE,0
CASE,0
CAST,0
CHECK,0
COLLATE,0
COLUMN,1
COMMIT,3
CONFLICT,1
CONSTRAINT,1
CREATE,0
CROSS,0
CURRENT_DATE,1
CURRENT_TIME,1
CURRENT_TIMESTAMP,1
DATABASE,1
DEFAULT,1
DEFERRABLE,1
DEFERRED,1
DELETE,0
DESC,1
DETACH,4
DISTINCT,1
DROP,0
EACH,1
ELSE,1
END,1
ESCAPE,1
EXCEPT,1
EXCLUSIVE,1
EXISTS,1
EXPLAIN,1
FAIL,1
FOR,1
FOREIGN,1
FROM,1
FULL,1
GLOB,1
GROUP,1
HAVING,1
IF,1
IGNORE,1
IMMEDIATE,1
IN,1
INDEX,1
INDEXED,1
INITIALLY,1
INNER,1
INSERT,1
INSTEAD,1
INTERSECT,1
INTO,1
IS,1
ISNULL,1
JOIN,1
KEY,1
LEFT,1
LIKE,1
LIMIT,1
MATCH,1
NATURAL,1
NO,1
NOT,1
NOTNULL,1
NULL,1
OF,1
OFFSET,1
ON,1
OR,1
ORDER,1
OUTER,1
PLAN,1
PRAGMA,4
PRIMARY,1
QUERY,1
RAISE,1
REFERENCES,1
REGEXP,1
REINDEX,4
RELEASE,1
RENAME,1
REPLACE,1
RESTRICT,1
RIGHT,1
ROLLBACK,1
ROW,1
SAVEPOINT,4
SELECT,0
SET,0
TABLE,2
TEMP,1
TEMPORARY,1
THEN,1
TO,1
TRANSACTION,1
TRIGGER,1
UNION,1
UNIQUE,1
UPDATE,1
USING,1
VACUUM,4
VALUES,1
VIEW,1
VIRTUAL,1
WHEN,1
WHERE,1";
            string q = s.Replace("\r", "");
            string[] list = q.Trim('\r').Split('\n');
            List<ICompletionData> lc = new List<ICompletionData>();
            for (int i = 0; i < list.Length; i++)
            {
                string[] x = list[i].Split(',');
                lc.Add(new DefaultCompletionData(x[0], int.Parse(x[1])));
            }
            return new SQLiteCodeCompletionProvider(completionListIcons, lc);
        }

        private void attachDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	AttachDbForm adbForm = new AttachDbForm();
        	adbForm.ShowDialog();
        	if(!adbForm.Canceled)
        	{
                string sql = "attach database \"" + adbForm.Path + "\" as [" + adbForm.Alias +"]";
                if(adbForm.Password!="")
                    sql+= " KEY '" + adbForm.Password + "' ;";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, SQLiteConnection))
        		{
        			try
        			{
        				TreeNode dbNode = new TreeNode(adbForm.Alias);
        				cmd.ExecuteNonQuery();
        				currentNode.Nodes.Add(dbNode);
        				RefreshNode(currentNode);
        				
        			}
        			catch(Exception ex)
        			{
        				MessageBox.Show(ex.Message, Application.ProductName,MessageBoxButtons.OK);
        			}
        		}
        	}
        
        		
        	
        }

        private void deattachDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("detach database \"" + currentNode.Text+"\";", SQLiteConnection))
            {
                try
                {
                    
                    cmd.ExecuteNonQuery();
                    currentNode.Remove();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK);
                }
            }
        }








        void vacuumToolStripMenuClick(object sender, EventArgs e)
        {
        	try{
        		using(SQLiteCommand cmd = new SQLiteCommand("vacuum;", SQLiteConnection))
				{
        		      	cmd.ExecuteNonQuery();
				}
        	
        	}
        	catch(SQLiteException ex)
        	{
        		MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        	}
        }
        
        void IntegrityCheckToolStripMenuItemClick(object sender, EventArgs e)
        {
        	try{
        		using(SQLiteCommand cmd = new SQLiteCommand("pragma `"+currentDataBase +"`.integrity_check;", SQLiteConnection))
				{
        			using(SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
        			{
        				DataTable dt = new DataTable();
        				da.Fill(dt);
        				if(dt.Rows.Count>0)
        				{
        					MessageForm mForm = new MessageForm(dt.Rows[0][0].ToString());
        					mForm.Show();
        				}
        					
        			}
        			      
				}
        	
        	}
        	catch(SQLiteException ex)
        	{
        		MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        	}
        }
        
        void ImportToolStripMenuItem1Click(object sender, EventArgs e)
        {
        	ImportFromCSV form = new ImportFromCSV(this.SQLiteConnection, currentDataBase, currentTable);
        	form.ShowDialog();
        }

        private void viewDDLDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("select `sql` from `" +currentNode.Parent.Text+"`.sqlite_master WHERE tbl_name = '"+currentNode.Text+"' ;", this.SQLiteConnection);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count < 1)
                    return;
                string s = dt.Rows[0][0].ToString();
                TextForm tf = new TextForm(s);
                tf.Show(this.parentForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void copyDDLDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("select `sql` from `" + currentNode.Parent.Text + "`.sqlite_master WHERE tbl_name = '" + currentNode.Text + "' ;", this.SQLiteConnection);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count < 1)
                    return;
                string s = dt.Rows[0][0].ToString();
                Clipboard.SetText(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = currentNode;
            if (n == null)
                return;
            if ((n.Level != 2 && n.Tag != null) || n.Parent == null)
                return;
            ShowExportTableForm(n.Parent.Text, n.Text);
        }
    }


    


}


