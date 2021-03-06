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
using System.Drawing;
using System.Windows.Forms;

namespace DBMS.core
{
    public partial class TypeCalendarForm : Form
    {

        public enum Mode { Date, DateAndTime, Time}

        private bool cancelForm;
        private int rowIndex;
        private int columnIndex;
        private string newValue;
        private string format;
        public string NewValue
        {
            get { return newValue; }
            set { newValue = value; }
        }
        private string currentValue;
        public string Value
        {
            get {
                return dateTimePicker1.Value.ToString(format);
                
            }
            set { this.currentValue = value;
      
        }
        }


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


        public TypeCalendarForm(int row, int column, string currentValue, int width, Mode mod)
        {
            this.rowIndex = row;
            this.columnIndex = column;
            this.MinimumSize = new Size(1, 1);
            InitializeComponent();
            this.Value = Value;
            try
            {
                dateTimePicker1.Value = DateTime.Parse(currentValue);
            }
            catch (FormatException)
            {
            }
            catch (ArgumentNullException)
            {
            } 
 
            switch( mod)
            {
                case Mode.Time:
                    format = "HH:mm:ss";
                    dateTimePicker1.ShowUpDown = true;
                    break;
                case Mode.DateAndTime:
                    format = "yyyy-MM-dd HH:mm:ss";
                    break;
                case Mode.Date:
                    format = "yyyy-MM-dd";
                    break;
            }
            dateTimePicker1.CustomFormat = format;
        }


        private void TypeCalendarForm_Deactivate(object sender, EventArgs e)
        {
            this.CancelForm = true;
            this.Close();
        }

        private void TypeCalendarForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.newValue = dateTimePicker1.Value.ToString(format);
            this.cancelForm = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.cancelForm = true;
            this.Close();

        }

        private void TypeCalendarForm_Leave(object sender, EventArgs e)
        {

        }

    }
}