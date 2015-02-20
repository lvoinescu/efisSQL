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

namespace DBMS.MySQL
{
    public partial class InputNewIndexForm : Form
    {
        private bool canceled;
        private string indexName;
        private string temp;
        private bool isPrimary;

        public bool IsPrimary
        {
            get { return checkBox1.Checked; }
        }

        public string IndexName
        {
            get { return indexName; }
            set { indexName = value; }
        }
        public bool Canceled
        {
            get { return canceled; }
            set { canceled = value; }
        }

        public InputNewIndexForm()
        {
            InitializeComponent();
            canceled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            canceled = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
                indexName = temp;
            else
                indexName = "PRIMARY";

            canceled = false;
            this.Close();
        }

        private void InputNewIndexForm_Load(object sender, EventArgs e)
        {
            canceled = true;
        }

        private void InputNewIndexForm_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !checkBox1.Checked;
            if (checkBox1.Checked)
            {
                textBox1.Text = "PRIMARY";
            }
            else
                textBox1.Text = temp;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
                if(!checkBox1.Checked)
                    temp = textBox1.Text;
        }
    }
}