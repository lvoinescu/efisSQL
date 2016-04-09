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
using System.Windows.Forms;
using DBMS.core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System.Collections.Generic;
using System.Drawing;
namespace DBMS.core
{
    public interface IDBBrowser
    {
        ContextMenuStrip TableMenu { get;}
        ContextMenuStrip DataBaseMenu{ get;}
        ContextMenuStrip ToolsMenu { get;}
        object Connection { get; set; }
        Form ParentForm { get;set; }
        ImageList ImageList { get;}
        bool BrowserIsBusy { get;}
        Color TreeViewColor { get;}
        CodeCompletionProvider CodeCompletionProvider { get;}

        string CurrentDatabase { get;}
        string GetConnectionSource();
        string GetDDLInformation(string dataBase, string tableName);

        string GetNewFunctionQuery(string dataBase, string tableName, string name);
        string GetNewStoredProcedureQuery(string dataBase, string tableName, string name);
        string GetNewTriggerQuery(string dataBase, string tableName, string name);
        string GetNewViewQuery(string dataBase, string tableName, string name);
        string ServerAddress { get; set; }
        System.Windows.Forms.Form MasterForm { get; set; }
        System.ComponentModel.IComponent MenuManager { get; }
        System.Windows.Forms.ContextMenuStrip GetMainMenu();

        void DeleteSelectedRowsTable(DataUserView dataUserView);
        void Disconnect();
        //void Dispose();
        void RegisterTreeView(TreeView tView);
        void Reconnect();
        void RefreshNode(System.Windows.Forms.TreeNode node);
        void RenameTable(string dataBase, string tableName, string newName);
        void SaveTable(string tableName, DataUserView dataUserView);
        void AddNewRowToBoundedTable(DataGridView dgTableView);
        void BindDataUserView(DataUserView dataUserView, TreeNode tableNode, string limitT1, string limitT2, bool useLimits, DataGridViewColumn sortColumn);
        void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        void SetObjectOutput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        void SetQueryInput(ICSharpCode.TextEditor.TextEditorControl syntaxTextBox);
        void ShowAlterTableForm(string dataBase, string tableName);
        void ShowCreateTableForm(TreeNode node);
        void ShowExecuteForm(string fileName, System.Windows.Forms.Form mdiParent);
        void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable);
        void ShowExportTableForm(System.Windows.Forms.DataGridView dgView, string [] columns);
        void ShowExportTableForm(string dataBase, string tableName);
        void ShowFlushForm();
        void ShowImportFromCSVForm(string dataBase, string tableName);
        void ShowIndexesTableForm(string dataBase, string tableName);
        void ShowNewDbForm(TreeNode node);
        void ShowReferenceTableForm(string dataBase, string tableName);
        void ShowReorderColumnForm(string dataBase, string tableName);
        void ShowStatusForm();
        void ShowTableDiagnosticForm();
        void ShowTableStatus(string dataBase, string table);
        void TreeNodeBeforeExpand(object sender, TreeViewCancelEventArgs e);
        void TreeNodeAfterExpand(object sender, TreeViewEventArgs e);
        void TreeNodeBeforeSelect(object sender, TreeViewCancelEventArgs e);
        void TreeNodeAfterSelect(object sender, TreeViewEventArgs e);
        void TreeNodeMouseDown(object sender, MouseEventArgs e);
        void ExecuteQuery(string query, DataUserView output);
        bool TreeNodeIsTableNode(TreeNode node);
        
        event TableBoundedHandler TableBounded;
        event NewDataUserViewRequestedHandler NewDataUserViewRequested;
        event DisconnectRequestedEventHandler DisconnectRequestedEvent;
        event QueryExecutedHandler QueryExecuted;

        QueryResult GetColumnInformation(string dataBase, string tableName);
        QueryResult SwitchDataBase(string dataBase);      
        QueryResult GetStoredProcedureDefinition(string dataBase, string name);
        QueryResult GetStoredProcedures(string dataBase);
        QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit);
        QueryResult GetTable(string dataBase, string tableName);
        QueryResult GetTableStatus(string dataBase);
        QueryResult GetTriggerDefinition(string dataBase, string name);
        QueryResult GetTriggers(string dataBase);
        QueryResult GetViewDefinition(string dataBase, string name);
        QueryResult GetViews(string dataBase);
        QueryResult ListColumns(string dataBase, string tableName);
        QueryResult ListDatabases();
        QueryResult ListTables(string dataBase);
        QueryResult TruncateDataBase(string dataBase);
        QueryResult TruncateTable(string dataBase, string tableName);
        QueryResult DropDataBase(string dataBase);
        QueryResult DropFunction(string database, string name);
        QueryResult DropProcedure(string database, string name);
        QueryResult DropTable(string dataBase, string tableName);
        QueryResult DropTrigger(string database, string name);
        QueryResult DropView(string database, string name);
        QueryResult EmptyTable(string dataBase, string tableName);
        QueryResult GetFunctionDefinition(string dataBase, string name);
        QueryResult GetFunctions(string dataBase);
        QueryResult GetIndexInformation(string dataBase, string tableName);
        TreeNode CreateMainTreeNode();


        


    }
}
