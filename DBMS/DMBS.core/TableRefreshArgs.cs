/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 5/22/2012
 * Time: 12:11 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace DBMS.core
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
    public enum Order {Ascending, Descending};

    public class TableRefreshArgs
    {
		private DataGridViewColumn sortColumn;
		    	
        public TableRefreshArgs(TreeNode node,DataGridViewColumn sortColumn)
        {
            this.node = node;
            this.sortColumn = sortColumn;
        }
        private TreeNode node;

        public TreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

    }
}
