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
    public partial class TextForm : Form
    {

        private bool cancelForm;
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        private int rowIndex;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }
        private int columnIndex;

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

        public TextForm()
        {
            InitializeComponent();
        }

        public TextForm(string s)
        {
            this.content = s;
            InitializeComponent();
            if (s != null) 
                this.textBox1.Text=s;
        }

        public TextForm(string s, int row, int col)
        {
            this.rowIndex = row;
            this.columnIndex = col;
            this.content = s;
            InitializeComponent();
            this.textBox1.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.content = textBox1.Text;
            cancelForm = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancelForm = true;
            this.Close();
        }

        private void TextForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}