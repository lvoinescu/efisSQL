using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBMS.SQLite
{
    public partial class InputNewIndexForm : Form
    {
        private bool canceled;
        private string indexName;
        private string temp;
        private bool isPrimary;

       
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
            this.indexName = textBox1.Text;
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


    }
}