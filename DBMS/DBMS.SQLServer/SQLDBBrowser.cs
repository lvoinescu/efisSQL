using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Data;
using SqlControls.DBEngine;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace SqlControls.DBEngine.SQL
{
    public class SQLDBBrowser :DBBrowser
    {


        public SQLDBBrowser(OdbcConnection connection)
            : base(connection)
        {
        }

        private OdbcConnection odbcConnection;
        private IComponent  menuManager;
        #region IDBBrowser Members

        public History History
        {
            set
            {
                this.history = value;
            }
            get
            {
                return this.history;
            }
        }


        public override event QueryExecutedHandler QueryExecuted;
        public override event TableBoundedHandler TableBounded;
        public override event TreeNodeRefreshedHandler TreeNodeRefreshed;

        public override IComponent MenuManager
        {
            get { return menuManager; }
        }
        
        public override Object Connection
        {
            get
            {
                return this.Connection;
            }
            set
            {
                this.Connection = value;
                this.odbcConnection = (OdbcConnection)Connection;
            }
        }
       
        public void CancelCurrentOpAsync()
        {
        }

        public override QueryResult ListDatabases()
        {
            try
            {
                string[] rez = null;
                DataTable dt = new DataTable();
                using (OdbcDataAdapter da = new OdbcDataAdapter("EXEC sp_databases;", odbcConnection))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count < 1)
                        return null;
                    rez = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                        rez[i] = dt.Rows[i][0].ToString();
                    return new QueryResult(rez);
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult ListTables(string database)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand  cmd = new OdbcCommand ("USE \""+database+"\"",odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText="SELECT * FROM sys.tables";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode,ex.Message);
            }
        }

        public override void ShowTableStatus(string database, string tablename)
        {
        }

        public override string GetConnectionSource()
        {
            return this.odbcConnection.DataSource;
        }

        public override QueryResult GetViews(string database)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + database + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"SELECT * from sysobjects where xtype='V'";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetFunctions(string database)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + database + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"SELECT name AS function_name
                                            ,SCHEMA_NAME(schema_id) AS schema_name
                                            ,type_desc
                                            FROM sys.objects
                                            WHERE type_desc LIKE '%FUNCTION%';";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetStoredProcedures(string database)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + database + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"SELECT routine_name AS object_name,
                                            routine_schema AS schema_name,
                                            routine_type AS object_type
                                            FROM INFORMATION_SCHEMA.ROUTINES
                                            WHERE routine_type = 'PROCEDURE'
                                            ORDER BY object_type, schema_name, object_name";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetTriggers(string database)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + database + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"select * from sys.triggers";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }

        }

        public override QueryResult GetViewDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  table_name FROM `INFORMATION_SCHEMA`.`VIES` WHERE `table_name` = '" + name + "' AND TABLE_SCHEMA=`" + dataBase + "`;";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetStoredProcedureDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE `specific_name` = '" + name + "' AND ROUTINE_SCHEMA=`" + dataBase + "`;";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetFunctionDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  routine_definition FROM `INFORMATION_SCHEMA`.`ROUTINES` WHERE `specific_name` = '" + name + "' AND ROUTINE_SCHEMA=`" + dataBase + "`;";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetTriggerDefinition(string dataBase, string name)
        {
            try
            {
                string s = "SELECT  ACTION_STATEMENT FROM `INFORMATION_SCHEMA`.`TRIGGERS` WHERE `specific_name` = '" + name + "' AND TRIGGER_SCHEMA=`" + dataBase + "`;";
                string rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        if (dt.Rows.Count < 1)
                            return new QueryResult(null);
                        rez = dt.Rows[0][0].ToString();
                        return new QueryResult(rez);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }

        }

        public override void ExecuteQuery(string query, DataGridView outPut)
        {
        }

        public override QueryResult DropProcedure(string database, string name)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop procedure`" + database + "`.`" + name + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult DropView(string database, string name)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop view `" + database + "`.`" + name + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult DropFunction(string database, string name)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop function `" + database + "`.`" + name + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult DropTrigger(string database, string name)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop trigger `" + database + "`.`" + name + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override string GetNewTriggerQuery(string dataBase, string tableName, string name)
        {
            string s = @"DELIMITER $$
            CREATE
                /*[DEFINER = { user | CURRENT_USER }]*/
                TRIGGER `" + dataBase + "`.`" + tableName + @"` BEFORE/AFTER INSERT/UPDATE/DELETE
                ON `" + dataBase + "`.`<" + tableName + @">`
                FOR EACH ROW
                    BEGIN

                    END$$

            DELIMITER ;";
            return s;
        }

        public override string GetNewFunctionQuery(string dataBase, string tableName, string name)
        {
            string s = @"DELIMITER $$
    CREATE
    /*[DEFINER = { user | CURRENT_USER }]*/
    FUNCTION `" + dataBase + "`.`" + name + @"`()
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

        public override string GetNewStoredProcedureQuery(string dataBase, string tableName, string name)
        {

            string s = @"DELIMITER $$

CREATE
    /*[DEFINER = { user | CURRENT_USER }]*/
    PROCEDURE `" + dataBase + "`.`" + name + @"`()
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

        public override string GetNewViewQuery(string dataBase, string tableName, string name)
        {
            string s = @"CREATE
    /*[ALGORITHM = {UNDEFINED | MERGE | TEMPTABLE}]
    [DEFINER = { user | CURRENT_USER }]
    [SQL SECURITY { DEFINER | INVOKER }]*/
    VIEW `" + dataBase + "`.`" + name + @"` 
    AS
(SELECT * FROM ...);";
            return s;
        }

        public override QueryResult ListColumns(string dataBase, string tableName)
        {
            try
            {
                string[] rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + dataBase + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"SELECT column_name 
                                            FROM INFORMATION_SCHEMA.COLUMNS 
                                            WHERE TABLE_NAME = '"+tableName+@"'
                                            ORDER BY ORDINAL_POSITION";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count < 1)
                                return new QueryResult(null);
                            rez = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                                rez[i] = dt.Rows[i][0].ToString();
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult SwitchDataBase(string dataBase)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("use " + dataBase + ";", this.odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                    return new QueryResult(null);
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public  OdbcConnection GetConnection()
        {
            return this.odbcConnection;
        }

        public  History GetHistory()
        {
           // return this.History;
            return null;
        }

        public override void SetHistoryOutput(ICSharpCode.TextEditor.TextEditorControl shtb)
        {
//            //this.History.SetOutputText(shtb);
//            this.historyText = shtb;


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

        public override void SetObjectOutput(ICSharpCode.TextEditor.TextEditorControl shtb)
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

        public override void SetQueryInput(ICSharpCode.TextEditor.TextEditorControl shtb)
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

        public override QueryResult GetTable(string dataBase, string tableName)
        {

            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "`;";
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        return new QueryResult(dt);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetTable(string dataBase, string tableName, string lowLimit, string hiLimit)
        {
            try
            {
                using (DataSet tableSet = new DataSet())
                {
                    string s = "select * from `" + dataBase + "`.`" + tableName + "` limit " + lowLimit + "," + hiLimit + ";";
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(tableSet, tableName);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        return new QueryResult(tableSet.Tables[tableName]);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetTableStatus(string dataBase)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    string s = "show table status from " + dataBase + ";";
                    using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                    {
                        DateTime start = DateTime.Now;
                        da.Fill(dt);
                        TimeSpan time = DateTime.Now.Subtract(start);
                        //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                        return new QueryResult(dt);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override QueryResult GetColumnInformation(string dataBase, string tableName)
        {
            try
            {
                DataTable rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + dataBase + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"SELECT column_name as Field, DATA_TYPE as Type,IS_NULLABLE as 'NULL'
                                            FROM INFORMATION_SCHEMA.COLUMNS 
                                            WHERE TABLE_NAME = '" + tableName + @"'
                                            ORDER BY ORDINAL_POSITION";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            rez = dt;
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }

        }

        public override QueryResult GetIndexInformation(string dataBase, string tableName)
        {
            try
            {
                DataTable rez = null;
                using (DataTable dt = new DataTable())
                {
                    using (OdbcCommand cmd = new OdbcCommand("USE \"" + dataBase + "\"", odbcConnection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"select name as key_name from sysindexes where id=object_id('"+tableName+"')";
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            rez = dt;
                        }
                        return new QueryResult(rez);
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
        }

        public override string GetDDLInformation(string dataBase, string tableName)
        {
            using (DataTable dt = new DataTable())
            {
                string rez = null;
                string s = "show create table `" + dataBase + "`.`" + tableName + "`;";
                using (OdbcDataAdapter da = new OdbcDataAdapter(s, odbcConnection))
                {
                    DateTime start = DateTime.Now;
                    da.Fill(dt);
                    TimeSpan time = DateTime.Now.Subtract(start);
                    //History.Add(new HistoryElement(DateTime.Now, s, time.Milliseconds));
                    if (dt.Rows.Count > 0)
                    {
                        rez = Encoding.ASCII.GetString((byte[])dt.Rows[0].ItemArray[1]);
                        return rez.ToLower() + ";";
                    }
                }
            }
            return null;
        }

        public override QueryResult DropTable(string database, string tableName)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop table `" + database + "`.`" + tableName + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult EmptyTable(string dataBase, string tableName)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("truncate table `" + dataBase + "`.`" + tableName + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult DropDataBase(string database)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("drop database `" + database + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult TruncateTable(string database, string tableName)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("truncate table `" + database + "`.`" + tableName + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override QueryResult TruncateDataBase(string database)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    using (OdbcDataAdapter da = new OdbcDataAdapter("show tables from `" + database + "`;", odbcConnection))
                    {
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                            TruncateTable(database, dt.Rows[i][0].ToString());
                    }
                }
            }
            catch (OdbcException ex)
            {
                return new QueryResult(ex.ErrorCode, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new QueryResult(ex.Message);
            }
            return null;
        }

        public override void Disconnect()
        {
            this.odbcConnection.Close();
            this.odbcConnection.Dispose();
        }

        public override void ShowAlterTableForm(string dataBase, string tableName)
        {
            //using (AlterTableForm aTForm = new AlterTableForm(this.odbcConnection, dataBase, tableName, AlterTableForm.Mod.Alter))
            //{
            //    aTForm.ShowDialog();
            //}
        }

        public override void ShowCreateTableForm(string dataBase)
        {
            //using (AlterTableForm aTForm = new AlterTableForm(this.odbcConnection, dataBase, tableName, AlterTableForm.Mod.Create))
            //{
            //    aTForm.ShowDialog();
            //}
        }

        public override void ShowIndexesTableForm(string dataBase, string tableName)
        {
            //using (IndexForm aTForm = new IndexForm(this.odbcConnection, dataBase, tableName))
            //{
            //    aTForm.ShowDialog();
            //}
        }

        public override void RenameTable(string dataBase, string tableName, string newName)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand("rename table `" + dataBase + "`.`" + tableName + "` to `" + dataBase + "`.`" + newName + "`;", odbcConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void ShowExportForm(string[] dataBases, string[] tables, string selectedDataBase, string selectedTable)
        {
            //BackUpDataForm bf = new BackUpDataForm(this.odbcConnection, dataBases, tables, selectedDataBase, selectedTable);
            //bf.Browser = this;
            //bf.ShowDialog();
        }

        public override void ShowExportTableForm(string dataBase, string tableName)
        {
            using (QueryResult qR = this.ListColumns(dataBase, tableName))
            {
                //ExportData bf = new ExportData(this.odbcConnection, dataBase, tableName, (string[])qR.Result);
                //bf.ShowDialog();
            }
        }

        public override void ShowExportTableForm(DataGridView dgView)
        {
            
        }

        public override void ShowReorderColumnForm(string dataBase, string tableName)
        {

        }

        public override void ShowExecuteForm(string fileName, Form mdiParent)
        {
            //ExecuteBatchForm eForm = new ExecuteBatchForm(fileName, this.odbcConnection);
            //eForm.ShowDialog();
        }

        public override void ShowReferenceTableForm(string dataBase, string tableName)
        {
        }

        public override void BindDataGridViewToTable(DataGridView dg, string dataBase, string tableName, string limitT1, string limitT2, bool useLimits)
        {
        }

        public override void SaveTable(string tableName)
        {
        }

        public override void AddNewRowToBoundedTable()
        {
            
        }

        public override void DeleteSelectedRowsTable()
        {
        }

        public override void ShowFlushForm()
        {
           
        }

        public override void ShowStatusForm()
        {
        }
        public override void ShowTableDiagnosticForm()
        {
        }

        public override void ShowImportFromCSVForm(string dataBase, string tableName)
        {
        }
        public override void ShowNewDbForm()
        {

        }

        public override void Reconnect()
        {
        }

        public override void RefreshNode(TreeNode node)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override ContextMenuStrip GetMainMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            return menu;
        }
        #endregion
    }
}
