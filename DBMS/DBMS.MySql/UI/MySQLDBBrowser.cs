/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/



 using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using mysqlLib = MySql;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ICSharpCode.TextEditor;
using DBMS.core;
using System.Drawing.Drawing2D;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System.Text.RegularExpressions;

namespace DBMS.MySQL
{


    public class MySQLDBBrowser :DBBrowserControl, IDBBrowser
    {
        
        private MySqlConnection MySqlConnection;
        private MySqlDataAdapter MySqlDataAdapter;
        private long totalRows = 0;

        private IContainer components;
        private  MySQLCodeCompletionProvider codeCompletionProvider;
        private string connectionString;
        
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public ContextMenuStrip TableMenu
        {
            get { return this.tableMenu; }
        }
        public ContextMenuStrip DataBaseMenu
        {
            get { return this.dataBaseMenu; }
        }
        public ContextMenuStrip ToolsMenu
        {
            get { return this.toolsMenu; }
        }
       

        public Color TreeViewColor
        {
            get { return Color.FromArgb(220, 220, 255);}
        }
        public ImageList ImageList
        {
            get
            {
                return this.imageList1;
            }
        }
        public CodeCompletionProvider CodeCompletionProvider
        {
            get { return codeCompletionProvider; }
            set { codeCompletionProvider = (MySQLCodeCompletionProvider)value; }
        }
       
        #region Menus
        public ContextMenuStrip triggerMenu;
        private ToolStripMenuItem refreshTriggerStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem11;
        private ToolStripMenuItem toolStripMenuItem12;
        public ContextMenuStrip procedureMenu;
        private ToolStripMenuItem refreshProcToolStripMenuItem;
        private ToolStripMenuItem newProcToolStripMenuItem;
        private ToolStripMenuItem alterProcToolStripMenuItem;
        private ToolStripMenuItem dropProcToolStripMenuItem;
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
        public ContextMenuStrip functionMenu;
        private ToolStripMenuItem refreshFunctionsToolStripMenuItem;
        private ToolStripMenuItem newFunctionToolStripMenuItem;
        private ToolStripMenuItem alterFunctionToolStripMenuItem;
        private ToolStripMenuItem dropFunctionToolStripMenuItem;
        public ContextMenuStrip serverMenu;
        private ToolStripMenuItem refresServerhMenuItem;
        private ToolStripMenuItem newQueryMenuItem;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem newDatabaseMenuItem;
        private ToolStripMenuItem restoreDatabaseFromSqlDumpMenuItem;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem reconnectMenuItem;
        private ToolStripMenuItem disconnectMenuItem;
        public ContextMenuStrip viewMenu;
        private ToolStripMenuItem refreshViewsToolStripMenuItem;
        private ToolStripMenuItem newViewToolStripMenuItem;
        private ToolStripMenuItem alterViewToolStripMenuItem;
        private ToolStripMenuItem dropViewToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem14;
        private ToolStripMenuItem showToolStripMenuItem;
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
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem toolStripMenuItem25;
        private ToolStripMenuItem toolStripMenuItem26;
        private ContextMenuStrip toolsMenu;
        private ToolStripMenuItem serverStatusToolStripMenuItem;
        private ToolStripMenuItem tableDiangosticToolStripMenuItem;
        private ToolStripMenuItem flushToolStripMenuItem;
        private ToolStripMenuItem viewDDLDefinitionToolStripMenuItem;
        private ToolStripMenuItem copyDDLDefinitionToolStripMenuItem; 
        private ImageList imageList1;

        #endregion


        private ImageList completionListIcons;
        

       

      
        public MySQLDBBrowser(MySqlConnection connection) : base()
        {
            InitializeComponent();
            //LanguageManager.LanguageSwitcher.Instance().SwitchLanguage(this.GetType().Namespace, this.GetType().Name, this);
            browserIsBusy = false;
            MySqlConnection = connection;
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
            workerQueryExecutor.DoWork+=new DoWorkEventHandler(workerQueryExecutor_DoWork);
            workerQueryExecutor.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(workerQueryExecutor_RunWorkerCompleted);


            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

        }

        #region DBBrowser Members


        public event TableBoundedHandler TableBounded;
        public event NewDataUserViewRequestedHandler NewDataUserViewRequested;
        public event DisconnectRequestedEventHandler DisconnectRequestedEvent;
        public IComponent MenuManager
        {
            get { return null; }
        }

        public  Object Connection
        {
            get
            {
                return this.MySqlConnection;
            }
            set
            {
                this.MySqlConnection = (MySqlConnection)value;
            }
        }

        public  QueryResult ListDatabases()
        {
            try
            {
                string[] rez = null;
                string s = "show databases;";
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection)) 
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack,new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                            rez[i] = dt.Rows[i][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new QueryResult(QueryStatus.Error,ex.Message);
            }

        }

        public  QueryResult ListTables(string database)
        {
            string s = "show table status from " + database + " where engine is not null;";
            MySqlCommand cmd = new MySqlCommand(s, MySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

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
            catch (MySqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error,ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error,ex.Message);
            }
        }

        public  QueryResult GetViews(string database)
        {
            string s = "show table status from " + database + " where engine is  null;";
            MySqlCommand cmd = new MySqlCommand(s, MySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
            catch (MySqlException ex)
            {
                da.Dispose();
                cmd.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetFunctions(string database)
        {
            string s = "select routine_name from information_schema.routines where routine_type='FUNCTION' and routine_schema='" + database + "';";
            MySqlCommand cmd = new MySqlCommand(s, MySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
            catch (MySqlException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetStoredProcedures(string database)
        {
            string s = "select routine_name from information_schema.routines where routine_type='PROCEDURE' and routine_schema='" + database + "';";
            MySqlCommand cmd = new MySqlCommand(s, MySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
            catch (MySqlException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetTriggers(string database)
        {
            string s = "show triggers in `"+database+"`;";
            MySqlCommand cmd = new MySqlCommand(s, MySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
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

        public void ShowTableStatus(string database, string tableName)
        {
            TableStatusForm tForm = new TableStatusForm(database, tableName, this.MySqlConnection);
            tForm.ShowDialog();
        }

        public  string GetConnectionSource()
        {
            return this.MySqlConnection.ServerVersion;
        }

        public  QueryResult GetViewDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  VIEW_DEFINITION FROM `INFORMATION_SCHEMA`.`VIEWS` WHERE table_name = '" + name + "' AND TABLE_SCHEMA='" + dataBase + "';";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        rez = @"CREATE
    /*[ALGORITHM = {UNDEFINED | MERGE | TEMPTABLE}]
    [DEFINER = { user | CURRENT_USER }]
    [SQL SECURITY { DEFINER | INVOKER }]*/
    VIEW `" + dataBase + "`.`" + name + @"` 
    AS " + rez + ";";

                        return new QueryResult((object)rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetStoredProcedureDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE specific_name = '" + name + "' AND ROUTINE_SCHEMA='" + dataBase + "';";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetFunctionDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE specific_name = '" + name + "' AND ROUTINE_SCHEMA='" + dataBase + "';";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetTriggerDefinition(string dataBase, string name)
        {
            try
            {
                string s = "show  create trigger `" + dataBase + "`.`" + name + "`;";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                        if (dt.Rows.Count < 1)
                            return new QueryResult((object)null);
                        rez = dt.Rows[0]["sql original statement"].ToString();
                        return new QueryResult((object)rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }

        }

        public  string GetNewTriggerQuery(string dataBase, string tableName, string name)
        {
            string s = @"DELIMITER $$
CREATE
    /*[DEFINER = { user | CURRENT_USER }]*/
    TRIGGER `"+name+ @"` BEFORE/AFTER INSERT/UPDATE/DELETE
    ON `" + dataBase + "`.`<" + tableName + @">`
    FOR EACH ROW
        BEGIN

        END$$

DELIMITER ;";
            return s;
        }

        public  string GetNewFunctionQuery(string dataBase, string tableName, string name)
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

        public  string GetNewStoredProcedureQuery(string dataBase, string tableName, string name)
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

        public  string GetNewViewQuery(string dataBase, string tableName, string name)
        {
            string s = @"CREATE
/*[ALGORITHM = {UNDEFINED | MERGE | TEMPTABLE}]
[DEFINER = { user | CURRENT_USER }]
[SQL SECURITY { DEFINER | INVOKER }]*/
VIEW `"+dataBase+"`.`"+name+@"` 
AS
    (SELECT * FROM ...);";
            return s;
        }

        public  void ExecuteQuery(string query, DataUserView outPut)
        {
            DateTime start = DateTime.Now;
            outPut.DGResultView.RowCount = 0;
            outPut.DGResultView.SuspendLayout();
            outPut.DGResultView.Rows.Clear();
            outPut.DGResultView.RowCount = 0;
            outPut.DGResultView.ColumnCount = 0;
            workerQueryExecutor.RunWorkerAsync(new object[] { query, outPut, start });
        }

        public  QueryResult ListColumns(string dataBase, string tableName)
        {
            MySqlDataAdapter da = new MySqlDataAdapter() ;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                if (dataBase == null || tableName == null)
                    return null;
                using (DataTable dt = new DataTable())
                {
                    string s = "show columns from `" + dataBase + "`.`" + tableName + "`;";
                    cmd = new MySqlCommand(s, MySqlConnection);
                    da = new MySqlDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    string[] rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i][0].ToString();
                    return new QueryResult(rez);
                }
            }
            catch (InvalidOperationException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(ex.Message);
            }
            catch (MySqlException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult SwitchDataBase(string dataBase)
        {
            try
            {
                string s = "use `" + dataBase + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, this.MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    
                    return new QueryResult(null);
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl shtb)
        {
//            this.history.SetOutputText(shtb);
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

        public  void SetObjectOutput(TextEditorControl shtb)
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

        public  void SetQueryInput(TextEditorControl shtb)
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

        public  QueryResult GetTable(string dataBase, string tableName)
        {

            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "`;";
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit)
        {
            try
            {
                using (DataSet tableSet = new DataSet())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "` limit " + lowLimit + "," + hiLimit + ";";
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetTableStatus(string dataBase)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "show table status from " + dataBase + ";";
                    using (MySqlDataAdapter da = new MySqlDataAdapter(s, MySqlConnection))
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
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
        }

        public  QueryResult GetColumnInformation(string dataBase, string tableName)
        {
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                if (dataBase == null || tableName == null)
                    return null;
                string s = "show columns from `" + dataBase + "`.`" + tableName + "`;";
                cmd = new MySqlCommand(s, MySqlConnection);
                using (DataTable dt = new DataTable())
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
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

        public  QueryResult GetIndexInformation(string dataBase, string tableName)
        {
            MySqlDataAdapter da = new MySqlDataAdapter() ;
            MySqlCommand cmd = new MySqlCommand() ;
            try
            {
                if (dataBase == null || tableName == null)
                    return null;
                string s = "show index from `" + dataBase + "`.`" + tableName + "`;";
                cmd = new MySqlCommand(s, MySqlConnection);
                using (DataTable dt = new DataTable())
                {
                    da = new MySqlDataAdapter(cmd);
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    dt.Columns["Column_name"].ColumnName = "column_name";
                    dt.Columns["key_name"].ColumnName = "key_name";
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
            catch (MySqlException ex)
            {
                cmd.Dispose();
                da.Dispose();
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
    }

        public  string GetDDLInformation(string dataBase, string tableName)
        {
            MySqlDataAdapter da = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand();
            DataTable dt = new DataTable();
            string rez = null;
            string s = "show create table `" + dataBase + "`.`" + tableName + "`;";
            cmd = new MySqlCommand(s,MySqlConnection);
            da = new MySqlDataAdapter(cmd); 
            DateTime start = DateTime.Now;
            da.Fill(dt);
            TimeSpan time = DateTime.Now.Subtract(start);
            asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
            
            if (dt.Rows.Count > 0)
            {
                MemoryStream str = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                if (dt.Rows[0][1].GetType() == typeof(byte[]))
                    rez = Encoding.Default.GetString((byte[])dt.Rows[0][1]).ToLower();
                else
                    rez = dt.Rows[0][1].ToString();
                da.Dispose();
                cmd.Dispose();
                return rez +";";
            }
            da.Dispose();
            cmd.Dispose();
            return null;
        }

        public  QueryResult DropTable(string database, string tableName)
        {
            try
            {
                string s = "drop table `" + database + "`.`" + tableName + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult EmptyTable(string dataBase, string tableName)
        {
            try
            {
                string s = "truncate table `" + dataBase + "`.`" + tableName + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult DropDataBase(string database)
        {
            try
            {
                string s = "drop database `" + database + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    currentNode.Remove();
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult DropProcedure(string database, string name)
        {
            try
            {
                string s ="drop procedure`" + database + "`.`" +name+"`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }

                return new QueryResult("OK");
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult DropView(string database, string name)
        {
            try
            {
                string s="drop view `" + database + "`.`" + name + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
                return new QueryResult("OK");
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult DropFunction(string database, string name)
        {
            try
            {
                string s = "drop function `" + database + "`.`" + name + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }

            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult DropTrigger(string database, string name)
        {
            try
            {
                string s = "drop trigger `" + database + "`.`" + name + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult TruncateTable(string database, string tableName)
        {
            try
            {
                string s = "truncate table `" + database + "`.`" + tableName + "`;";
                using (MySqlCommand cmd = new MySqlCommand(s, MySqlConnection))
                {
                    DateTime start = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TimeSpan time = DateTime.Now.Subtract(start);
                    asyncOperation.Post(postCallBack, new QueryExecutedEventArgs(new HistoryElement(DateTime.Now, s, time.TotalMilliseconds)));
                    return new QueryResult(QueryStatus.OK, 0, null);
                }

            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  QueryResult TruncateDataBase(string database)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("show tables from `" + database + "`;", MySqlConnection))
                    {
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                            TruncateTable(database, dt.Rows[i][0].ToString());
                        return new QueryResult(QueryStatus.OK, 0, null);
                    }
                }
            }
            catch (MySqlException ex)
            {
                return new QueryResult(QueryStatus.Error, ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
        }

        public  void Disconnect()
        {
            this.MySqlConnection.Close();
            this.MySqlConnection.Dispose();
            updateNodeTimer.Stop();
            updateNodeTimer.Dispose();
            //this.Dispose();
        }

        public  void ShowAlterTableForm(string dataBase, string tableName)
        {
            using (AlterTableForm aTForm = new AlterTableForm( this.MySqlConnection, dataBase, tableName, AlterTableForm.Mod.Alter))
            {
                aTForm.ShowDialog();
            }
        }

        public  void ShowReorderColumnForm(string dataBase, string tableName)
        {
            using (ReorderColumnForm rcTForm = new ReorderColumnForm(this.MySqlConnection, dataBase, tableName))
            {
                rcTForm.ShowDialog();
            }
        }

        public  void ShowCreateTableForm(TreeNode node)
        {
            string dataBase = node.Text;
            using (AlterTableForm aTForm = new AlterTableForm(this.MySqlConnection, dataBase, null, AlterTableForm.Mod.Create))
            {
                aTForm.ShowDialog();
                if (aTForm.TableCreated)
                {
                    TreeNode nod = new TreeNode(aTForm.TableName);
                    int i=0;
                    while (i < currentNode.Nodes.Count && aTForm.TableName.CompareTo(currentNode.Nodes[i].Text)>0)
                        i++;
                    nod.Nodes.Add("");
                    nod.ContextMenuStrip = TableMenu;
                    nod.Tag = new object[] { this, SQLNodeType.Table, SQLNodeStatus.Unset };
                    nod.ImageIndex = 2;
                    nod.SelectedImageIndex = 2;
                    node.Nodes.Insert(i, nod);
                }

            }
        }

        public  void ShowIndexesTableForm(string dataBase, string tableName)
        {
            using (IndexForm aTForm = new IndexForm(this.MySqlConnection, dataBase, tableName))
            {
                aTForm.ShowDialog();
            }
        }

        public  void RenameTable(string dataBase, string tableName, string newName)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("rename table `" + dataBase + "`.`" + tableName + "` to `" + dataBase + "`.`" + newName + "`;", MySqlConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public  void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
        {
            BackUpDataForm bf = new BackUpDataForm( this.MySqlConnection,dataBases, tables, selectedDataBase, selectedTable);
            bf.Browser = this;
            bf.ShowDialog();
        }

        public  void ShowExportTableForm(string dataBase, string tableName)
        {
            using (QueryResult qR = this.ListColumns(dataBase,tableName))
            {
                exportData bf = new exportData(this.MySqlConnection, dataBase, tableName, (string[])qR.Result);
                bf.ShowDialog();
            }
        }

        public  void ShowExportTableForm(DataGridView dgView, string[] columns)
        {
                exportData bf = new exportData(dgView, columns);
                bf.ShowDialog();
        }

        public  void ShowExecuteForm(string fileName, Form mdiParent)
        {
            ExecuteBatchForm eForm = new ExecuteBatchForm(fileName, this);
            eForm.Show(mdiParent);
        }

        public  void ShowReferenceTableForm(string dataBase, string tableName)
        {
            ReferenceTableForm rForm = new ReferenceTableForm(dataBase, tableName,this.MySqlConnection);
            rForm.ShowDialog();
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
                MySqlCommand cmd = new MySqlCommand();
                try
                {
                    string s = "use `" + dataBase + "`;show columns from `" + dataBase + "`.`" + tableName + "`;";
                    cmd = new MySqlCommand(s, MySqlConnection);
                    MySqlDataAdapter da = new MySqlDataAdapter();
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

                    if (dt.Rows[i]["key"].ToString().ToLower() == "pri")
                    {
                        TablePrimaryKeys.Add(dt.Rows[i][0].ToString());
                    }
                    string fullType = dt.Rows[i][1].ToString().ToLower();
                    string mainType = ExtractMainTypeFromFullType(fullType);

                    //handle text, set,enum and blob columns
                    switch (mainType)
                    {
                        case "enum":
                            DataGridViewComboBoxColumn c1 = new DataGridViewComboBoxColumn();
                            c1.Resizable = DataGridViewTriState.True;
                            c1.DefaultCellStyle.NullValue = "(null)";
                            string s = dt.Rows[i][1].ToString();
                            s = s.Substring(s.IndexOf('(') + 1, s.LastIndexOf(')') - s.IndexOf('(') - 1);
                            string[] si = s.Split(',');
                            for (int j = 0; j < si.Length; j++)
                            {
                                string cd = si[j].Substring(1, si[j].Length - 2);
                                c1.Items.Add(cd);
                            }
                            c1.DataPropertyName = dt.Rows[i][0].ToString();
                            c1.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                            c1.FlatStyle = FlatStyle.Flat;
                            c1.HeaderText = c1.DataPropertyName;
                            c1.Name = c1.HeaderText;
                            c1.Tag = new object[] { mainType, dt.Rows[i]["default"] };
                            dgTableView.Columns.Add(c1);
                            c1.SortMode = DataGridViewColumnSortMode.Programmatic;
                            break;
                        case "set":
                            string x = dt.Rows[i][1].ToString();
                            string m = x.Substring(x.IndexOf('(') + 1, x.LastIndexOf(')') - x.IndexOf('(') - 1).Replace("'", "");
                            string[] set = m.Split(',');
                            DataGridViewButtonColumn c2 = new DataGridViewButtonColumn();
                            c2.Resizable = DataGridViewTriState.True;
                            c2.DefaultCellStyle.NullValue = "(null)";
                            s = dt.Rows[i][1].ToString();
                            c2.DataPropertyName = dt.Rows[i][0].ToString();
                            c2.FlatStyle = FlatStyle.Flat;
                            c2.HeaderText = c2.DataPropertyName;
                            c2.Name = c2.HeaderText;
                            c2.Tag = new object[] { mainType, set };
                            dgTableView.Columns.Add(c2);
                            c2.SortMode = DataGridViewColumnSortMode.Programmatic;
                            break;

                        case "blob":
                        case "longblob":
                        case "tinyblob":
                        case "mediumblob":
                            DataGridViewButtonColumn c3 = new DataGridViewButtonColumn();
                            c3.DefaultCellStyle.NullValue = "(null)";
                            c3.Resizable = DataGridViewTriState.True;
                            c3.DataPropertyName = dt.Rows[i][0].ToString();
                            c3.HeaderText = c3.DataPropertyName;
                            dgTableView.Columns.Add(c3);
                            c3.Tag = new object[] { mainType, null };
                            c3.Name = c3.HeaderText;
                            break;
                        case "text":
                        case "tinytext":
                        case "mediumtext":
                        case "longtext":
                            DataGridViewButtonColumn c4 = new DataGridViewButtonColumn();
                            c4.Resizable = DataGridViewTriState.True;
                            c4.DefaultCellStyle.NullValue = "(null)";
                            c4.DataPropertyName = dt.Rows[i][0].ToString();
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
                            c5.Frozen = false;
                            c5.Resizable = DataGridViewTriState.True;
                            c5.DataPropertyName = dt.Rows[i][0].ToString();
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
                            c6.DataPropertyName = dt.Rows[i][0].ToString();
                            c6.HeaderText = c6.DataPropertyName;
                            c6.Name = c6.HeaderText;
                            c6.Tag = new object[] { mainType, null };
                            c6.SortMode = DataGridViewColumnSortMode.Programmatic;
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

        private string ExtractMainTypeFromFullType(string fullType)
        {
             Match m = Regex.Match(fullType, "^[\\w-]+");
             if (m.Success)
                 return m.Value.ToLower();
             return "";
        }
       
        public void SaveTable(string tableName, DataUserView dataUserView)
        {
            ExecuteGeneratedUpdateString(dataUserView);
            ExecuteGeneratedInsertString(dataUserView);

        }

        public  void AddNewRowToBoundedTable(DataGridView dgTableView)
        {
            if (dgTableView != null)
            {
                if (dgTableView.ColumnCount < 1)
                    return;
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                cachedRows.Add(new SqlDataGridViewRow(new object[dgTableView.ColumnCount-1],new UpdateInfo(UpdateType.Insert)));
                dgTableView.Rows.Add();
            }
        }

        public  void DeleteSelectedRowsTable(DataUserView dataUserView)
        {
            bool atLeastOne = false;
            int nrPar = 0;
            StringBuilder cmdText = new StringBuilder();
            DataGridView dgTableView = dataUserView.DGTableView;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            dgTableView.EndEdit();
            MySqlCommand cmd = new MySqlCommand("", MySqlConnection);
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
  
                        string rem = "delete from `" + dataUserView.DataBase + "`.`" + dataUserView.TableName + "` where ";
                        int nr = 0;
                        string and = "";
                        for (int i = 0; i < dgTableView.Columns.Count-1; i++)
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
                }
            }
            if (atLeastOne)
            {
                try
                {
                    cmd.CommandText = cmdText.ToString();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public  void ShowFlushForm()
        {
            FlushForm fForm = new FlushForm(MySqlConnection);
            fForm.ShowDialog();
        }

        public  void ShowStatusForm()
        {
            StatusForm fForm = new StatusForm(MySqlConnection);
            fForm.ShowDialog();
        }

        public  void ShowTableDiagnosticForm()
        {
            TableDiagnosticForm fForm = new TableDiagnosticForm(MySqlConnection,this,currentDataBase);
            fForm.ShowDialog();
        }

        public  void ShowNewDbForm(TreeNode node)
        {
            NewDbForm ndbForm = new NewDbForm(MySqlConnection);
            ndbForm.ShowDialog();
            if (ndbForm.DataBaseCreated)
            {
                TreeNode nod = new TreeNode(ndbForm.DataBaseName);
                nod.Nodes.Add("");
                nod.Tag = new object[] { this, SQLNodeType.Database, SQLNodeStatus.Unset };
                int i = 0;
                while (i < currentNode.Nodes.Count && ndbForm.DataBaseName.CompareTo(currentNode.Nodes[i].Text) > 0)
                    i++;
                nod.ImageIndex = 1;
                nod.SelectedImageIndex = 1;
                nod.ContextMenuStrip = dataBaseMenu;
                node.Nodes.Insert(i, nod);
                node.TreeView.SelectedNode = nod;
            }
        }

        public  void ShowImportFromCSVForm(string dataBase, string tableName)
        {
            ImportFromCSV2TableForm fForm = new ImportFromCSV2TableForm(MySqlConnection, this, dataBase, tableName);
            fForm.ShowDialog();
        }

        public  void Reconnect()
        {
            try
            {
                this.MySqlConnection.Close();
                this.MySqlConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public  ContextMenuStrip GetMainMenu()
        {
            return serverMenu;
        }

        public TreeNode CreateMainTreeNode()
        {
            TreeNode nodS = new TreeNode(GetConnectionSource() + "/" + ServerAddress, 0, 0);
            nodS.Tag = new object[] { this, SQLNodeType.Server, SQLNodeStatus.Refreshed};
            nodS.ContextMenuStrip = this.GetMainMenu();
            nodS.ImageIndex = 0;
            nodS.SelectedImageIndex =  0;
            nodS.Nodes.Add("-");
            currentNode = nodS;
            return nodS;
        }

        private void serverMenuItem_click(object sender, EventArgs e)
        {
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
            string database = dbNode.Text;
            codeCompletionProvider.AddDataBase(database);
            ret.Add(dbNode);
            string[] tables = new string[] { };
            using (MySqlDataAdapter da = new MySqlDataAdapter())
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
                            nod.Tag = new object[] { this, SQLNodeType.Table, SQLNodeStatus.Unset };
                            nod.ContextMenuStrip = tableMenu;
                            TreeNode cNode = new TreeNode("Columns", 3, 3);
                            cNode.Tag = new object[] { this, SQLNodeType.Column, SQLNodeStatus.Unset };
                            cNode.Nodes.Add("-");
                            nod.Nodes.Add(cNode);
                            TreeNode iNode = new TreeNode("Index", 3, 3);
                            iNode.Tag = new object[] { this, SQLNodeType.Index, SQLNodeStatus.Unset };
                            iNode.Nodes.Add("-");
                            nod.Nodes.Add(iNode);
                            ret.Add(nod);

                                MySqlCommand cmdC = new MySqlCommand();
                                using (DataTable dt = new DataTable())
                                {
                                    string s = "show columns from `" + dbNode.Text + "`.`" + tables[i] + "`;";
                                    cmdC = new MySqlCommand(s, MySqlConnection);
                                    da.SelectCommand=  cmdC;
                                    da.Fill(dt);
                                    string [] cols = new string[]{};
                                    if(dt.Rows.Count>0)
                                    {
                                        cols = new string[dt.Rows.Count];
                                        for (int k = 0; k < dt.Rows.Count; k++)
                                        {
                                            cols[k] = dt.Rows[k][0].ToString();
                                        }
                                        codeCompletionProvider.AddColumnCompletionToTableDB(database, tables[i], cols);
                                    }
                                }
                            }
                        }
                    }
                    TreeNode vNode = new TreeNode("Views",  3,  3);
                    vNode.Tag = new object[] { this, SQLNodeType.View, SQLNodeStatus.Unset };
                    vNode.ContextMenuStrip = viewMenu;
                    vNode.Nodes.Add("-");
                    TreeNode pNode = new TreeNode("Procedures",  3,  3);
                    pNode.ContextMenuStrip = procedureMenu;
                    pNode.Tag = new object[] { this, SQLNodeType.Procedure, SQLNodeStatus.Unset };
                    pNode.Nodes.Add("-");
                    TreeNode fNode = new TreeNode("Functions",  3,  3);
                    fNode.ContextMenuStrip = functionMenu;
                    fNode.Tag = new object[] { this, SQLNodeType.Function, SQLNodeStatus.Unset };
                    fNode.Nodes.Add("-");
                    TreeNode tNode = new TreeNode("Triggers",  3,  3);
                    tNode.ContextMenuStrip = triggerMenu;
                    tNode.Tag = new object[] { this, SQLNodeType.Trigger, SQLNodeStatus.Unset };
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
            cNode.Tag = new object[] { this, SQLNodeType.Column, SQLNodeStatus.Refreshed };
            for (int i = 0; i < cols.Rows.Count; i++)
            {
                string id = cols.Rows[i]["Field"].ToString();
                string type = cols.Rows[i]["Type"].ToString();
                string nul = cols.Rows[i]["Null"].ToString().ToUpper() == "YES" ? "null" : "not null";
                TreeNode nodCol = new TreeNode(id + " [" + type + " , " + nul + "]", 7,  7);
                nodCol.Tag = new object[] { this, SQLNodeType.Column,SQLNodeStatus.Refreshed};
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
                    TreeNode nodInd = new TreeNode(type + " ,[" + col + "]",  8, 8);
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
                    string [] views = (string[])qRv.Result;
                    if(views!=null)
                    for(int i=0;i<views.Length;i++)
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
                    if(proc!=null)
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
                    if(trigg!=null)
                    for (int i = 0; i < trigg.Length; i++)
                    {
                        TreeNode node = new TreeNode(trigg[i], 9, 9);
                        node.ContextMenuStrip = triggerMenu;
                        node.Tag = new object[] { this, SQLNodeType.Trigger, SQLNodeStatus.Refreshed};
                        ret.Add(node);
                    }
                }
            }
            return ret;
        }



        #endregion

        #region privates

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
            MySqlDataAdapter da = new MySqlDataAdapter(q, MySqlConnection);

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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySQLDBBrowser));
            this.triggerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshTriggerStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.procedureMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.functionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refresServerhMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.newDatabaseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreDatabaseFromSqlDumpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.reconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alterViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem25 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem26 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.serverStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableDiangosticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.completionListIcons = new System.Windows.Forms.ImageList(this.components);
            this.triggerMenu.SuspendLayout();
            this.procedureMenu.SuspendLayout();
            this.dataBaseMenu.SuspendLayout();
            this.functionMenu.SuspendLayout();
            this.serverMenu.SuspendLayout();
            this.viewMenu.SuspendLayout();
            this.tableMenu.SuspendLayout();
            this.toolsMenu.SuspendLayout();
            this.SuspendLayout();
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
            this.triggerMenu.Opening += new System.ComponentModel.CancelEventHandler(this.triggerMenu_Opening);
            // 
            // refreshTriggerStripMenuItem
            // 
            this.refreshTriggerStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshTriggerStripMenuItem.Image")));
            this.refreshTriggerStripMenuItem.Name = "refreshTriggerStripMenuItem";
            this.refreshTriggerStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.refreshTriggerStripMenuItem.Text = "Refresh";
            this.refreshTriggerStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem22_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem11.Image")));
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem11.Text = "Alter trigger";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.toolStripMenuItem11_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem12.Image")));
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem12.Text = "Drop trigger";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.toolStripMenuItem12_Click);
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
            this.procedureMenu.Opening += new System.ComponentModel.CancelEventHandler(this.procedureMenu_Opening);
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
            this.newProcToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // alterProcToolStripMenuItem
            // 
            this.alterProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterProcToolStripMenuItem.Image")));
            this.alterProcToolStripMenuItem.Name = "alterProcToolStripMenuItem";
            this.alterProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.alterProcToolStripMenuItem.Text = "Alter proccedure";
            this.alterProcToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // dropProcToolStripMenuItem
            // 
            this.dropProcToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropProcToolStripMenuItem.Image")));
            this.dropProcToolStripMenuItem.Name = "dropProcToolStripMenuItem";
            this.dropProcToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.dropProcToolStripMenuItem.Text = "Drop procedure";
            this.dropProcToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
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
            this.dataBaseMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dataBaseMenu_Opening);
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
            // alterDatabaseToolStripMenuItem
            // 
            this.alterDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterDatabaseToolStripMenuItem.Image")));
            this.alterDatabaseToolStripMenuItem.Name = "alterDatabaseToolStripMenuItem";
            this.alterDatabaseToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.alterDatabaseToolStripMenuItem.Text = "Alter";
            this.alterDatabaseToolStripMenuItem.Click += new System.EventHandler(this.alterDatabaseToolStripMenuItem_Click);
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
            this.backupToolStripMenuItem.Click += new System.EventHandler(this.backupToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importToolStripMenuItem.Image")));
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.importToolStripMenuItem.Text = "Restore from sql dump";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
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
            this.functionMenu.Opening += new System.ComponentModel.CancelEventHandler(this.functionMenu_Opening);
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
            this.newFunctionToolStripMenuItem.Click += new System.EventHandler(this.newFunctionToolStripMenuItem_Click);
            // 
            // alterFunctionToolStripMenuItem
            // 
            this.alterFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alterFunctionToolStripMenuItem.Image")));
            this.alterFunctionToolStripMenuItem.Name = "alterFunctionToolStripMenuItem";
            this.alterFunctionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.alterFunctionToolStripMenuItem.Text = "Alter function";
            this.alterFunctionToolStripMenuItem.Click += new System.EventHandler(this.alterFunctionToolStripMenuItem_Click);
            // 
            // dropFunctionToolStripMenuItem
            // 
            this.dropFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dropFunctionToolStripMenuItem.Image")));
            this.dropFunctionToolStripMenuItem.Name = "dropFunctionToolStripMenuItem";
            this.dropFunctionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.dropFunctionToolStripMenuItem.Text = "Drop function";
            this.dropFunctionToolStripMenuItem.Click += new System.EventHandler(this.dropFunctionToolStripMenuItem_Click);
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
            this.refresServerhMenuItem.Click += new System.EventHandler(this.refreshMenuItem_Click);
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
            // newDatabaseMenuItem
            // 
            this.newDatabaseMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newDatabaseMenuItem.Image")));
            this.newDatabaseMenuItem.Name = "newDatabaseMenuItem";
            this.newDatabaseMenuItem.Size = new System.Drawing.Size(245, 22);
            this.newDatabaseMenuItem.Text = "New database";
            this.newDatabaseMenuItem.Click += new System.EventHandler(this.newDatabaseMenuItem_Click);
            // 
            // restoreDatabaseFromSqlDumpMenuItem
            // 
            this.restoreDatabaseFromSqlDumpMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreDatabaseFromSqlDumpMenuItem.Image")));
            this.restoreDatabaseFromSqlDumpMenuItem.Name = "restoreDatabaseFromSqlDumpMenuItem";
            this.restoreDatabaseFromSqlDumpMenuItem.Size = new System.Drawing.Size(245, 22);
            this.restoreDatabaseFromSqlDumpMenuItem.Text = "Restore database from sql dump";
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
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showToolStripMenuItem.Image")));
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
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
            this.tableMenu.Size = new System.Drawing.Size(209, 308);
            this.tableMenu.Opening += new System.ComponentModel.CancelEventHandler(this.tableMenu_Opening);
            // 
            // refreshTabletoolStripMenuItem
            // 
            this.refreshTabletoolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshTabletoolStripMenuItem.Image")));
            this.refreshTabletoolStripMenuItem.Name = "refreshTabletoolStripMenuItem";
            this.refreshTabletoolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.refreshTabletoolStripMenuItem.Text = "Refresh";
            this.refreshTabletoolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem15_Click);
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
            this.reorderColumnsToolStripMenuItem.Click += new System.EventHandler(this.reorderColumnsToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // truncateToolStripMenuItem1
            // 
            this.truncateToolStripMenuItem1.Name = "truncateToolStripMenuItem1";
            this.truncateToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.truncateToolStripMenuItem1.Text = "Truncate";
            this.truncateToolStripMenuItem1.Click += new System.EventHandler(this.truncateToolStripMenuItem1_Click);
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
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asSqldumpToolStripMenuItem,
            this.asCSVFileToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // asSqldumpToolStripMenuItem
            // 
            this.asSqldumpToolStripMenuItem.Name = "asSqldumpToolStripMenuItem";
            this.asSqldumpToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.asSqldumpToolStripMenuItem.Text = "As SQL dump";
            this.asSqldumpToolStripMenuItem.Click += new System.EventHandler(this.asSqldumpToolStripMenuItem_Click);
            // 
            // asCSVFileToolStripMenuItem
            // 
            this.asCSVFileToolStripMenuItem.Name = "asCSVFileToolStripMenuItem";
            this.asCSVFileToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.asCSVFileToolStripMenuItem.Text = "As ...";
            this.asCSVFileToolStripMenuItem.Click += new System.EventHandler(this.asCSVFileToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.importToolStripMenuItem1.Text = "Import from CSV file";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
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
            // copyDDLDefinitionToolStripMenuItem
            // 
            this.copyDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyDDLDefinitionToolStripMenuItem.Image")));
            this.copyDDLDefinitionToolStripMenuItem.Name = "copyDDLDefinitionToolStripMenuItem";
            this.copyDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.copyDDLDefinitionToolStripMenuItem.Text = "Copy DDL definition";
            this.copyDDLDefinitionToolStripMenuItem.Click += new System.EventHandler(this.copyDDLDefinitionToolStripMenuItem_Click);
            // 
            // viewDDLDefinitionToolStripMenuItem
            // 
            this.viewDDLDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewDDLDefinitionToolStripMenuItem.Image")));
            this.viewDDLDefinitionToolStripMenuItem.Name = "viewDDLDefinitionToolStripMenuItem";
            this.viewDDLDefinitionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewDDLDefinitionToolStripMenuItem.Text = "View DDL definition";
            this.viewDDLDefinitionToolStripMenuItem.Click += new System.EventHandler(this.viewDDLDefinitionToolStripMenuItem_Click);
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
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem16.Image")));
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem16.Text = "Server status";
            // 
            // toolStripMenuItem25
            // 
            this.toolStripMenuItem25.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem25.Image")));
            this.toolStripMenuItem25.Name = "toolStripMenuItem25";
            this.toolStripMenuItem25.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem25.Text = "Table diagnostic";
            // 
            // toolStripMenuItem26
            // 
            this.toolStripMenuItem26.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem26.Image")));
            this.toolStripMenuItem26.Name = "toolStripMenuItem26";
            this.toolStripMenuItem26.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem26.Text = "Flush";
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
            this.serverStatusToolStripMenuItem.Click += new System.EventHandler(this.serverStatusToolStripMenuItem_Click);
            // 
            // tableDiangosticToolStripMenuItem
            // 
            this.tableDiangosticToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tableDiangosticToolStripMenuItem.Image")));
            this.tableDiangosticToolStripMenuItem.Name = "tableDiangosticToolStripMenuItem";
            this.tableDiangosticToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.tableDiangosticToolStripMenuItem.Text = "Table diagnostic";
            this.tableDiangosticToolStripMenuItem.Click += new System.EventHandler(this.tableDiangosticToolStripMenuItem_Click);
            // 
            // flushToolStripMenuItem
            // 
            this.flushToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("flushToolStripMenuItem.Image")));
            this.flushToolStripMenuItem.Name = "flushToolStripMenuItem";
            this.flushToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.flushToolStripMenuItem.Text = "Flush";
            this.flushToolStripMenuItem.Click += new System.EventHandler(this.flushToolStripMenuItem_Click);
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
            // MySQLDBBrowser
            // 
            this.Name = "MySQLDBBrowser";
            this.triggerMenu.ResumeLayout(false);
            this.procedureMenu.ResumeLayout(false);
            this.dataBaseMenu.ResumeLayout(false);
            this.functionMenu.ResumeLayout(false);
            this.serverMenu.ResumeLayout(false);
            this.viewMenu.ResumeLayout(false);
            this.tableMenu.ResumeLayout(false);
            this.toolsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #region dgBoundedTable

        private void worker_DoWork( object sender, DoWorkEventArgs e)
        {
            object[] arg = (object[])e.Argument;
            string q = (string)arg[1];
            DataGridView dgTableView = (DataGridView)arg[2]; ////
            MySqlDataAdapter = new MySqlDataAdapter(q, MySqlConnection);
            MySqlCommand cmd = new MySqlCommand(q, MySqlConnection);
            MySqlDataReader reader = null ;
            DataTable data = new DataTable();
            try
            {
                if (MySqlConnection.State == ConnectionState.Closed)
                    MySqlConnection.Open();
                reader = cmd.ExecuteReader();
                int nr = 50;
                List<object[]> rez = new List<object[]>();
                int i = 0;
                int k = 0;
                object[] temp = new object[reader.FieldCount];
                while (reader.Read())
                {
                    temp = new object[reader.FieldCount];
                    reader.GetValues(temp);
                    rez.Add(temp);
                    k++;
                    i++;
                    if (i%nr==0)
                    {
                        workerBinder.ReportProgress(0, new object[] { dgTableView, rez });
                        rez = new List<object[]>();
                        k = 0;
                        ready.WaitOne();
                    }
                }
                totalRows = i;
                if(k<nr&&k>0)
                    workerBinder.ReportProgress(0, new object[] { dgTableView, rez });
                reader.Close();
            }
            catch (Exception ex)
            {
                e.Result = new object[] { q, null, dgTableView ,ex.Message};
                if(reader!=null)
                    reader.Close();
                return;
            }
            e.Result = new object[] { q,data,dgTableView ,null};
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string error = null;
            object[] rez = (object[])e.Result;
            DataGridView dgTableView = (DataGridView)rez[2];
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if(totalRows>0)
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
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>) dgTableView.Tag;
                List<object[]> arg = (List<object[]>)argp[1];
                if (arg != null)
                {
                    for (int i = 0; i < arg.Count; i++)
                    {
                        for (int j = 0; j < arg[i].Length; j++)
                        {
                            if (arg[i][j].GetType() == typeof(mysqlLib.Data.Types.MySqlDateTime))
                            {

                                mysqlLib.Data.Types.MySqlDateTime dt = (mysqlLib.Data.Types.MySqlDateTime)arg[i][j];
                                    if (arg[i][j].ToString().Length > 10)
                                    // datetime, timestamp
                                    arg[i][j] = string.Format("{0}-{1}-{2} {3}:{4}:{5}",dt.Year,dt.Month,dt.Day, dt.Hour, dt.Minute, dt.Second);
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
                ready.Set();
            }
        }


        #endregion

        #region ForTableSaving

        private void ExecuteGeneratedInsertString(DataUserView dataUserView)
        {
                DataGridView dgTableView = dataUserView.DGTableView;
                int par = 0;
                MySqlCommand cmd = new MySqlCommand("", MySqlConnection);
                StringBuilder strb = new StringBuilder();
                DataTable dtc = new DataTable();
                cmd.CommandText = "show columns from `" + dataUserView.DataBase + "`.`" + dataUserView.TableName + "`;";
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dtc);
                List<string> autoIncrementedColumns = new List<string>();
                for (int k = 0; k < dtc.Rows.Count; k++)
                {
                    if (dtc.Rows[k]["extra"].ToString().Contains("auto_increment"))
                        autoIncrementedColumns.Add(dtc.Rows[k]["field"].ToString());
                }

                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                for (int i = 0; i < cachedRows.Count; i++)
                {

                    if (cachedRows[i].Tag != null)
                    {

                          
                            UpdateInfo ui = (UpdateInfo)cachedRows[i].Tag;
                            if (ui.tipUpdate == UpdateType.Insert)
                            {
                                try
                                {
                                    strb.Length = 0;
                                    string vstr = "";
                                    strb.Append("insert into `");
                                    strb.Append(dataUserView.DataBase);
                                    strb.Append("`.`");
                                    strb.Append(dataUserView.TableName);
                                    strb.Append("` (");
                                    vstr = "";
                                    for (int j = 1; j < dgTableView.Columns.Count; j++)
                                    {
                                        strb.Append(vstr);
                                        strb.Append("`" + dgTableView.Columns[j].Name +"`");
                                        vstr = ",";
                                    }
                                    strb.Append(" ) values (");
                                    vstr = "";
                                    for (int j = 1; j < dgTableView.Columns.Count; j++)
                                    {
                                        strb.Append(vstr);
                                        strb.Append("@par" + par.ToString());
                                        vstr = ",";
                                        cmd.Parameters.AddWithValue("@par" + par.ToString(), dgTableView.Rows[i].Cells[j].Value);
                                        par++;
                                    }
                                    strb.Append(");");
                                    cachedRows[i].Tag = null;
                                    cmd.CommandText = strb.ToString();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                    }
                }

        }

        private void ExecuteGeneratedUpdateString( DataUserView dataUserView)
        {
                DataGridView dgTableView = dataUserView.DGTableView;
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                int par = 0;
                MySqlCommand cmd = new MySqlCommand("", MySqlConnection);
                StringBuilder strb = new StringBuilder();
                for (int i = 0; i < dgTableView.RowCount; i++)
                {
                    if (cachedRows[i].Tag != null)
                    {
                        strb.Length=0;
                        UpdateInfo ui = (UpdateInfo)cachedRows[i].Tag;
                        if (ui.tipUpdate == UpdateType.Update)
                        {
                            strb.Append("update `" + dataUserView.DataBase + "`.`" + dataUserView.TableName + "` set ");
                                for (int k = 0; k < ui.elements.Count; k++)
                                {
                                    strb.Append("`");
                                    strb.Append(ui.elements[k].columnName);
                                    strb.Append("`");
                                    strb.Append("=@par" + par.ToString());
                                    strb.Append(",");
                                    cmd.Parameters.AddWithValue("@par" + par.ToString(), cachedRows[i].Data[ui.elements[k].columnIndex-1]);
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
                                            if (cachedRows[i].Data[k - 1] != DBNull.Value )
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
                                            if (ui.elements[id].oldValue!= DBNull.Value )
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
                                                    if (dgTableView.Rows[i].Cells[j].Value !=null)
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



        private void newDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowNewDbForm(currentNode);
        }

        private void restoreDatabaseFromSqlDumpMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowExecuteForm("", this.ParentForm);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshNode(currentNode);
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowCreateTableForm(currentNode);
        }

        private void alterDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AlterDataBase aDBForm = new AlterDataBase(this.MySqlConnection, currentNode.Text))
            {
                aDBForm.ShowDialog();
            }
        }

        private void dropDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to drop `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
           QueryResult q = this.DropDataBase(this.currentDataBase);
           if (q.Status == QueryStatus.Error)
               MessageBox.Show(q.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void truncateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to drop `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            QueryResult q = this.TruncateDataBase(currentDataBase);
            if (q.Status == QueryStatus.Error)
                MessageBox.Show(q.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void refreshMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.RefreshNode(currentNode);
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.RefreshNode(currentNode);
        }

        private void alterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowAlterTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void manageIndexesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowIndexesTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void viewReletionshipforeignKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowReferenceTableForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void reorderColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowReorderColumnForm(currentNode.Parent.Text, currentNode.Text);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowTableStatus(currentNode.Parent.Text, currentNode.Text);
        }

        private void truncateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Are you sure you want to drop the table `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            QueryResult qres =this.TruncateTable(currentNode.Parent.Text, currentNode.Text); 
            if (qres.Status == QueryStatus.Error)
            {
                MessageBox.Show(qres.Message, "Error" + qres.ErrorNo);
            }
        }

        private void dropTabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
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
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Are you sure you want to drop the table `" + currentNode.Text + "`?\n\r You will loss your data.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            QueryResult qres = this.EmptyTable(currentNode.Parent.Text, currentNode.Text);
            if (qres.Status == QueryStatus.Error)
                MessageBox.Show(qres.Message, "Error" + qres.ErrorNo);
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TreeNodeIsTableNode(currentNode))
            {
                //currentNode.TreeView.LabelEdit = true;
                currentNode.BeginEdit();
            }
        }

        private void asSqldumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
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
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            TreeNode n = currentNode;
            if (n == null)
                return;
            if ((n.Level != 2 && n.Tag != null) || n.Parent == null)
                return;
            ShowExportTableForm(n.Parent.Text, n.Text);
        }

        private void createTriggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewTriggerQuery(currentNode.Parent.Text, currentNode.Text,"newTrigger"));
        }

        private void newQueryMenuItem_Click(object sender, EventArgs e)
        {
            this.NewDataUserViewRequested(this, null);
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            this.RefreshNode(currentNode);
        }

        private void newViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewViewQuery(currentNode.Parent.Text, currentTable, "newView"));
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewStoredProcedureQuery(currentNode.Parent.Text, currentTable, "newProcedure"));
        }

        private void newFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewFunctionQuery(currentNode.Parent.Text, currentTable, "newFunction"));
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
                NewDataUserViewRequested(this, GetNewTriggerQuery(currentNode.Parent.Text, currentTable, "newTrigger"));
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP TRIGGER \"" + currentNode.Text + "\";";
                sql += Environment.NewLine + GetTriggerDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this, sql);
            }
        }

        private void alterFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP VIEW \"" + currentNode.Text + "\";";
                sql += Environment.NewLine + GetFunctionDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this, sql);
            }
        }

        private void alterViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP FUNCTION \"" + currentNode.Text + "\";";
                sql += Environment.NewLine + GetViewDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this, sql);
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (this.NewDataUserViewRequested != null)
            {
                string sql = "DROP PROCEDURE \"" + currentNode.Text + "\";";
                sql += Environment.NewLine + GetStoredProcedureDefinition(currentNode.Parent.Parent.Text, currentNode.Text).Result.ToString();
                NewDataUserViewRequested(this, sql);
            }
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

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
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

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sure you want to drop procedure " + currentNode.Text + "?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            QueryResult rez = DropProcedure(currentNode.Parent.Parent.Text, currentNode.Text);
            currentNode.Remove();
            currentNode = null;
            if (rez.Message.ToString() == "OK")
                return;
            if (rez.ErrorNo != 0)
                MessageBox.Show(rez.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dropFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sure you want to drop procedure " + currentNode.Text + "?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            QueryResult rez = DropFunction(currentNode.Parent.Parent.Text, currentNode.Text);
            currentNode.Remove();
            currentNode = null;
            if (rez.Message.ToString() == "OK")
                return;
            if (rez.ErrorNo != 0)
                MessageBox.Show(rez.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            if (this.DisconnectRequestedEvent != null)
                DisconnectRequestedEvent(this, currentNode);
        }
       
        private void serverStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusForm();
        }

        private void tableDiangosticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTableDiagnosticForm();
        }
        
        private void flushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFlushForm();
        }

        private void viewDDLDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
				try
				{
	                MySqlCommand cmd = new MySqlCommand("show create table `" + currentNode.Parent.Text + "`.`" + currentNode.Text + "`;", this.MySqlConnection);
	                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
	                DataTable dt = new DataTable();
	                ad.Fill(dt);
	                if(dt.Rows.Count<1)
	                	return;
	                string s = dt.Rows[0][1].ToString();
	                TextForm tf = new TextForm(s);
	                tf.Show(this.parentForm);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}	
        }
        
        private void copyDDLDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("show create table `" + currentNode.Parent.Text + "`.`" + currentNode.Text + "`;", this.MySqlConnection);
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count < 1)
                    return;
                string s = dt.Rows[0][1].ToString();
                Clipboard.SetText(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tableMenu_Opening(object sender, CancelEventArgs e)
        {
            if (TreeNodeIsTableNode(currentNode))
                tableMenu.Enabled = true;
            else
                tableMenu.Enabled = false;
        }
         
        private void dataBaseMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode == null)
                return;
            if (currentNode.Level == 1)
                dataBaseMenu.Enabled = true;
            else
                dataBaseMenu.Enabled = false;
        }
    
        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (BrowserIsBusy)
            {
                MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            this.ShowImportFromCSVForm(currentNode.Parent.Text, currentNode.Text);
        }



        private void swowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!browserIsBusy)
            {

            }
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            RefreshNode(currentNode);
        }

        private void triggerMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                toolStripMenuItem11.Visible = false;
                toolStripMenuItem12.Visible = false;
                refreshTriggerStripMenuItem.Visible = true;
            }
            else
            {
                refreshTriggerStripMenuItem.Visible = false;
                toolStripMenuItem11.Visible = true;
                toolStripMenuItem12.Visible = true;
            }
        }

        private void viewMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                newViewToolStripMenuItem.Visible = true;
                alterViewToolStripMenuItem.Visible = false;
                dropViewToolStripMenuItem.Visible = false;
                refreshViewsToolStripMenuItem.Visible = true;
                showToolStripMenuItem.Visible = false;
            }
            else
            {
                newViewToolStripMenuItem.Visible = false;
                alterViewToolStripMenuItem.Visible = true;
                dropViewToolStripMenuItem.Visible = true;
                refreshViewsToolStripMenuItem.Visible = false;
                showToolStripMenuItem.Visible = true;
            }
        }

        private void functionMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                newFunctionToolStripMenuItem .Visible = true;
                alterFunctionToolStripMenuItem.Visible = false;
                dropFunctionToolStripMenuItem.Visible = false;
                refreshFunctionsToolStripMenuItem.Visible = true;
            }
            else
            {
                newFunctionToolStripMenuItem.Visible = false;
                alterFunctionToolStripMenuItem.Visible = true;
                dropFunctionToolStripMenuItem.Visible = true;
                refreshFunctionsToolStripMenuItem.Visible = false ;
            }
        }

        private void procedureMenu_Opening(object sender, CancelEventArgs e)
        {
            if (currentNode.Level == 2)
            {
                newProcToolStripMenuItem.Visible = true;
                alterProcToolStripMenuItem.Visible = false;
                dropProcToolStripMenuItem.Visible = false;
                refreshProcToolStripMenuItem.Visible = true;
            }
            else
            {
                newProcToolStripMenuItem.Visible = false;
                alterProcToolStripMenuItem.Visible = true;
                dropProcToolStripMenuItem.Visible = true;
                refreshProcToolStripMenuItem.Visible = false;
            }
        }
  
		#endregion

        public void SaveLanguage()
        {


            //AlterTableForm a1 = new AlterTableForm(this.MySqlConnection, null, null, AlterTableForm.Mod.Alter);
            //BackUpDataForm a2 = new BackUpDataForm();
            //ExecuteBatchForm a3 = new ExecuteBatchForm(null, null);
            //ExportData a4 = new ExportData();
            //FlushForm a5 = new FlushForm();
            //ImportFromCSV2TableForm a6 = new ImportFromCSV2TableForm(null,this,null,null);
            //IndexForm a7 = new IndexForm();
            //InputNewIndexForm a8 = new InputNewIndexForm();
            //NewDbForm a9 = new NewDbForm();
            //NewReferenceForm a10 = new NewReferenceForm();
            //ReferenceTableForm a11 = new ReferenceTableForm();
            //ReorderColumnForm a12 = new ReorderColumnForm();
            //SetTypeForm a13 = new SetTypeForm(0, 0, null, null, 100);
            //StatusForm a14 = new StatusForm(this.MySqlConnection);
            //TableDiagnosticForm a15 = new TableDiagnosticForm(this.MySqlConnection, this, null);
            //TableStatusForm a16 = new TableStatusForm();
            //TypeCalendarForm a17 = new TypeCalendarForm(0, 0, null, 200);

          


            //LanguageSwitcher.Instance().SaveControlLanguage(new Control[] { a1,a2,a3,a4,a5,a6,a7,a8,a9});
            //LanguageSwitcher.Instance().SaveControlLanguage(new Control[] { a10, a11, a12, a13, a14, a15, a16, a17});
  
            //MySqlconnectionBuilder a18 = new MySqlconnectionBuilder();
            //LanguageSwitcher.Instance().SaveControlLanguage(a18);
            //LanguageSwitcher.Instance().SaveControlLanguage(this);

        }



        #endregion



        private MySQLCodeCompletionProvider GenerateCompletionData()
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
            return new MySQLCodeCompletionProvider(completionListIcons, lc);
        }

        protected override void OnDataBaseChanged(string currentDataBase)
        {
            codeCompletionProvider.CurrentDataBase = currentDataBase;
            MySqlCommand cmd = new MySqlCommand("use `" + currentDataBase + "`;", MySqlConnection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

 
    }









}


