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

namespace DBMS.core
{
    public partial class SetTypeForm : Form
    {
        private string[] newSet;
        private string[] currentSet;

        public string[] CurrentSet
        {
            get { return currentSet; }
            set { currentSet = value; }
        }
        private bool cancelForm;
        private int rowIndex;
        private int columnIndex;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        public bool CancelForm
        {
            get { return cancelForm; }
            set { cancelForm = value; }
        }

        public string[] Set
        {
            get {
                string[] s = new string[listView1.CheckedItems.Count];
                for (int i = 0; i < listView1.CheckedItems.Count; i++)
                    s[i] = listView1.CheckedItems[i].Text;
                return s;
            }
        }

        public SetTypeForm(int row, int column,string [] set,string currentSet,int width)
        {
            this.rowIndex = row;
            this.columnIndex = column;
            this.newSet = set;
            if(currentSet!=null)
            this.currentSet = currentSet.Split(',');
            InitializeComponent();
            if (width < 220)
                this.Width = 220;
            else
                this.Width = width;
        }

        private void TypeSetForm_Load(object sender, EventArgs e)
        {
            listView1.ItemChecked -= listView1_ItemChecked;
            listView1.Items.Clear();
            for (int i = 0; i < newSet.Length; i++)
                listView1.Items.Add(newSet[i],newSet[i]);
            
            if(currentSet!=null)
                if(currentSet.Length>0)
                    for (int i = 0; i < currentSet.Length; i++)
                    {
                        int id = Array.IndexOf(newSet,currentSet[i]);
                        if(id>-1)
                            listView1.Items[id].Checked = true;
                        
                    }
            listView1.ItemChecked += new ItemCheckedEventHandler(listView1_ItemChecked);

        }

        private void TypeSetForm_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            cancelForm = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cancelForm = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancelForm = true;
            this.Close();
        }
    }
}