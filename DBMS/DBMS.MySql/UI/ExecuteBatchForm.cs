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
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;
using DBMS.core;
using DBMS.core.SqlParser;
using MySql.Data.MySqlClient;

namespace DBMS.MySQL
{
	public partial class ExecuteBatchForm : Form
	{
		DateTime start, stop;
		private string fileName;
		long lenght;
		delegate void QueryItemReadn(object sender, EventArgs e);
		MySqlConnection connection;
		MySqlCommand cmd;
		DockingManager _manager;
		int nrOfExecQueries;
		RichTextBox logText;
		private long fileSize;
		private bool ReadOnly = true;
		private int readQueries;
		private List<DisplayObject> dgList;
		AutoResetEvent mrs = new AutoResetEvent(true);
		MySQLDBBrowser browser;
		SqlParser parser;
		class EventErrorArgs
		{
			private long position;

			public long Position
			{
				get { return position; }
				set { position = value; }
			}
			private string error;

			public string Error
			{
				get { return error; }
				set { error = value; }
			}

			public EventErrorArgs(long position,string error)
			{
				this.position = position;
				this.error = error;
			}
		}

		enum PostMode { InProgress, AtEnd };
		
		class EventProcessingArgs
		{

			private Exception error;

			public Exception Error
			{
				get { return error; }
				set { error = value; }
			}

			private List<string> result;

			public List<string> Result
			{
				get { return result; }
				set { result = value; }
			}
			private long position;

			public long Position
			{
				get { return position; }
				set { position = value; }
			}
			private PostMode mod;

			public PostMode Mod
			{
				get { return mod; }
				set { mod = value; }
			}
			private int noExecution;

			public int NoExecution
			{
				get { return noExecution; }
				set { noExecution = value; }
			}

			public EventProcessingArgs(long position, List<string> queries, PostMode mod, int noExecution, Exception ex)
			{
				this.position = position;
				this.mod = mod;
				this.noExecution=noExecution;
				this.error = ex;
				this.result = queries;
			}
		}

		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		private Encoding encoding;
		
		public ExecuteBatchForm(string fileName, MySQLDBBrowser browser )
		{
			this.browser = browser;
			this.fileName = fileName;
			cmd = new MySqlCommand("", connection);
			InitializeComponent();
			_manager = new DockingManager(this, VisualStyle.IDE);
			_manager.AutoResize = false;
			logText = new RichTextBox();
			logText.BorderStyle = BorderStyle.None;
			Content c1 = _manager.Contents.Add(logText, "Status");
			c1.CloseButton = false;
			c1.CloseOnHide = false;
			c1.DisplaySize = new Size(350, 250);
			string connString = this.browser.ConnectionString;
			string toFind = "charset=";
			int start = connString.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
			int end = connString.IndexOf(";", start);
			string newConnectionString = connString.Substring(0,start) + "charset=cp1257" + connString.Substring(end, connString.Length - end);
			parser = new SqlParser();
			parser.QueriesRead+= new QueriesReadDelegate(parser_QueriesRead);
			connection = new MySqlConnection(newConnectionString);
			try
			{
				connection.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot open a new conection for backup" + Environment.NewLine + ex.Message);
				return;
			}
			try{
				
				
				if(browser.CurrentDatabase!="" && browser.CurrentDatabase!=null)
				{
					MySqlCommand cmdb = new MySqlCommand("use `" + browser.CurrentDatabase +"`;", connection);
					cmdb.ExecuteNonQuery();
				}
			}
			catch(MySqlException ex)
			{
				logText.AppendText(ex.Message + Environment.NewLine);
			}
			
			
			_manager.AddContentWithState(c1, State.DockBottom);
			_manager.OuterControl = statusStrip1;
			Column1.Width = 50;
			Column2.Width = 50;
			_manager.ToggleContentAutoHide(c1);
			_manager.ToggleContentAutoHide(c1);

		}
		


		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			nrOfExecQueries = 0;
			readQueries = 0;
			dg.RowCount = 0;
			long bytes1 = GC.GetTotalMemory(true);
			if (backgroundWorker1.IsBusy)
				return;
			dg.RowCount = 0;

			dgList = new List<DisplayObject>();
			toolStripStatusLabel1.Visible = true;
			toolStripStatusLabel2.Visible = true;
			toolStripStatusLabel3.Visible = true;
			toolStripStatusLabel4.Visible = true;
			fileSizeText.Visible = true;
			if (!File.Exists(textBox1.Text))
			{
				MessageBox.Show("File does not exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			cmd = new MySqlCommand("", connection);
			fileName = textBox1.Text;
			toolStripStatusLabel2.Text = "0";
			logText.ForeColor = Color.Black;
			FileInfo f = new FileInfo(fileName);
			lenght = f.Length;
			fileSize = lenght;
			fileSizeText.Text = "/" + Utils.FormatBytes(fileSize);
			start = DateTime.Now;
			logText.AppendText("Job started at: " + DateTime.Now.ToString("hh:mm:ss") + "\n");
			logText.AppendText("File name: " + fileName + "\n");
			logText.AppendText("File size: " + lenght.ToString() + " bytes\n");
			dg.Rows.Clear();
			ReadOnly= ( mode.SelectedIndex == 1);
			progressBar1.Visible = true;

			backgroundWorker1.RunWorkerAsync();

		}

		private void parser_QueriesRead(object senderParser, QueriesReadEventArgs arg) {
			readQueries += arg.Queries.Count;
			int id = dg.Rows.Count;
			if (!ReadOnly)
			{
				for (int i = 0; i < arg.Queries.Count; i++)
				{
					cmd = new MySqlCommand(arg.Queries[i], connection);
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch(Exception ex)
					{
						backgroundWorker1.ReportProgress( Convert.ToInt16(arg.PercentageRead), new EventProcessingArgs(arg.BytesRead, arg.Queries, PostMode.InProgress, 0, ex));
					}
					nrOfExecQueries++;
				}
			}

			backgroundWorker1.ReportProgress( Convert.ToInt16(arg.PercentageRead), new EventProcessingArgs(arg.BytesRead, arg.Queries, PostMode.InProgress, 0, null));
		}
		
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			logText.Clear();
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			OpenFileDialog oDiag = new OpenFileDialog();
			oDiag.Filter = "Sql files|*.sql|Text files|*.txt|All files|*.*";
			oDiag.ShowDialog();
			if(oDiag.FileName!="")
				textBox1.Text = oDiag.FileName;

		}

		private void ExecuteBatchForm_Load(object sender, EventArgs e)
		{
			mode.SelectedIndex = 0;
			backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
			backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
		}

		private void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || (e.ColumnIndex!=2&& e.ColumnIndex!=3))
				return;
			if (e.ColumnIndex == 2)
			{
				TextForm tForm = new TextForm((string)dgList[e.RowIndex].Data.ToString());
				tForm.Show(this);
			}
			else
				if (e.ColumnIndex == 3)
			{
				using (MySqlCommand cmd = new MySqlCommand((string)dgList[e.RowIndex].Data.ToString(), connection))
				{
					try
					{
						int nrOfRowAffected = cmd.ExecuteNonQuery();
						logText.AppendText(string.Format("{0} row(s) affected\n", nrOfRowAffected));
					}
					catch (MySqlException ex)
					{
						logText.AppendText(ex.Message + "\n");
					}
					catch (Exception ex)
					{
						logText.AppendText(ex.Message + "\n");
					}
				}
			}

		}

		private void dg_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			string s = "";
			switch (e.ColumnIndex)
			{
				case 0:
					e.Value = e.RowIndex;
					break;
				case 1:
					e.Value = dgList[e.RowIndex].Len;
					break;
				case 2:
					s=dgList[e.RowIndex].Data.ToString();
					e.Value = s.Length > 300 ? s.Substring(0, 300)+"..." : s;
					break;
				case 3:
					e.Value = "Execute";
					break;
				default:
					break;
			}
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			backgroundWorker1.CancelAsync();
		}

		
		
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			FileStream fileStream  = null;
			try
			{
				fileStream= new FileStream(fileName, FileMode.Open, FileAccess.Read);
				parser.InputStream = fileStream;
				parser.ReadQueries();
			}
			catch(MySqlException ex)
			{
				logText.AppendText("Error while executing queries");
			}
			finally{
				fileStream.Close();
			}
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			EventProcessingArgs arg = (EventProcessingArgs)e.UserState;
			if(arg.Result!=null)
				if(mode.SelectedIndex==1)
					if (arg.Result.Count > 0)
			{
				for (int i = 0; i < arg.Result.Count; i++)
					dgList.Add(new DisplayObject(0, arg.Result[i].Length, arg.Result[i]));
				dg.RowCount += arg.Result.Count;
			}
			int x = (int)(100 * ((double)arg.Position / lenght));
			progressBar1.Value = x;
			toolStripStatusLabel2.Text = Utils.FormatBytes(arg.Position);
			if (arg.Error == null)
				toolStripStatusLabel3.Text = nrOfExecQueries.ToString();
			else
				logText.AppendText(arg.Error.Message + Environment.NewLine);
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				stop = DateTime.Now;
				_manager.Contents[0].BringToFront();
				logText.AppendText("Number of identified querries: " + readQueries.ToString() + "\n");
				logText.AppendText("Number of executed querries: " + nrOfExecQueries.ToString() + "\n");
				logText.AppendText("Job ended at: " + stop.ToString("hh:mm:ss" + "\n"));
				TimeSpan t = stop.Subtract(start);
				if (t.TotalSeconds > 5)
					logText.AppendText("Execution time: " + t.TotalSeconds.ToString() + " seconds.");
				else
					logText.AppendText("Execution time: " + t.TotalMilliseconds.ToString() + " milliseconds.");
				logText.AppendText("\n\n");

			}
			else
				logText.AppendText(e.Error.Message);
			progressBar1.Visible = false;

		}




		private class DisplayObject
		{
			private object tag;

			public object Tag
			{
				get { return tag; }
				set { tag = value; }
			}
			private int id;

			public int Id
			{
				get { return id; }
				set { id = value; }
			}
			private int len;

			public int Len
			{
				get { return len; }
				set { len = value; }
			}
			private object data;

			public object Data
			{
				get { return data; }
				set { data = value; }
			}

			public DisplayObject(int id, int len, object data)
			{
				this.id = id;
				this.len = len;
				this.data = data;
			}
		}

		private void ExecuteBatchForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.Dispose();
			GC.Collect();
		}

		private void ExecuteBatchForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (backgroundWorker1.IsBusy)
			{
				if (MessageBox.Show("Cancel the work that is still in progress?", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
					e.Cancel = true;
				else
				{
					backgroundWorker1.ProgressChanged -= backgroundWorker1_ProgressChanged;
					backgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted;
					backgroundWorker1.CancelAsync();
					connection.Close();
					connection.Dispose();
				}
			}
			
		}




	}
}