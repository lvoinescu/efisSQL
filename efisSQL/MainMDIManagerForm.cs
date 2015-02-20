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
using System.Data.Odbc;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DBMS;
using DBMS.core;

namespace efisSQL
{
	public partial class MainMDIManagerForm : Form
	{



		public MainMDIManagerForm()
		{
			InitializeComponent();
		}

		
		private void MainMDIManagerForm_MdiChildActivate(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
			{
				string[] dataBases = (ActiveMdiChild as DBManagerForm).GetDatabases();
				tableToolStripMenuItem.DropDown = (this.ActiveMdiChild as DBManagerForm).CurrentDbBrowser.TableMenu;
				dataToolStripMenuItem.DropDown = (this.ActiveMdiChild as DBManagerForm).CurrentDbBrowser.DataBaseMenu;
				toolsToolStripMenuItem.DropDown = (this.ActiveMdiChild as DBManagerForm).CurrentDbBrowser.ToolsMenu;
			}
		}

		public void ChildBrowserChanged(object sender, BrowserChangedEventArgs e)
		{
			tableToolStripMenuItem.DropDown = e.Browser.TableMenu;
			dataToolStripMenuItem.DropDown = e.Browser.DataBaseMenu;
			toolsToolStripMenuItem.DropDown = e.Browser.ToolsMenu;
		}

		#region ToolStripObjects

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			SelectDBEngineForm dbForm = new SelectDBEngineForm();
			dbForm.ShowInTaskbar = false;
			dbForm.Show(this);
		}
		
		private void ExecuteAllFromDbForm(object sender, EventArgs e)
		{
			if(this.ActiveMdiChild is DBManagerForm)
			{
				DBManagerForm x = ActiveMdiChild as DBManagerForm;
				x.ExecuteAll();
			}
		}

		private void queryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
			{
				(ActiveMdiChild as DBManagerForm).AddNewDataControl();
			}
		}

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			SelectDBEngineForm dbForm = new SelectDBEngineForm();
			dbForm.Show(this);

		}

		private void disconnectStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
			{
				(ActiveMdiChild as DBManagerForm).Disconnect();
			}

		}
		

		private void showQueryEditorResultToolStripMenuItem_Click(object sender,  EventArgs e)
		{
			if(showQueryEditorResultToolStripMenuItem.CheckState==CheckState.Unchecked)
			{
				showQueryEditorOnlyToolStripMenuItem.CheckState = CheckState.Unchecked;
				showResultOutputOnlyToolStripMenuItem.CheckState = CheckState.Unchecked;
				showQueryEditorResultToolStripMenuItem.CheckState = CheckState.Checked;
				if (this.ActiveMdiChild is DBManagerForm)
				{
					(ActiveMdiChild as DBManagerForm).ShowElements(DataUserView.ShowMode.All);
				}
			}
		}

		private void showQueryEditorOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showQueryEditorOnlyToolStripMenuItem.CheckState == CheckState.Unchecked)
			{
				showResultOutputOnlyToolStripMenuItem.CheckState = CheckState.Unchecked;
				showQueryEditorResultToolStripMenuItem.CheckState = CheckState.Unchecked;
				showQueryEditorOnlyToolStripMenuItem.CheckState = CheckState.Checked;
				if (this.ActiveMdiChild is DBManagerForm)
				{
					(ActiveMdiChild as DBManagerForm).ShowElements(DataUserView.ShowMode.OnlyQueryEditor);
				}
			}
		}

		private void showResultOutputOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showResultOutputOnlyToolStripMenuItem.CheckState == CheckState.Unchecked)
			{
				showQueryEditorResultToolStripMenuItem.CheckState = CheckState.Unchecked;
				showQueryEditorOnlyToolStripMenuItem.CheckState = CheckState.Unchecked;
				showResultOutputOnlyToolStripMenuItem.CheckState = CheckState.Checked;
				if (this.ActiveMdiChild is DBManagerForm)
				{
					(ActiveMdiChild as DBManagerForm).ShowElements(DataUserView.ShowMode.OnlyDataResult);
				}
			}
		}

		private void showObjectBrowserToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).SetDataBrowserState(showObjectBrowserToolStripMenuItem.Checked);
		}


		private void saveStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Save();
		}

		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).SaveAs();
		}
		#endregion

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).LoadQuery();

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.ExitThread();
		}

		private void undoStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Undo() ;

		}

		private void redoStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Redo();

		}

		private void cutStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Cut();

		}

		private void copyStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Copy();

		}

		private void pasteStripButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).Paste();

		}

		private void ExecuteCurrentQuery(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ExecuteActiveQuery();

		}

		private void ExecuteSelection(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ExecuteSelectionQuery();

		}

		private void dropDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).RefreshElement();

		}


		private void saveAllMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.MdiChildren.Length; i++)
				if (this.MdiChildren[i] is DBManagerForm)
					(this.MdiChildren[i] as DBManagerForm).SaveAll();
		}

		private void dataToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			if (this.dataToolStripMenuItem.DropDown.IsDisposed)
			{
				this.dataToolStripMenuItem.DropDown = null;
			}
		}

		private void tableToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			if (this.tableToolStripMenuItem.DropDown.IsDisposed)
			{
				this.tableToolStripMenuItem.DropDown = null;
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).RefreshElement();
		}

		private void toolStripButton2_Click_1(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).BackUpDataBase();
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ExportTable();
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ExecuteBatchFile();

		}

		private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}


		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ShowImportTableFromCSVForm();
		}

		public void ResolveIconBug(Form child)
		{
			ActivateMdiChild(null);
			ActivateMdiChild(child);
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutForm aForm = new AboutForm();
			aForm.ShowDialog();
		}

		private void MainMDIManagerForm_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void MainMDIManagerForm_Load(object sender, EventArgs e)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				Console.WriteLine(assembly.GetName());
			}
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ShowFind(false);
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).ShowFind(true);
		}

		private void newTableButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).NewTable();
		}

		private void newDataBaseButton_Click(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild is DBManagerForm)
				(ActiveMdiChild as DBManagerForm).NewDatabase();
		}







	}
}