using System;
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