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
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Crownwood.Magic.Controls;
    using Crownwood.Magic.Docking;
    using Crownwood.Magic.Common;
    using DBMS.core;
    using System.Threading;
    using System.Collections;
    using System.Drawing.Drawing2D;

    namespace efisSQL
    {



        public partial class DBManagerForm : Form
        {

            public event BrowserChangedDelegate BrowserChanged;

            protected DockingManager _manager;
            protected StatusBar _statusBar;
            protected VisualStyle _style;
           // protected TreeView browseTree;
            protected int noOfDataViews = 0;
            protected string[] dataBases;
            private IDBBrowser currentDbBrowser;
            
            private int noOfQueries;
            private TreeNode currentNode;


            private AsyncOperation asyncOp;
            private List<BackgroundWorker> backgroundWorker;
            private Hashtable boundingTableBrowser;
            private sealed class AsyncRefreshArgument
            {
                private int idBrowser;
                private TreeNode node;
                private string methode;
                private object result;

                public object Result
                {
                    get { return result; }
                    set { result = value; }
                }


                public AsyncRefreshArgument(int id, TreeNode node, string methode)
                {
                    this.idBrowser = id;
                    this.node = node;
                    this.methode = methode;
                }

                public string Methode
                {
                    get { return methode; }
                    set { methode = value; }
                }
                

                public TreeNode Node
                {
                    get { return node; }
                    set { node = value; }
                }


                public int IdBrowser
                {
                    get { return idBrowser; }
                    set { idBrowser = value; }
                }
            }

            public IDBBrowser CurrentDbBrowser
            {
                get { return currentDbBrowser; }
            }


            private List<Crownwood.Magic.Controls.TabbedGroups> tabGroupBrowserCotrol;
            private List<List<DataUserView>> dataUserView;
            protected List<IDBBrowser> dbBrowser;


            public DBManagerForm()
            {
                backgroundWorker = new List<BackgroundWorker>();
                asyncOp = AsyncOperationManager.CreateOperation(this);
                tabGroupBrowserCotrol = new List<TabbedGroups>();
                boundingTableBrowser = new Hashtable();
                dataUserView = new List<List<DataUserView>>();
                dbBrowser = new List<IDBBrowser>();
                this.InitializeComponent();
            }


            public void AddNewBrowser(IDBBrowser browser)
            {

                browser.NewDataUserViewRequested += new NewDataUserViewRequestedHandler(browser_NewDataUserViewRequested);
                browser.DisconnectRequestedEvent += new DisconnectRequestedEventHandler(browser_DisconnectRequestedEvent);
                browser.QueryExecuted += new QueryExecutedHandler(browser_QueryExecuted);
                browser.TableBounded += new TableBoundedHandler(browser_TableBounded);
                browser.ParentForm = this;
                
                browser.RegisterTreeView(browseTree);
                TreeNode nodS = browser.CreateMainTreeNode();
                browseTree.Nodes.Add(nodS);

                this.dbBrowser.Add(browser);
                if (BrowserChanged != null)
                    this.BrowserChanged(this, new BrowserChangedEventArgs(browser));
                currentDbBrowser = browser;


                currentNode = nodS;
                
                //RefreshServer(nodS);
                Crownwood.Magic.Controls.TabbedGroups tabGroup = new Crownwood.Magic.Controls.TabbedGroups();
                this.tabGroupBrowserCotrol.Add(tabGroup);
                tabGroup.PageCloseRequest += new TabbedGroups.PageCloseRequestHandler(tabGroup_PageCloseRequest);
                TabGroupLeaf tgl = tabGroup.RootSequence[0] as TabGroupLeaf;

                Crownwood.Magic.Controls.TabPage page = new Crownwood.Magic.Controls.TabPage(nodS.Text, tabGroup, 0);
                page.Tag = currentDbBrowser;

                DataUserView dView = new DataUserView(this, browser);
                List<DataUserView> ldata = new List<DataUserView>();
                ldata.Add(dView);
                dView.QuerySaved+=new DelegateSavedQueryHandler(dView_QuerySaved);
                dView.TableRefresh+=new DelegateTableNeedsRefreshed(dView_TableRefresh);
                dataUserView.Add(ldata);
                browser.SetQueryInput(dView.queryText);
                browser.MasterForm = this;
                browser.SetHistoryOutput(dView.historyText);
                Crownwood.Magic.Controls.TabPage pageDataView = new Crownwood.Magic.Controls.TabPage("Query" + (noOfQueries++).ToString(), dView, 10);
                pageDataView.Tag = dView;
                dView.Tag = pageDataView;
                tgl.TabPages.Add(pageDataView);
                tabBrowser.TabPages.Add(page);
                page.Selected = true;

                browseTree.SelectedNode = nodS;
                browser.RefreshNode(nodS);
                nodS.Expand();

            }


            private void browser_NewDataUserViewRequested(object sender, object e)
            {
                string s = "";
                if (e != null)
                    s = (string)e;
                IDBBrowser browser= (IDBBrowser)sender;
                AddNewDataControl(browser,s);
            }

            private void browser_DisconnectRequestedEvent(object sender, object e)
            {
                IDBBrowser browser = (IDBBrowser)sender;
                browser.Disconnect();
                browser.NewDataUserViewRequested -= browser_NewDataUserViewRequested;
                browser.QueryExecuted -= browser_QueryExecuted;
                browser.TableBounded -= browser_TableBounded;
                browser.DisconnectRequestedEvent -= browser_DisconnectRequestedEvent;


  

                int i = GetIndexOfRootParent(currentNode);
                for (int j = 0; j < dataUserView[i].Count; j++)
                {
                    dataUserView[i][j].QuerySaved -= dView_QuerySaved;
                    dataUserView[i][j].TableRefresh -= dView_TableRefresh;
                    dataUserView[i][j].Dispose();
                }
                currentDbBrowser = dbBrowser[i];
                dbBrowser[i].Disconnect();
                dbBrowser.RemoveAt(i);
                dataUserView.RemoveAt(i);
                Crownwood.Magic.Controls.TabControl x = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Controls.Clear();
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Dispose();
                tabGroupBrowserCotrol.RemoveAt(i);
                tabBrowser.TabPages.RemoveAt(i);
                browseTree.Nodes.RemoveAt(i); ;
                if (dbBrowser.Count == 0)
                {
                    this.Close();
                    this.Dispose();
                }

            }

            public void AddNewDataControl()
            {
                int i = GetIndexOfRootParent(currentNode);
                AddNewDataControl(dbBrowser[i],null);

            }

            private void AddNewDataControl(IDBBrowser browser, string query)
            {
                int i = GetIndexOfRootParent(currentNode);
                DataUserView dView = new DataUserView(this,browser);
                if (query != null)
                    dView.queryText.Document.Insert(0,query);
                dView.TableRefresh+=new DelegateTableNeedsRefreshed(dView_TableRefresh);
                dataUserView[i].Add(dView);
                Crownwood.Magic.Controls.TabPage page = new Crownwood.Magic.Controls.TabPage("Query", dView, 10);
                tabBrowser.SelectedIndex = i;
                TabGroupLeaf tgl = tabGroupBrowserCotrol[i].RootSequence[0] as TabGroupLeaf;
                Crownwood.Magic.Controls.TabPage pageDataView = new Crownwood.Magic.Controls.TabPage("Query" + (noOfQueries++).ToString(), dView, 10);
                tgl.TabPages.Add(pageDataView);

                pageDataView.Selected = true;
                pageDataView.Tag = dView;
                dView.Tag = pageDataView;
            }



            private void dView_QuerySaved(object sender, QuerySavedEvent e)
            {
                DataUserView dView = (DataUserView)sender;
                if (dView.Tag != null)
                {
                    Crownwood.Magic.Controls.TabPage page = (Crownwood.Magic.Controls.TabPage)dView.Tag;
                    int i=e.FileName.LastIndexOf('\\')+1;
                    if (i > 0)
                    {
                        page.Title = e.FileName.Substring(i, e.FileName.Length - i);
                    }
                }
            }

            private void dView_TableRefresh(object sender, TableRefreshArgs e)
            {
                DataUserView dview = (DataUserView)sender;
                if (e == null)
                    return;
                browseTree.SelectedNode = null;
                browseTree.SelectedNode = e.Node;
            }

            private void tabGroup_PageCloseRequest(object sender, TGCloseRequestEventArgs e)
            {
                int i = tabBrowser.SelectedIndex;
                Crownwood.Magic.Controls.TabControl tab = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                if (tabGroupBrowserCotrol[i].RootSequence.Count == 1 && e.TabControl.TabPages.Count == 1)
                {
                    e.Cancel = true;
                    return;
                }
                DataUserView dView = (DataUserView)e.TabPage.Tag;
                dView.RemoveEvents();
                dView.Dispose();
                dataUserView[i].Remove(dView);
            }

            private void tabBrowser_TabIndexChanged(object sender, EventArgs e)
           {
           }


            #region browseTree

            private void browseTree_AfterExpand(object sender, TreeViewEventArgs  e)
            {
                if (e.Node.Tag == null)
                    return;
                object[] x = (object[])e.Node.Tag;
                IDBBrowser br = (IDBBrowser)x[0];
                br.TreeNodeAfterExpand(sender, e);
            }

            private void browseTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
            {
                if (e.Node == null)
                    return;
                if (e.Node.Tag == null)
                    return;
                object[] x = (object[])e.Node.Tag;
                IDBBrowser br = (IDBBrowser)x[0];
                if (br.BrowserIsBusy)
                {
                    e.Cancel = true;
                    return;
                }
                br.TreeNodeBeforeSelect(sender, e);
            }

            private void browseTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
            {
                if (e.Node.Tag == null)
                    return;
                object[] x = (object[])e.Node.Tag;
                IDBBrowser br = (IDBBrowser)x[0];
                if (br.BrowserIsBusy)
                    return;
                br.TreeNodeBeforeExpand(sender, e);
            }  

            private void browseTree_AfterSelect(object sender, TreeViewEventArgs e)
            {
                currentNode = e.Node;
                browseTree.SuspendLayout();
                if (e.Node == null)
                    return;
                if (e.Node.Tag == null)
                    return;
                object[] x = (object[])e.Node.Tag;
                IDBBrowser br = (IDBBrowser)x[0];
                if (br.BrowserIsBusy)
                    return;

                br.TreeNodeAfterSelect(sender, e);
                int i = GetIndexOfRootParent(e.Node);
                tabBrowser.SelectedIndex = i;
                if (br.TreeNodeIsTableNode(e.Node))
                {
                    TabGroupLeaf tgl = tabGroupBrowserCotrol[i].ActiveLeaf;
                    Crownwood.Magic.Controls.TabControl dataUserTab = tabGroupBrowserCotrol[i].RootSequence.TabbedGroups.ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                    object t = dataUserTab.SelectedTab.Tag;
                    DataUserView dView = (DataUserView)t;
                    dView.SetViewTable(e.Node);
                }
                browseTree.Focus();
                if (br != currentDbBrowser)
                {
                    currentDbBrowser = br;
                    if (BrowserChanged != null)
                        this.BrowserChanged(this, new BrowserChangedEventArgs(br));
                }
            }

            private void browseTree_MouseDown(object sender, MouseEventArgs e)
            {
                currentNode = browseTree.GetNodeAt(e.X, e.Y);
                if (currentNode == null )
                    return;
                if (currentNode.Tag == null)
                    return;
                int i = GetIndexOfRootParent(currentNode);
                tabBrowser.SelectedIndex = i;
                object []tag =(object[])currentNode.Tag;
                IDBBrowser br = (IDBBrowser)tag[0];
                br.TreeNodeMouseDown(sender, e); 
            }

            private void browseTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
            {
                if (currentDbBrowser.TreeNodeIsTableNode(e.Node))
                {
                    if (e.Label == null)
                        return;
                    currentDbBrowser.RenameTable(e.Node.Parent.Text, e.Node.Text, e.Label);
                }
            }

            private void browseTreeControlRemoved(object sender, ControlEventArgs e)
            {
                MessageBox.Show("The control named " + e.Control.Name + " has been removed from the form.");
            }



            int pW = 16; int pH = 16;
            private void browseTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
            {
            	if(e.Node!=null)
                	DrawNode(e.Node, e.Graphics);
                //bug with draw node ... so put this here until solution
                TreeNode node = browseTree.GetNodeAt(10, 10);
                if(node!=null)
                	DrawNode(node, e.Graphics);
            }

            private void DrawNode(TreeNode node, Graphics g)
            {
                    if (node.Tag == null)
                        return;
                    object[] x = (object[])node.Tag;
                    IDBBrowser br = (IDBBrowser)x[0];
                    int imgW = br.ImageList.ImageSize.Width;
                    int imgH = br.ImageList.ImageSize.Height;

                    int xPos = node.Bounds.X;
                    int yPos = node.Bounds.Y;
                    int pmW = 40;
                    Rectangle pmRect = new Rectangle(node.Bounds.Left - pmW, node.Bounds.Top, pW, pH);


                    Rectangle imgRect = new Rectangle(xPos + pW - pmW, yPos, pW, pH);
                    g.FillRectangle(new SolidBrush(browseTree.BackColor), new Rectangle(0,yPos, browseTree.Width, pH));
                    g.DrawImage(br.ImageList.Images[node.ImageIndex], imgRect);
                    if (node.Nodes.Count > 0)
                        if (node.IsExpanded)
                            g.DrawImage(node.TreeView.ImageList.Images[0], pmRect);
                        else
                            g.DrawImage(node.TreeView.ImageList.Images[1], pmRect);
                    if (node.IsSelected)
                    {
                        SizeF strw = g.MeasureString(node.Text, node.TreeView.Font);
                        g.FillRectangle(new SolidBrush(Color.SteelBlue), imgRect.Left + imgRect.Width, node.Bounds.Top, strw.Width, strw.Height);
                    }

                    g.DrawString(node.Text, browseTree.Font, Brushes.Blue, node.Bounds.Left - pmW + 2 * pW, yPos);

            }


            #endregion

            public void RemoveTabbedGroup(TabbedGroups tg)
                {
                    int i = this.tabGroupBrowserCotrol.IndexOf(tg);
                    dbBrowser[i].QueryExecuted -= browser_QueryExecuted;
                    this.tabGroupBrowserCotrol.Remove(tg);
                    tabBrowser.TabPages.Remove(tabBrowser.SelectedTab);
                }

            protected void DefineContentState(Content c)
            {
                c.CaptionBar = true;
                c.CloseButton = false;
            }

            protected void DefineControlColors(Content c, Color backColor, Color foreColor)
            {
                // Only interested in Forms and Panels
                if ((c.Control is Form) || (c.Control is Panel))
                {
                    c.Control.BackColor = backColor;
                    c.Control.ForeColor = foreColor;
                }
            }

            private void browser_TableBounded(object sender, TableBoundedArg e)
            {
                IDBBrowser browser = (IDBBrowser)sender;
                if (e.Error != null)
                    MessageBox.Show(e.Error, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    int i = GetIndexOfRootParent(e.TableTreeNode);
                    TabGroupLeaf tgl = tabGroupBrowserCotrol[i].ActiveLeaf;
                    Crownwood.Magic.Controls.TabControl dataUserTab = tabGroupBrowserCotrol[i].RootSequence.TabbedGroups.ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                    object t = dataUserTab.SelectedTab.Tag;
                    DataUserView dView = (DataUserView)t;
                    dView.PrintBoundedTable(e.DataBase, e.TableName);
            }
            
            private void browser_QueryExecuted(object sender, QueryExecutedEventArgs e)
            {
                IDBBrowser browser = (IDBBrowser)sender;
                if (currentNode.TreeView == null)
                    return;
                int i = GetIndexOfRootParent(currentNode);
                TabGroupLeaf tgl = tabGroupBrowserCotrol[i].ActiveLeaf;
                Crownwood.Magic.Controls.TabControl dataUserTab = tabGroupBrowserCotrol[i].RootSequence.TabbedGroups.ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                object t = dataUserTab.SelectedTab.Tag;
                DataUserView dView = (DataUserView)t;
                dView.AddToHistoryOutput(e.HistoryElement);
                if (e.Data != null)
                {
                    e.DataUserView.BindTableResult(e);
                }

            }


            private int GetIndexOfRootParent(TreeNode n)
            {
                if(n==null)
                    return -1;
                while (n.Level > 0)
                    n = n.Parent;
                for (int i = 0; i < browseTree.Nodes.Count; i++)
                    if (n == browseTree.Nodes[i])
                        return i;
                return n.Index;
            }

            public void Disconnect()
            {
                int i = GetIndexOfRootParent(currentNode);
                currentDbBrowser = dbBrowser[i];
                dbBrowser[i].Disconnect();
                dbBrowser.Remove(currentDbBrowser);
                Crownwood.Magic.Controls.TabControl x = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Controls.Clear();
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Dispose();
                tabGroupBrowserCotrol.RemoveAt(i);
                tabBrowser.TabPages.RemoveAt(i);
                browseTree.Nodes.RemoveAt(i); ;
                if (dbBrowser.Count == 0)
                {
                    this.Close();
                    this.Dispose();
                }
            }

            public void SetDataBrowserState(bool state)
            {
                splitContainer1.Panel1Collapsed = !state;
            }

            public void ShowElements(DataUserView.ShowMode mode)
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Controls.TabControl x = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                int j = x.SelectedIndex;
                int index = (int)tabBrowser.SelectedIndex;
                foreach (Control c in x.SelectedTab.Controls)
                {
                }
                dataUserView[index][j].ShowElements(mode);
            }

            public void LoadQuery()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Controls.TabControl x = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                int j = x.SelectedIndex;
                int index = (int)tabBrowser.SelectedIndex;
                dataUserView[index][j].LoadQuery();
            }

            public string[] GetDatabases()
            {
                return dataBases;
            }

            public void Save()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.SaveQuery();
                        }
                    }
                }
            }

            public void SaveAs()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.SaveAsNewQuery();
                        }
                    }
                }
            }

            public void SaveAll()
            {
                for (int k = 0; k < dataUserView.Count; k++)
                {
                    for (int i = 0; i < dataUserView[k].Count; i++)
                        dataUserView[k][i].SaveQuery();
                }
            }

            public void Undo()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.Undo();
                        }
                    }
                }
            }

            public void Redo()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.Redo();
                        }
                    }
                }
            }

            public void Copy()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.CopyToClipboardFromActiveControl();
                        }
                    }
                }
            }

            public void Paste()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.PasteFromClipboardToActiveControl();
                        }
                    }
                }
            }

            public void Cut()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            if (dView.queryText.ActiveTextAreaControl.SelectionManager.SelectedText != null && dView.queryText.ActiveTextAreaControl.SelectionManager.SelectedText != "")
                            {
                                Clipboard.SetText(dView.queryText.ActiveTextAreaControl.SelectionManager.SelectedText);
                                dView.queryText.ActiveTextAreaControl.TextArea.SelectionManager.RemoveSelectedText();
                            }
                        }
                    }
                }
            }

            public void ExecuteAll()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            if (!currentDbBrowser.BrowserIsBusy)
                            {
                                DataUserView dView = (DataUserView)t.Tag;
                                dView.ExecuteAllQueries();
                            }
                            else
                            {
                                MessageBox.Show("Current browser is busy.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            
                        }
                    }
                }
            }

            public void ExecuteActiveQuery()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            if (!currentDbBrowser.BrowserIsBusy)
                            {
                                DataUserView dView = (DataUserView)t.Tag;
                                dView.ExecuteActiveQuery();
                            }
                            else
                            {
                                MessageBox.Show("Current browser is busy.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                    }
                }
            }

            public void ExecuteSelectionQuery()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.ExecuteSelectionQuery();
                        }
                    }
                }
            }

            public void BackUpDataBase()
            {
                backupToolStripMenuItem_Click(this, null);
            }

            public void ExportTable()
            {
                asCSVFileToolStripMenuItem_Click(this, null);
            }

            public void ExecuteBatchFile()
            {
                this.currentDbBrowser.ShowExecuteForm("", this.MdiParent);
            }

            public int GetNoOfConnection()
            {
                return this.dbBrowser.Count;
            }

            public void RefreshElement()
            {
                if (currentDbBrowser.BrowserIsBusy)
                    return;
                if(browseTree.SelectedNode!=null)
                    currentDbBrowser.RefreshNode(browseTree.SelectedNode);

            }

            public void NewTable()
            {
                if (currentDbBrowser.BrowserIsBusy)
                    return;
                if (currentDbBrowser.CurrentDatabase == null)
                    return;
                int i=0,k=0;
                for (i = 0; i < dbBrowser.Count; i++)
                    if (dbBrowser[i] == currentDbBrowser)
                        break;

                for (k = 0; k < browseTree.Nodes[i].Nodes.Count; k++)
                    if (browseTree.Nodes[i].Nodes[k].Text == currentDbBrowser.CurrentDatabase)
                        break;
                currentDbBrowser.ShowCreateTableForm(browseTree.Nodes[i].Nodes[k]);
            }

            public void NewDatabase()
            {
                if (currentNode == null)
                    return;
                if (currentDbBrowser.BrowserIsBusy)
                    return;
                int i = GetIndexOfRootParent(currentNode);
                currentDbBrowser.ShowNewDbForm(browseTree.Nodes[i]);
            }

            public void ShowFindAndReplace()
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.ShowFindAndReplace(true);
                        }
                    }
                }
            }

            public void ShowFind(bool replaceMode)
            {
                int i = (int)tabBrowser.SelectedIndex;
                Crownwood.Magic.Collections.TabPageCollection tColl = tabGroupBrowserCotrol[i].ActiveLeaf.TabPages as Crownwood.Magic.Collections.TabPageCollection;
                foreach (Crownwood.Magic.Controls.TabPage t in tColl)
                {
                    if (t.Selected == true)
                    {
                        if (t.Tag != null)
                        {
                            DataUserView dView = (DataUserView)t.Tag;
                            dView.ShowFindAndReplace(replaceMode);
                        }
                    }
                }
            }
     
            private void backupToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (currentDbBrowser.BrowserIsBusy)
                    return;
                TreeNode n = currentNode;
                string[] dataBases = (string[])currentDbBrowser.ListDatabases().Result;
                string[] tables;
                if (n == null)
                    return;
                if (n.Level == 1)
                {
                    QueryResult qR = currentDbBrowser.ListTables(n.Text);
                    if (qR.Message != null)
                    {
                        MessageBox.Show(qR.Message, "Error " + qR.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                        tables = (string[])qR.Result;
                    currentDbBrowser.ShowExportForm(dataBases, tables, n.Text, null);
                }
                else
                    if (currentDbBrowser.TreeNodeIsTableNode(n))
                    {
                        QueryResult qR = currentDbBrowser.ListTables(n.Parent.Text);
                        if (qR.Message != null)
                        {
                            MessageBox.Show(qR.Message, "Error " + qR.ErrorNo.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                            tables = (string[])qR.Result;
                        currentDbBrowser.ShowExportForm(dataBases, tables,currentDbBrowser.CurrentDatabase,n.Text);
                    }
            }

            private void asCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (currentDbBrowser.BrowserIsBusy)
                {
                    MessageBox.Show("Current browser is busy...", Application.ProductName, MessageBoxButtons.OK);
                    return;
                }
            	if (!currentDbBrowser.TreeNodeIsTableNode(browseTree.SelectedNode ))
                    return;
                TreeNode n = currentNode;
                if (n == null)
                    return;
                if ((n.Level != 2 && n.Tag != null) || n.Parent == null)
                    return;
                currentDbBrowser.ShowExportTableForm(n.Parent.Text, n.Text);

            }

            private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                ContextMenuStrip menuParent = (ContextMenuStrip)menuItem.Owner;
                int i = currentNode.Index;
                currentDbBrowser = dbBrowser[i];
                dbBrowser[i].Disconnect();
                dbBrowser.Remove(currentDbBrowser);
                Crownwood.Magic.Controls.TabControl x = tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Controls.Clear();
                tabGroupBrowserCotrol[i].ActiveLeaf.GroupControl.Dispose();
                tabGroupBrowserCotrol.RemoveAt(i);
                //currentDbBrowser.Dispose();
                tabBrowser.TabPages.RemoveAt(i);
                browseTree.Nodes.Remove(currentNode);
                GC.Collect(2, GCCollectionMode.Forced);
                GC.Collect(1, GCCollectionMode.Forced);
            }

            private void treeView1_ControlRemoved(object sender, ControlEventArgs e)
            {

            }

    
   

            private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                currentDbBrowser.ShowTableStatus(currentNode.Parent.Text, currentNode.Text);
            }


            private void restoreDatabaseFromSqlDumpToolStripMenuItem_Click(object sender, EventArgs e)
            {
                currentDbBrowser.ShowExecuteForm("", this);
            }

            private void swowToolStripMenuItem_Click(object sender, EventArgs e)
            {
                TreeNode n = currentNode;
                int i = tabBrowser.SelectedIndex;
                int it = (int)tabBrowser.SelectedIndex;
                if (n.Level == 3)
                {
                    dataUserView[it][i].ExecuteQuery("select * from `" + n.Parent.Parent.Text + "`.`" + n.Text + "`;");
                }

            }

            private void reorderColumnsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (currentNode != null)
                    currentDbBrowser.ShowReorderColumnForm(currentNode.Parent.Text, currentNode.Text);

            }

            private void viewReletionshipforeignKeysToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (currentNode != null)
                    currentDbBrowser.ShowReferenceTableForm(currentNode.Parent.Text, currentNode.Text);

            }

            public void ShowImportTableFromCSVForm()
            {
            	if(browseTree.SelectedNode==null || currentDbBrowser.BrowserIsBusy)
            		return;
            	if (!currentDbBrowser.TreeNodeIsTableNode(browseTree.SelectedNode))
                    return;
                int i = GetIndexOfRootParent(browseTree.SelectedNode);
                TabGroupLeaf tgl = tabGroupBrowserCotrol[i].ActiveLeaf;
                Crownwood.Magic.Controls.TabControl dataUserTab = tabGroupBrowserCotrol[i].RootSequence.TabbedGroups.ActiveLeaf.GroupControl as Crownwood.Magic.Controls.TabControl;
                object t = dataUserTab.SelectedTab.Tag;
                DataUserView dView = (DataUserView)t;
                if (dView.DataBase != null)
                    currentDbBrowser.ShowImportFromCSVForm(dView.DataBase, dView.TableName);
            }

            private void importToolStripMenuItem1_Click(object sender, EventArgs e)
            {
                currentDbBrowser.ShowImportFromCSVForm(currentNode.Parent.Text, currentNode.Text);
            }

            private void DBManagerForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                browseTree.DrawNode-= browseTree_DrawNode;
                foreach (IDBBrowser db in dbBrowser)
                {
                    db.Disconnect();
                }
                this.Dispose();
            }

            private void DBManagerForm_ResizeBegin(object sender, EventArgs e)
            {
                 browseTree.SuspendLayout();
            }

            private void DBManagerForm_ResizeEnd(object sender, EventArgs e)
            {
                browseTree.ResumeLayout();
            }

            private void button1_Click(object sender, EventArgs e)
            {

            }
 


        }

            public class BrowserChangedEventArgs
            {
                private IDBBrowser browser;

                public IDBBrowser Browser
                {
                    get { return browser; }
                    set { browser = value; }
                }
                public BrowserChangedEventArgs(IDBBrowser browser)
                {
                    this.browser = browser;
                }
            }

            public delegate void BrowserChangedDelegate(object sender, BrowserChangedEventArgs e);


        }
