/*
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using DBMS.core ;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace DBMS.core
{

    public partial class DataUserView : UserControl
    {
        public enum  ShowMode {All,OnlyQueryEditor, OnlyDataResult}

        public event DelegateSavedQueryHandler QuerySaved;
        public event DelegateTableNeedsRefreshed TableRefresh;


        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr  lParam);
        public const int CB_SHOWDROPDOWN = 0x14F;
        public const int pageSize = 1000;
        public string queryName;

        private CodeCompletionForm completionForm;
        private bool tableModifiedAndNotSaved = false;
        private int limitT1, limitT2;
        private bool querySaved;
        private string queryFileName;
        private TreeNode currentTableNode;
        private DataTable resultTable;
        private bool queryIsExecuting;


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
        public TreeNode CurrentTableNode
        {
            get { return currentTableNode; }
            set { currentTableNode = value; }
        }

        private IDBBrowser browser;
        public IDBBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }

        private string dataBase;
        public string DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        private Form parentForm;

        public DataGridView DGResultView
        {
            get { return this.dgResult; }
        }
        public DataGridView DGTableView
        {
            get { return this.dgTableView; }
        }

        StringBuilder restCommandBuffer;
        StringBuilder mainCommandBuffer;
 

        private String _message;
        public String Message
        {
            get { return _message; }
            set
            {
                _message = value;
                this.messageText.Text = _message;
            }
        }
       
        private string analized;
        private string delimiter = ";";
        private int chars;
        


        public DataUserView(Form parentForm, IDBBrowser browser)
        {
            this.parentForm = parentForm;
            InitializeComponent();
            comboBoxRows.SelectedIndex = 0;
            querySaved = true;
            this.browser = browser;
            tablePrimaryKeys = new List<string>();
            if (browser != null)
            {
                completionForm = new CodeCompletionForm(this.parentForm, this.queryText, browser.CodeCompletionProvider);
                completionForm.StartPosition = FormStartPosition.Manual;
                completionForm.ParentControl = this;
            }

        }

        public void GetObjects(string dataBase, string tableName)
        {
            //objectText.Clear();
        }

        public void SetViewTable(TreeNode node)
        {
            if (node == null)
                return;
            currentTableNode = node;
            tableName = node.Text;
            dataBase = node.Parent.Text;
            if (tableModifiedAndNotSaved)
            {
                if (MessageBox.Show("Data was modified, but was not saved. Save the data?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    tableStatus.ForeColor = Color.Blue;
                    tableStatus.Text = "`" + this.dataBase + "`.`" + TableName + "`" + " - Data saved.";
                }
            }
            tableModifiedAndNotSaved = false;
            browser.BindDataUserView(this, node, textBox1.Text, textBox2.Text, comboBoxRows.SelectedIndex == 1, DGTableView.SortedColumn);
        }

        public void PrintBoundedTable(string dataBase,string tableName)
        {
            this.tableStatus.Text = "`" + dataBase + "`.`" + tableName + "`";
        }


        private bool IsValidEnclosed(string s, char waitingChar, int i, bool lastCharWasSpecial)
        {
            int k = i - 1;
            bool gata = false;
            while ((k >= 0) && (gata == false))
            {
                if (s[k] != '\\')
                {
                    gata = true;
                }
                else
                    k--;
            }
            if (k > -1)
            {
                //este caracter de inchidere 
                if ((i - k) % 2 != 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if ((i % 2 == 0) && (!lastCharWasSpecial) || ((i % 2 != 0) && (lastCharWasSpecial)))
                {
                    return true;
                }
                else
                    return false;
            }

        }


        private List<string> ProcessClearCmd(int cursorPosition,string cmd)
        {
            int start = 0;
            int initialLeng = restCommandBuffer.Length;
            analized = restCommandBuffer.Append(cmd).ToString();
            restCommandBuffer.Append(cmd);
            List<string> ret = new List<string>();

            string l = analized.Substring(start, analized.Length - start);
            string[] srez = l.Split(new string[] { delimiter }, StringSplitOptions.None);
            if (srez.Length > 1)
            {
                mainCommandBuffer.Append(srez[0]+";");
                ret.Add(mainCommandBuffer.ToString());
                mainCommandBuffer.Length = 0;
                int j = 0;
                for (j = 1; j < srez.Length - 1; j++)
                    ret.Add(srez[j] +";");

                restCommandBuffer.Length = 0;
                restCommandBuffer.Append(srez[srez.Length - 1]);
            }
            else
            {
                restCommandBuffer.Length = 0;
                restCommandBuffer.Append(l);
            }
            return ret;
        }

        private string GetQuery(string s, int startindex, int cursorPosition, int len, ref bool waitingForEnclose, ref char waitingChar, ref string delimiter, ref bool waitingForDelimiter, ref bool lastCharWasSpecial)
        {
            string aux = "", cmd = "";
            int i = 0, istartp = 0, istopp = 0, crt = 0;
            List<string> queries = new List<string>();
            bool delNotFound = false;
            for (i = 0; i < len; i++)
            {
                if ((s[i] == '\'' || s[i] == '"'))
                {
                    if (!waitingForEnclose)
                        waitingChar = s[i];
                    bool isEnclosing = IsValidEnclosed(s, waitingChar, i, lastCharWasSpecial);
                    if (isEnclosing)
                    {
                        waitingChar = s[i];
                        if (!waitingForEnclose)
                        {
                            cmd =s.Substring(crt, i - crt);
                            List<string> r = ProcessClearCmd(cursorPosition, cmd);
                            for (int j = 0; j < r.Count; j++)
                                queries.Add(r[j]);
                            for (int j = 0; j < r.Count; j++)
                            {
                                if (chars <= cursorPosition && cursorPosition <= chars + r[j].Length)
                                {
                                    return r[j];
                                }
                                chars += r[j].Length;

                            }
                            istartp = i + 1;
                            mainCommandBuffer.Append(restCommandBuffer.ToString());
                            mainCommandBuffer.Append(s[i]);
                        }
                        else
                        {
                            restCommandBuffer.Length = 0;
                            istopp = i;
                            if (istopp > -1)
                            {
                                mainCommandBuffer.Append(s, istartp, istopp - istartp);
                            }
                            mainCommandBuffer.Append(s[i]);
                            crt = i + 1;
                        }
                        waitingForEnclose = !waitingForEnclose;
                    }
                }
            }
            if (waitingForEnclose)
                mainCommandBuffer.Append(s, istartp, len - istartp);
            else
            {
                string tmpa = s.Substring(crt, len - crt);
                List<string> r = ProcessClearCmd(cursorPosition, tmpa);
                if (r.Count == 0)
                    return s;
                for (int j = 0; j < r.Count; j++)
                    queries.Add(r[j]);
                for (int j = 0; j < r.Count; j++)
                {
                    if (chars <= cursorPosition && cursorPosition <= chars + r[j].Length)
                    {
                        return r[j];
                    }
                    chars += r[j].Length;

                }
            }
            if (s[len - 1] == '\\')
            {
                int kb = len - 2;
                bool gatab = false;
                while ((kb >= 0) && (gatab == false))
                {
                    if (s[kb] != '\\')
                    {
                        gatab = true;
                    }
                    else
                        kb--;
                }
                if ((len - 1 - kb) % 2 != 0)
                    lastCharWasSpecial = true;
                else
                    lastCharWasSpecial = false;
            }
            else
                lastCharWasSpecial = false;
            return "";
        }


        public void ShowFindAndReplace(bool replaceMode)
        {
           FindAndReplaceWindow.ShowWindow(this.queryText, replaceMode, "");
        }

        public void ExecuteActiveQuery()
        {
            int startIndex = 0, stopIndex = 0;
            int currentPos = queryText.ActiveTextAreaControl.Caret.Offset;
            string text = queryText.Text;
            int i = currentPos;
            chars = 0;     
            bool waitingForEnclose = false;
            char waitingChar = '\'';
            bool lastCharWasSpecial = false;
            string   delimiter = ";";
            bool waitingForDelimiter = false;
            restCommandBuffer = new StringBuilder();
            mainCommandBuffer = new StringBuilder();
            string q = GetQuery(queryText.Text, 0, currentPos, queryText.Text.Length, ref waitingForEnclose, ref waitingChar, ref delimiter, ref waitingForDelimiter, ref lastCharWasSpecial);
            ExecuteQuery(q);
        }

        public void ExecuteAllQueries()
        {
            ExecuteQuery(queryText.Text);
        }

        public void ExecuteQuery(string query)
        {
            if (query != null)
            {
                if (query.Trim() == "")
                    return;
            }
            else
                return;
            tabControl.SelectedIndex = 1;
            try
            {
                queryIsExecuting = true;
                dgBindingProgressBar.Visible = true;
                dgBindingProgressBar.MarqueeAnimationSpeed = 10;
                browser.ExecuteQuery(query,this);
            }
            catch (Exception ex)
            {
                messageText.AppendText(ex.Message);
                messageText.AppendText(Environment.NewLine);
            }
            tabControl.SelectedIndex = 3;

        }

        public void ExecuteSelectionQuery()
        {
            ExecuteQuery(queryText.ActiveTextAreaControl.SelectionManager.SelectedText);
        }

        public void ShowElements(ShowMode mode)
        {
            switch (mode)
            {
                case ShowMode.All:
                    splitContainer1.SplitterDistance = (int)splitContainer1.Height / 2;
                    break;
                case ShowMode.OnlyDataResult:
                    splitContainer1.SplitterDistance = 0;
                    break;
                case ShowMode.OnlyQueryEditor:
                    splitContainer1.SplitterDistance = splitContainer1.Height;
                    break;
            }
        }

        public void LoadQuery()
        {
            if (!querySaved)
            {
                DialogResult res = MessageBox.Show("Query not saved. Save now?", "Save current", MessageBoxButtons.YesNoCancel);
                switch (res)
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        SaveQuery();
                        LoadNewFile();
                        break;
                    case DialogResult.No:
                        LoadNewFile();
                        break;
                }
            }
            else
            {
                LoadNewFile();
            }
        }

        public void SaveQuery()
        {
            SaveFileDialog sDiag = null ;
            if (queryFileName == null)
            {
                sDiag = new SaveFileDialog();
                sDiag.Filter = "Text files|*.txt|SQL file|*.sql|All files|*.*";
                sDiag.ShowDialog();
                if (sDiag.FileName == "")
                    return;
                queryFileName = sDiag.FileName;
                queryName = Path.GetFileName(queryFileName);
            }
            FileStream fStream = File.Open(queryFileName, FileMode.Create);
            byte[] s = Encoding.ASCII.GetBytes(queryText.Text);
            fStream.Write(s, 0, s.Length);
            fStream.Close();
            querySaved = true;
            if (QuerySaved != null)
                QuerySaved(this, new QuerySavedEvent(queryFileName));
        }

        public void SaveAsNewQuery()
        {
            SaveFileDialog sDiag = new SaveFileDialog();
            sDiag.Filter = "Text files|*.txt|SQL file|*.sql|All files|*.*";
            sDiag.ShowDialog();
            if (sDiag.FileName == "")
                return;
            queryFileName = sDiag.FileName;
            queryName = Path.GetFileName(queryFileName);
            SaveQuery();
        }

        public void CopyToClipboardFromActiveControl()
        {
            if(queryText.ActiveControl!=null)
                if (queryText.ActiveControl.Focused)
                {
                    if (queryText.ActiveTextAreaControl.SelectionManager.SelectedText != null && queryText.ActiveTextAreaControl.SelectionManager.SelectedText != "")
                        Clipboard.SetText(queryText.ActiveTextAreaControl.SelectionManager.SelectedText);
                    return;
                }
            if (dgTableView.Focused)
            {
                if (dgTableView.CurrentCell != null)
                {
                    Clipboard.SetText(dgTableView.CurrentCell.FormattedValue.ToString());
                }
                return;
            }

            if (historyText.ActiveControl != null)
                if (historyText.ActiveControl.Focused)
                {
                    if (historyText.ActiveTextAreaControl.SelectionManager.SelectedText != null && historyText.ActiveTextAreaControl.SelectionManager.SelectedText != "")
                        Clipboard.SetText(historyText.ActiveTextAreaControl.SelectionManager.SelectedText);
                    return;
                }
        }

        public void PasteFromClipboardToActiveControl()
        { 
            if(queryText.ActiveControl!=null)
                if (queryText.ActiveControl.Focused)
                {
                    queryText.ActiveTextAreaControl.TextArea.InsertString(Clipboard.GetText());
                    return;
                }
            if (dgTableView.Focused)
            {
                if (dgTableView.CurrentCell != null)
                {
                    dgTableView.CurrentCell.Value = Clipboard.GetText() ;
                }
            }
        }


        public void Undo()
        {
            queryText.Document.UndoStack.Undo();
        }

        public void Redo()
        {
            queryText.Document.UndoStack.Redo();
        }

        public void AddToHistoryOutput(HistoryElement value)
        {
            if (value == null)
                return;
            if (value.SqlString == null)
                return;
            if (value.SqlString.Trim() == "")
                return;
            try
            {
                historyText.Document.ReadOnly = false;
                    if (!value.SqlString.EndsWith(";"))
                        value.SqlString = value.SqlString.Insert(value.SqlString.Length, ";");
                string s1 = value.TimeStamp.ToString("hh:mm:ss tt");
                string s2 = value.ExecTime.ToString() + "ms";
                string header = "/*[" + s1.PadLeft(7) + "] [" + s2.PadLeft(12) + "]*/" + "     ";
                string blank = "";
                blank = blank.PadRight(header.Length);
                string[] lines = value.SqlString.Split('\n');
                if (lines.Length > 0)
                {
                    lines[0] = lines[0].TrimEnd(new char[] { '\n', '\r' });
                    historyText.Document.Insert( historyText.Document.TextLength, header + lines[0] + Environment.NewLine);
                    for (int i = 1; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].TrimEnd(new char[] { '\n', '\r' });
                        historyText.Document.Insert(historyText.Document.TextLength, blank + lines[i].TrimStart(' ') + Environment.NewLine);
                    }
                    historyText.Document.CommitUpdate();
                    historyText.Refresh();
                }
                historyText.ActiveTextAreaControl.TextArea.ScrollToCaret();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                historyText.Document.ReadOnly = true;
            }
        }

        public void RemoveEvents(){
            
        }
       
        public void BindTableResult(QueryExecutedEventArgs e)
        {
             if (queryIsExecuting)
                 tabControl.SelectedIndex = 1;
             queryIsExecuting = false;
             if (e.ErrorMessage != null)
                 messageText.AppendText(e.ErrorMessage + Environment.NewLine);

             if (e.Data != null)
             {
                 dgResult.Rows.Clear();
                 resultTable = (DataTable)e.Data;
                 dgResult.VirtualMode = true;
                 dgResult.RowCount = (int)e.RowsReturned;
                 tableModifiedAndNotSaved = false;
             }
             else
             {
                 dgResult.RowCount = 0;
                 resultTable = new DataTable();
             }
             dgBindingProgressBar.Visible = false;
             dgBindingProgressBar.MarqueeAnimationSpeed = 0;
             tableStatus.ForeColor = Color.Blue;
        }
        
        private void LoadNewFile()
        {
            OpenFileDialog oF = new OpenFileDialog();
            oF.Filter = "Text files|*.txt|SQL file|*.sql|All files|*.*";
            oF.ShowDialog();
            if (oF.FileName == "")
                return;
            StreamReader fStream = new StreamReader(oF.FileName);
            this.queryText.Text = fStream.ReadToEnd();
            fStream.Close();
            queryFileName = oF.FileName;
            querySaved = true;
        }
       
        private void OutputQuerryMessage(string message)
        {
            this.messageText.Text = message;
            this.resultPage.Select();
        }
        
        private void saveStripButton_Click(object sender, EventArgs e)
        {
            if (dgTableView.ColumnCount < 1)
                return;
            try
            {
                DGTableView.EndEdit();
                browser.SaveTable(tableName,this);
                tableStatus.ForeColor = Color.Blue;
                tableStatus.Text = "Data saved.";
                tableModifiedAndNotSaved = false;
                //browser.BindDataGridViewToTable(dgTableView, browser.CurrentDataBase, browser.CurrentTable, textBox1.Text, textBox2.Text, comboBoxRows.SelectedIndex == 1);
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteRowsStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete selected rows?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            dgTableView.SuspendLayout();
            try
            {

                browser.DeleteSelectedRowsTable(this);
                List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
                int nr = cachedRows.FindAll(CheckedItems).Count;
                dgTableView.RowCount -= nr;
                cachedRows.RemoveAll(CheckedItems);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                dgTableView.ResumeLayout();
            }
        }

        private static bool CheckedItems(SqlDataGridViewRow row)
        {
            return row.Checked;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            queryText.Focus();
            if (currentTableNode == null)
                return;
            if (currentTableNode.Parent == null)
                return;
            if (TableRefresh != null)
                	TableRefresh(this, new TableRefreshArgs(currentTableNode, DGTableView.SortedColumn));
        }


        private void textBox1_Validated(object sender, EventArgs e)
        {
          int.TryParse(textBox1.Text, out limitT1);
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out limitT2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out limitT2);
            limitT1 -= limitT2;
            if (limitT1 < 0)
                limitT1 = 0;
            textBox1.Text = limitT1.ToString();
            SetViewTable(currentTableNode);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out limitT2);
            limitT1 += limitT2;
            textBox1.Text = limitT1.ToString();
            SetViewTable(currentTableNode);
        }

        private void DataUserView_Load(object sender, EventArgs e)
        {
            queryText.ActiveTextAreaControl.TextArea.DoProcessDialogKey+=new DialogKeyProcessor(TextArea_DoProcessDialogKey);
        	string dir = "highlighting\\"; // Insert the path to your xshd-files.
            FileSyntaxModeProvider fsmProvider; // Provider
            if (Directory.Exists(dir))
            {
                HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider("highlighting"));
                queryText.SetHighlighting("SQL");
                object x = HighlightingManager.Manager.HighlightingDefinitions["SQL"];
                historyText.SetHighlighting("SQL");
            }
            comboBoxColumns.SelectedIndex = 0;
            comboBoxRows.SelectedIndex = 1;
            limitT1 = 0;
            limitT2 = 50;
            textBox1.Text = limitT1.ToString();
            textBox2.Text = limitT2.ToString();
            queryText.ActiveTextAreaControl.TextArea.MouseDown +=new MouseEventHandler(TextArea_MouseDown);
            comboBoxColumns.SelectedIndex = 3;
            //LanguageSwitcher.Instance().SaveControlLanguage(this);
            //LanguageManager.LanguageSwitcher.Instance().SwitchLanguage(this.GetType().Namespace, this.GetType().Name, this);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRows.SelectedIndex == 0)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
        }

        private void textBox1_Validated_1(object sender, EventArgs e)
        {
            limitT1 = int.Parse(textBox1.Text);
            limitT2 = int.Parse(textBox2.Text);
        }

        #region toolsAction

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string[] columns = new string[dgResult.ColumnCount];
            for(int i=0;i<dgResult.ColumnCount;i++)
                columns[i] = dgResult.Columns[i].HeaderText;
            browser.ShowExportTableForm(dgResult, columns);
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            historyText.IsReadOnly = false;
            historyText.Text = "";
            historyText.IsReadOnly = true;
        }

        private void saveHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDiag = new SaveFileDialog();
            sDiag.Filter = "Text files|*.txt|SQL file|*.sql|All files|*.*";
            sDiag.ShowDialog();
            if (sDiag.FileName == "")
                return;
            FileStream fStream = File.Open(sDiag.FileName, FileMode.Create);
            fStream.Write(Encoding.Default.GetBytes(historyText.Text),0,historyText.Text.Length);
            fStream.Close();
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int currentPos = historyText.ActiveTextAreaControl.Caret.Offset;
            int i = currentPos;
            chars = 0;
            bool waitingForEnclose = false;
            char waitingChar = '\'';
            bool lastCharWasSpecial = false;
            string delimiter = ";";
            bool waitingForDelimiter = false;
            restCommandBuffer = new StringBuilder();
            mainCommandBuffer = new StringBuilder();
            string q = GetQuery(historyText.Text, 0, currentPos, historyText.Text.Length, ref waitingForEnclose, ref waitingChar, ref delimiter, ref waitingForDelimiter, ref lastCharWasSpecial);
            q=q.TrimStart('\r','\n');
            q = q.Remove(0, 37);
            ExecuteQuery(q);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            historyText.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(this,e);
        }

        private void executeSelectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            string query = queryText.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
            ExecuteQuery(query);

        }

        private void queryText_TextChanged(object sender, EventArgs e)
        {
            querySaved = false;
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            queryText.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(this, e);
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            queryText.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(this, e);
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            queryText.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(this, e);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            queryText.Text = "";
        }

        private void ExecuteAllQueryMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteQuery(queryText.Text);
        }

        private void ExecuteCurrentQuerryMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteActiveQuery();
        }

        private void executeSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteQuery(queryText.ActiveTextAreaControl.SelectionManager.SelectedText);
        }

    

        private void addRowStripButton_Click(object sender, EventArgs e)
        {
            dgTableView.SuspendLayout();
            browser.AddNewRowToBoundedTable(dgTableView);
            dgTableView.ResumeLayout();
        }


        private void toolStripComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
                case 1:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
                    break;
                case 2:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
                    break;
                case 3:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    break;
                case 4:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
                    break;
                case 5:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;
                case 6:
                    dgResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    break;
            }

        }

        private void exportStripButton_Click(object sender, EventArgs e)
        {
            if (dgTableView.ColumnCount < 1)
                return;
            string[] columns = new string[dgTableView.ColumnCount-1];
            for (int i = 1; i < dgTableView.ColumnCount; i++)
                columns[i - 1] = dgTableView.Columns[i].HeaderText;
            browser.ShowExportTableForm(dgTableView, columns);
        }

       #endregion




        private void dgResult_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (dgResult != null)
            {
                List<SqlDataGridViewRow> data =(List<SqlDataGridViewRow>) dgResult.Tag;
                e.Value = data[e.RowIndex].Data[e.ColumnIndex];
            }
        }

        private void dgTableView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
                return;
            tableModifiedAndNotSaved = true;
            tableStatus.ForeColor = Color.Red;
            tableStatus.Text = "`" + dataBase + "`.`" + TableName + "`" + " - Data not saved.";
        }

        private void tabControl2_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void dgResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            BlobViewForm bForm = new BlobViewForm(e.RowIndex, e.ColumnIndex, true);
            if (resultTable.Rows[e.RowIndex][e.ColumnIndex ] != DBNull.Value)
            {
                if(resultTable.Rows[e.RowIndex][e.ColumnIndex ].GetType()==typeof(byte[]))
                    bForm = new BlobViewForm((byte[])resultTable.Rows[e.RowIndex][e.ColumnIndex ], e.RowIndex, e.ColumnIndex, true);
                else
                    bForm = new BlobViewForm(Encoding.Default.GetBytes(resultTable.Rows[e.RowIndex][e.ColumnIndex].ToString()), e.RowIndex, e.ColumnIndex, true);
            }
            bForm.ShowInTaskbar = true;
            bForm.TopMost = true;
            bForm.ShowDialog();

        }


        private void dgResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgTableView = (DataGridView)sender;
                int x = e.RowIndex;
                int y = e.ColumnIndex;
                if (e.Value != null)
                {
                    if (e.Value.ToString() != "")
                    {
                        if (e.Value.GetType() == typeof(byte[]))
                        {
                            long fieldLength = ((byte[])resultTable.Rows[e.RowIndex][e.ColumnIndex]).Length;
                            e.Value =Utils.FormatBytes(fieldLength);
                            e.FormattingApplied = true;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
            }
        }

        private void comboBoxColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxColumns.SelectedIndex)
            {
                case 0:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    break;
                case 1:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
                case 2:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
                    break;
                case 3:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
                    break;
                case 4:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    break;
                case 5:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
                    break;
                case 6:
                    dgTableView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;
            }

        }

        private void dgTableView_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;
            if (dg.CurrentCell == null)
                return;
            if (e.Control && e.KeyCode ==Keys.C)
            {
                Clipboard.SetDataObject(dg.CurrentCell.Value);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                dg.CurrentCell.Value = Clipboard.GetDataObject().ToString();
            }
        }




        protected  bool TextArea_DoProcessDialogKey(Keys keyData)
        {
           return completionForm.ProcessKey(keyData); 
        }



        protected void TextArea_MouseDown(object sender, MouseEventArgs e)
        {
            if(completionForm !=null)
                if (completionForm.Visible)
                    completionForm.HideIt();
        }



        private void refreshResultTable_Click(object sender, EventArgs e)
        {

        }

        private void exportResultTable_Click(object sender, EventArgs e)
        {
            if (dgResult.Columns.Count <1)
                return;
            string[] columns = new string[dgResult.ColumnCount - 1];
            for (int i = 1; i < dgResult.ColumnCount; i++)
                columns[i - 1] = dgResult.Columns[i].HeaderText;
            browser.ShowExportTableForm(dgResult, columns);
        }

        private void dgTableView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgView = (DataGridView)sender;
            if (dgView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            for (int i = 0; i < dgView.Columns.Count; i++)
            {
                if (i != e.ColumnIndex)
                    dgView.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            if(dgView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending)
                dgView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            else
                dgView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
            //TableRefresh(this, new TableRefreshArgs(currentTableNode, DGTableView.Columns[e.ColumnIndex]));
            browser.BindDataUserView(this,currentTableNode, textBox1.Text, textBox2.Text, comboBoxRows.SelectedIndex == 1, dgView.Columns[e.ColumnIndex]);
        }

        private void dgTableView_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {

        }





    }
}
