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
using System.Text;
using System.Windows.Forms;

namespace DBMS.core
{
    public class TableBoundedArg
    {

        public TableBoundedArg(string dataBase, string tableName, TreeNode tableNode, string error)
        {
            this.tableTreeNode = tableNode;
            this.tableName = tableName;
            this.dataBase = dataBase;
            this.error = error;
        }



        private string tableName;
        private string dataBase;
        private string error;
        private TreeNode tableTreeNode;

        public TreeNode TableTreeNode
        {
            get { return tableTreeNode; }
            set { tableTreeNode = value; }
        }
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public string DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

    }

}
