using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Collections;
using System.Data;
using ICSharpCode.TextEditor;
using System.Windows.Forms;
using System.ComponentModel;
namespace DBMS.core
{
    public abstract class DBBrowser : IDisposable
    {
        private Object connection;
        public abstract Object Connection { get;set;}

        protected TextEditorControl historyText;
        protected TextEditorControl objectText;
        protected DataSet dataSet;
        private string currentDataBase;

        public abstract IComponent MenuManager{get;}
        private Form masterForm;

        public Form MasterForm
        {
            get { return masterForm; }
            set { masterForm = value; }
        }
        
        protected History history;

        private string serverAddress;

        public string ServerAddress
        {
            get { return serverAddress; }
            set { serverAddress = value; }
        }

        public string CurrentDataBase
        {
            get { return currentDataBase; }
            set { currentDataBase = value; }
        }
        private string currentTable;

        public string CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; }
        }

        private bool tableHasPrimaryKeys;
        public bool TableHasPrimaryKeys
        {
            get { return tablePrimaryKeys.Count > 0; }
        }

        private List<string> tablePrimaryKeys;

        public List<string> TablePrimaryKeys
        {
            get { return tablePrimaryKeys; }
            set { tablePrimaryKeys = value; }
        }

        public DBBrowser(object connection)
        {
            dataSet = new DataSet();
            history = new History();
            this.connection = connection;
            tableHasPrimaryKeys = false;
            tablePrimaryKeys = new List<string>();
        }

        public virtual void Dispose()
        {
            GC.Collect();
        }



        public abstract event QueryExecutedHandler QueryExecuted;
        public abstract event TableBoundedHandler TableBounded;
        public abstract event TreeNodeRefreshedHandler TreeNodeRefreshed;


        public abstract QueryResult ListDatabases();
        public abstract QueryResult ListTables(string dataBase);
        public abstract QueryResult ListColumns(string dataBase, string tableName);
        public abstract QueryResult GetViews(string dataBase);
        public abstract QueryResult GetStoredProcedures(string dataBase);
        public abstract QueryResult GetFunctions(string dataBase);
        public abstract QueryResult GetTriggers(string dataBase);
        public abstract void ShowTableStatus(string dataBase, string table);

        public abstract QueryResult GetViewDefinition(string dataBase, string name);
        public abstract QueryResult GetStoredProcedureDefinition(string dataBase, string name);
        public abstract QueryResult GetFunctionDefinition(string dataBase, string name);
        public abstract QueryResult GetTriggerDefinition(string dataBase, string name);
        public abstract void ExecuteQuery(string query, DataGridView output);


        public abstract string GetConnectionSource();
        public abstract QueryResult SwitchDataBase(string dataBase);
        public abstract void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        public abstract void SetObjectOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        public abstract void SetQueryInput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        public abstract QueryResult GetTable(string dataBase, string tableName);
        public abstract QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit);
        public abstract QueryResult GetTableStatus(string dataBase);
        public abstract QueryResult GetColumnInformation(string dataBase, string tableName);
        public abstract QueryResult GetIndexInformation(string dataBase, string tableName);
        public abstract string GetDDLInformation(string dataBase, string tableName);
        public abstract string GetNewTriggerQuery(string dataBase, string tableName, string name);
        public abstract string GetNewFunctionQuery(string dataBase, string tableName, string name);
        public abstract string GetNewStoredProcedureQuery(string dataBase, string tableName, string name);
        public abstract string GetNewViewQuery(string dataBase, string tableName, string name);
        public abstract void Disconnect();
        public abstract QueryResult DropTable(string dataBase, string tableName);
        public abstract QueryResult DropProcedure(string database, string name);
        public abstract QueryResult DropView(string database, string name);
        public abstract QueryResult DropFunction(string database, string name);
        public abstract QueryResult DropTrigger(string database, string name);
        public abstract QueryResult EmptyTable(string dataBase, string tableName);
        public abstract QueryResult DropDataBase(string dataBase);
        public abstract QueryResult TruncateDataBase(string dataBase);
        public abstract QueryResult TruncateTable(string dataBase, string tableName);
        public abstract void RenameTable(string dataBase, string tableName, string newName);
        public abstract void ShowAlterTableForm(string dataBase, string tableName);
        public abstract void ShowFlushForm();
        public abstract void ShowReorderColumnForm(string dataBase, string tableName);
        public abstract void ShowCreateTableForm(string dataBase);
        public abstract void ShowIndexesTableForm(string dataBase, string tableName);
        public abstract void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable);
        public abstract void ShowExportTableForm(string dataBase, string tableName);
        public abstract void ShowExportTableForm(DataGridView dgView);
        public abstract void ShowReferenceTableForm(string dataBase, string tableName);
        public abstract void ShowExecuteForm(string fileName, Form mdiParent);
        public abstract void ShowImportFromCSVForm(string dataBase, string tableName);
        public abstract void ShowStatusForm();
        public abstract void ShowTableDiagnosticForm();
        public abstract void ShowNewDbForm();
        public abstract void AddNewRowToBoundedTable();
        public abstract void DeleteSelectedRowsTable();
        public abstract void BindDataGridViewToTable(DataGridView dg, string dataBase, string tableName, string limitT1, string limitT2, bool useLimits);
        public abstract void SaveTable(string tableName);
        public abstract void Reconnect();



        public abstract void RefreshNode(TreeNode node);
        public abstract ContextMenuStrip GetMainMenu();




    }


    

}
