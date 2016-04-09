/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 5/13/2012
 * Time: 10:24 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Drawing;

namespace DBMS.core
{
	/// <summary>
	/// Description of DBBrowserControl.
	/// </summary>
	public class DBBrowserControl:UserControl
	{

        protected int oldImageIndex;
        protected long noOfRowsReturnedFromSelectQuery = 0;
        protected BackgroundWorker worker;
        protected BackgroundWorker workerBinder;
        protected BackgroundWorker workerQueryExecutor;
        protected AsyncOperation asyncOperation;
        protected System.Windows.Forms.Timer updateNodeTimer;
        private IContainer components;
        protected SendOrPostCallback postCallBack;
        protected ManualResetEvent ready;
        private ContextMenuStrip cellMenu;
        private ToolStripMenuItem setTuNULLToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem viewAsTextToolStripMenuItem;
        protected bool tableIsBinding = true;
        private int cellRowMenu = 0, cellColumnMenu = 0;

        public event QueryExecutedHandler QueryExecuted;
		public DBBrowserControl()
		{
            InitializeComponent();
		}


        protected bool browserIsBusy;
        public bool BrowserIsBusy
        {
            get { return browserIsBusy; }
            set { browserIsBusy = value; }
        }
        protected TreeView browseTree;
        public void RegisterTreeView(TreeView tView)
        {
            this.browseTree = tView;
        }
        
        protected History history;
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
       
        private Form masterForm;
        public Form MasterForm
        {
            get { return masterForm; }
            set { masterForm = value; }
        }
       
        protected TreeNode currentNode;
        
        protected Form parentForm;
        public new Form ParentForm
        {
            get
            {
                return this.parentForm;
            }
            set
            {
                this.parentForm = value;
            }
        }
        
        protected string serverAddress;
        public string ServerAddress
        {
            get { return serverAddress; }
            set { serverAddress = value; }
        }
       
        protected string currentDataBase;
        public string CurrentDatabase
        {
            get { return currentDataBase; }
        }
       
        protected string currentTable;
        public string CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; }
        }

        protected void OnQueryExecuted(object e)
        {
            if (QueryExecuted != null)
                QueryExecuted(this, (QueryExecutedEventArgs)e);
        }
        protected virtual void OnDataBaseChanged(string currentDataBase)
        {
        }



        public virtual void RefreshNode(TreeNode node)
        {
            if (browserIsBusy)
            {
                MessageBox.Show("Current browser is busy for the moment", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            updateNodeTimer.Enabled = true;
            updateNodeTimer.Start();
            oldImageIndex = node.ImageIndex;
            node.ImageIndex = 12;
            browserIsBusy = true;
            worker.RunWorkerAsync(node);
        }

        public virtual bool TreeNodeIsTableNode(TreeNode node)
        {
            object[] tag = (object[])node.Tag;
            SQLNodeType type = (SQLNodeType)tag[1];
            return type == SQLNodeType.Table;
        }




        #region treeView

        public virtual void TreeNodeAfterSelect(object sender, TreeViewEventArgs e)
        {
            currentNode = e.Node;
            object[] arg = (object[])e.Node.Tag;
            SQLNodeType nodType = (SQLNodeType)arg[1];
            SQLNodeStatus nodStatus = (SQLNodeStatus)arg[2];
            if (browserIsBusy)
            {
                return;
            }
        }

        public virtual void TreeNodeAfterExpand(object sender, TreeViewEventArgs e)
        {
        }

        public virtual void TreeNodeBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            object[] arg = (object[])e.Node.Tag;
            SQLNodeType nodType = (SQLNodeType)arg[1];
            SQLNodeStatus nodStatus = (SQLNodeStatus)arg[2];
            if (browserIsBusy && nodStatus == SQLNodeStatus.Unset)
            {
                e.Cancel = true;
                return;
            }
            if (nodStatus == SQLNodeStatus.Unset)
                RefreshNode(e.Node);
        }

        public virtual void TreeNodeBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (browserIsBusy || e.Node.Tag == null)
                e.Cancel = true;
        }

        public virtual void TreeNodeMouseDown(object sender, MouseEventArgs e)
        {
            if (browserIsBusy)
                return;
            currentNode = browseTree.GetNodeAt(e.X, e.Y);
            object[] arg = (object[])currentNode.Tag;
            SQLNodeType nodType = (SQLNodeType)arg[1];
            SQLNodeStatus nodStatus = (SQLNodeStatus)arg[2];
            if (currentNode == null)
                return;
            if (currentNode.Level == 2 && nodType == SQLNodeType.Table)
            {
                if (currentNode.Parent.Text != currentDataBase)
                {
                    currentDataBase = currentNode.Parent.Text;
                    OnDataBaseChanged(currentDataBase);
                }
                currentTable = currentNode.Text;
            }
            else
            {
                if (currentNode.Level == 1 && nodType == SQLNodeType.Database)
                {
                    if (currentDataBase != currentNode.Text)
                    {
                        currentDataBase = currentNode.Text;
                        OnDataBaseChanged(currentDataBase);
                    }
                }
            }
        }
        #endregion


        #region queryExecution && RESULT table

        protected virtual void workerQueryExecutor_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        protected virtual void workerQueryExecutor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            object[] rez = (object[])e.Result;
            DataTable dt = (DataTable)rez[0];
            DataUserView output = (DataUserView)rez[1];
            DateTime start = (DateTime)rez[2];
            string query = (string)rez[3];
            string errMess = (string)rez[5];
            if (dt.Columns.Count > 0)
            {
                output.DGResultView.Columns.Clear();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].DataType == typeof(byte[]))
                    {
                        DataGridViewButtonColumn c = new DataGridViewButtonColumn();
                        c.DataPropertyName = dt.Columns[i].ColumnName;
                        c.HeaderText = c.DataPropertyName;
                        c.Name = c.HeaderText;
                        output.DGResultView.Columns.Add(c);
                    }
                    else
                    {
                        DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                        c.DataPropertyName = dt.Columns[i].ColumnName;
                        c.HeaderText = c.DataPropertyName;
                        c.Name = c.HeaderText;
                        output.DGResultView.Columns.Add(c);
                    }
                }
                List<SqlDataGridViewRow> res = new List<SqlDataGridViewRow>();
                for (int i = 0; i < dt.Rows.Count; i++)
                    res.Add(new SqlDataGridViewRow(dt.Rows[i].ItemArray));
                output.DGResultView.Tag = res;
                output.DGResultView.ColumnCount = dt.Columns.Count;
            }
            TimeSpan time = DateTime.Now.Subtract(start);
            history.Add(new HistoryElement(start, query, time.TotalMilliseconds));
            QueryExecutedEventArgs argEx = new QueryExecutedEventArgs(new HistoryElement(start, query, time.TotalMilliseconds));
            argEx.RowsReturned = (long)rez[4];
            argEx.ErrorMessage = errMess;
            argEx.Data = dt;
            argEx.DataUserView = output;
            if (QueryExecuted != null)
                QueryExecuted(this, argEx);
        }

        protected virtual void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
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

        protected virtual List<TreeNode> RefreshServer(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshDataBase(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshTableNode(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshFunctions(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshProcedures(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshViews(TreeNode node)
        {
            return null;
        }
        protected virtual List<TreeNode> RefreshTriggers(TreeNode node)
        {
            return null;
        }
        protected virtual void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            currentNode.ImageIndex = oldImageIndex;
            currentNode.SelectedImageIndex = currentNode.ImageIndex;
        }


        #endregion

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.updateNodeTimer = new System.Windows.Forms.Timer(this.components);
            this.cellMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setTuNULLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateNodeTimer
            // 
            this.updateNodeTimer.Tick += new System.EventHandler(this.updateNodeTimer_Tick);
            // 
            // cellMenu
            // 
            this.cellMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setTuNULLToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.viewAsTextToolStripMenuItem});
            this.cellMenu.Name = "contextMenuStrip1";
            this.cellMenu.Size = new System.Drawing.Size(153, 92);
            // 
            // setTuNULLToolStripMenuItem
            // 
            this.setTuNULLToolStripMenuItem.Name = "setTuNULLToolStripMenuItem";
            this.setTuNULLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.setTuNULLToolStripMenuItem.Text = "Set to NULL";
            this.setTuNULLToolStripMenuItem.Click += new System.EventHandler(this.setToNULLToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy data";
            this.copyToolStripMenuItem.Visible = false;
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // viewAsTextToolStripMenuItem
            // 
            this.viewAsTextToolStripMenuItem.Name = "viewAsTextToolStripMenuItem";
            this.viewAsTextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewAsTextToolStripMenuItem.Text = "View as text";
            this.viewAsTextToolStripMenuItem.Click += new System.EventHandler(this.viewAsTextToolStripMenuItem_Click);
            // 
            // DBBrowserControl
            // 
            this.Name = "DBBrowserControl";
            this.cellMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void updateNodeTimer_Tick(object sender, EventArgs e)
        {
            currentNode.ImageIndex++;
            if (currentNode.ImageIndex > 15)
            {
                currentNode.ImageIndex = 12;
            }
            currentNode.SelectedImageIndex = currentNode.ImageIndex;
        }



        protected virtual void dgTableView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView dgTableView = (DataGridView)sender;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (!tableIsBinding)
                if (e.ColumnIndex > 0)
                    QueueForUpdateRow(dgTableView, e.RowIndex, e.ColumnIndex, cachedRows[e.RowIndex].Data[e.ColumnIndex - 1]);
        }

        protected virtual void QueueForUpdateRow(DataGridView dgTableView, int row, int column, object oldValue)
        {
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (column > 0)
            {
                if (cachedRows[row].Tag == null)
                {

                    UpdateInfo ui = new UpdateInfo(UpdateType.Update);
                    ui.elements.Add(new UpdatedCell(null, oldValue, dgTableView.Columns[column].Name, column));
                    cachedRows[row].Tag = ui;
                }
                else
                {
                    bool found = false;
                    UpdateInfo ui = (UpdateInfo)cachedRows[row].Tag;
                    if (ui.tipUpdate != UpdateType.Insert)
                    {
                        for (int j = 0; j < ui.elements.Count && !found; j++)
                        {
                            if (ui.elements[j].columnName.ToLower() == dgTableView.Columns[column].Name)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                            ui.elements.Add(new UpdatedCell(null, oldValue, dgTableView.Columns[column].Name, column));

                    }
                    cachedRows[row].Tag = ui;
                }
            }

        }

        protected virtual void dgTableView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                        case "tinyblob":
                        case "mediumblob":
                        case "longblob":
                            long blobLen = ((byte[])e.Value).Length;
                            e.Value = Utils.FormatBytes(blobLen);
                            e.FormattingApplied = true;
                            break;
                        case "text":
                        case "mediumtext":
                        case "longtext":
                        case "tinytext":
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

        protected virtual void dgTableView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected virtual void SetTypeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetTypeForm tForm = (SetTypeForm)sender;
            DataGridView dgTableView = (DataGridView)tForm.Tag;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (!tForm.CancelForm)
            {
                dgTableView.EndEdit();
                QueueForUpdateRow(dgTableView, tForm.RowIndex, tForm.ColumnIndex, dgTableView.Rows[tForm.RowIndex].Cells[tForm.ColumnIndex].Value);
                if (tForm.Set != null)
                {
                    if (tForm.Set.Length > 0)
                    {
                        string rez = tForm.Set[0];
                        for (int i = 1; i < tForm.Set.Length; i++)
                            rez += "," + tForm.Set[i];
                        cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = rez;
                    }
                    else
                        cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = null;
                }
                else
                    cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = null;
            }
            else
                dgTableView.CancelEdit();
        }

        protected virtual void txtForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TextForm tForm = (TextForm)sender;
            DataGridView dgTableView = (DataGridView)tForm.Tag;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (!tForm.CancelForm)
            {
                dgTableView.EndEdit();
                QueueForUpdateRow(dgTableView, tForm.RowIndex, tForm.ColumnIndex, dgTableView.Rows[tForm.RowIndex].Cells[tForm.ColumnIndex].Value);
                cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = tForm.Content;
            }
            else
                dgTableView.CancelEdit();
        }

        protected virtual void DateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TypeCalendarForm tForm = (TypeCalendarForm)sender;
            DataGridView dgTableView = (DataGridView)tForm.Tag;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (!tForm.CancelForm)
            {
                dgTableView.EndEdit();
                QueueForUpdateRow(dgTableView, tForm.RowIndex, tForm.ColumnIndex, dgTableView.Rows[tForm.RowIndex].Cells[tForm.ColumnIndex].Value);
                if (tForm.Value != null)
                {
                    if (tForm.Value.Length > 0)
                    {
                        cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = tForm.Value;
                    }
                    else
                        cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = null;
                }
                else
                    cachedRows[tForm.RowIndex].Data[tForm.ColumnIndex - 1] = null;
            }
            else
                dgTableView.CancelEdit();
        }

        protected virtual void blobForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            BlobViewForm bForm = (BlobViewForm)sender;
            DataGridView dgTableView = (DataGridView)bForm.Tag;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            if (!bForm.CancelForm)
            {
                dgTableView.EndEdit();
                QueueForUpdateRow(dgTableView, bForm.RowIndex, bForm.ColumnIndex, dgTableView.Rows[bForm.RowIndex].Cells[bForm.ColumnIndex].Value);
                cachedRows[bForm.RowIndex].Data[bForm.ColumnIndex - 1] = bForm.Data;
            }
            else
                dgTableView.CancelEdit();
        }

        protected virtual void dgTableView_CellClick(object sender, DataGridViewCellEventArgs e)
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
                        case "tinytext":
                        case "mediumtext":
                        case "longtext":

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
                        case "tinyblob":
                        case "mediumblob":
                        case "longblob":
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
                        case "set":
                            DataGridViewCell abc = dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                            Rectangle rect = dgTableView.GetCellDisplayRectangle(abc.ColumnIndex, abc.RowIndex, true);
                            string[] set = (string[])tg[1];
                            string crt = null;
                            if (abc.Value == DBNull.Value)
                                crt = null;
                            else
                                crt = (string)abc.Value;
                            SetTypeForm tForm = new SetTypeForm(e.RowIndex, e.ColumnIndex, set, crt, rect.Width);
                            tForm.Tag = dgTableView;
                            Point p = dgTableView.PointToScreen(rect.Location);
                            tForm.Top = p.Y; ;
                            tForm.Left = p.X;
                            tForm.Show();
                            tForm.FormClosed += new FormClosedEventHandler(SetTypeForm_FormClosed);
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

        protected virtual void dgTableView_CellValuePushed(object sender, System.Windows.Forms.DataGridViewCellValueEventArgs e)
        {
            DataGridView dView = (DataGridView)sender;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dView.Tag;
            if (e.ColumnIndex > 0)
                cachedRows[e.RowIndex].Data[e.ColumnIndex - 1] = e.Value;
            else
                cachedRows[e.RowIndex].Checked = (bool)e.Value;
        }

        protected virtual void dgTableResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgTableView = (DataGridView)sender;
            if (dgTableView.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn))
            {
                BlobViewForm bForm = new BlobViewForm(e.RowIndex, e.ColumnIndex, true);
                if (dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
                    bForm = new BlobViewForm((byte[])dgTableView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, e.RowIndex, e.ColumnIndex, false);
                bForm.Tag = dgTableView;
                dgTableView.BeginEdit(false);
                bForm.FormClosed += new FormClosedEventHandler(blobForm_FormClosed);
                bForm.ShowInTaskbar = true;
                bForm.TopMost = true;
                bForm.ShowDialog();
            }
        }

        protected virtual void dgTableView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            DataGridView dView = (DataGridView)sender;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dView.Tag;
            if (cachedRows.Count < 1)
                return;
            if (e.ColumnIndex > 0)
            {
                if (e.RowIndex < cachedRows.Count && cachedRows[e.RowIndex] != null)
                    e.Value = cachedRows[e.RowIndex].Data[e.ColumnIndex - 1];
            }
            else
                e.Value = cachedRows[e.RowIndex].Checked;
        }

        protected virtual void dgTableView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView dgTableView = (DataGridView)sender;
            if (dgTableView.IsCurrentCellInEditMode)
                return;
            System.Windows.Forms.DataGridView.HitTestInfo hitTestInfo = dgTableView.HitTest(e.X, e.Y);
            // If column is first column
            if (hitTestInfo.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right)
            {
                cellMenu.Show(dgTableView, e.X, e.Y);
                cellRowMenu = hitTestInfo.RowIndex;
                cellColumnMenu = hitTestInfo.ColumnIndex;
            }
        }
        private void setToNULLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgTableView = (DataGridView)cellMenu.SourceControl;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            QueueForUpdateRow(dgTableView, cellRowMenu, cellColumnMenu, dgTableView.Rows[cellRowMenu].Cells[cellColumnMenu].Value);
            dgTableView.Rows[cellRowMenu].Cells[cellColumnMenu].Value = null;
            dgTableView.InvalidateCell(cellColumnMenu, cellRowMenu);
            cachedRows[cellRowMenu].Data[cellColumnMenu - 1] = null;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgTableView = (DataGridView)cellMenu.SourceControl;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            Clipboard.SetDataObject(cachedRows[cellRowMenu].Data[cellColumnMenu - 1]);
        }
        private void viewAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgTableView = (DataGridView)cellMenu.SourceControl;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            string s = cachedRows[cellRowMenu].Data[cellColumnMenu - 1].ToString();
            TextForm tf = new TextForm(s);
            tf.Show(this.parentForm);
        }
        private void pasteDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgTableView = (DataGridView)cellMenu.SourceControl;
            List<SqlDataGridViewRow> cachedRows = (List<SqlDataGridViewRow>)dgTableView.Tag;
            QueueForUpdateRow(dgTableView, cellRowMenu, cellColumnMenu, dgTableView.Rows[cellRowMenu].Cells[cellColumnMenu].Value);
            dgTableView.Rows[cellRowMenu].Cells[cellColumnMenu].Value = null;
            dgTableView.InvalidateCell(cellColumnMenu, cellRowMenu);
            cachedRows[cellRowMenu].Data[cellColumnMenu - 1] = Clipboard.GetData("text");
        }
	}
}
