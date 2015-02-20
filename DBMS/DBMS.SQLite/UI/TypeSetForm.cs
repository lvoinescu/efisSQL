using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBMS.SQLite
{
    public partial class TypeSetForm : Form
    {
        private string[] set;
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

        public TypeSetForm(int row, int column,string [] set,string currentSet,int width)
        {
            this.rowIndex = row;
            this.columnIndex = column;
            this.set = set;
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
            for (int i = 0; i < set.Length; i++)
                listView1.Items.Add(set[i],set[i]);
            
            if(currentSet!=null)
                if(currentSet.Length>0)
                    for (int i = 0; i < currentSet.Length; i++)
                    {
                        int id = Array.IndexOf(set,currentSet[i]);
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