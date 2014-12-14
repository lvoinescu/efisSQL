using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBMS.SQLite
{
    public partial class SelectFkeyForm : Form
    {
        public SelectFkeyForm(string [] tables, string[][]keys)
        {
            InitializeComponent();
        }
    }
}