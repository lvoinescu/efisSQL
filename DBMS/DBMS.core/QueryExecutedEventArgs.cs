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
using DBMS.core;

namespace DBMS.core
{
    public class QueryExecutedEventArgs
    {
        private HistoryElement historyElement;
        private object data;
        private long rowsReturned;
        private string errorMessage;
        private DataUserView dataUserView;

        public DataUserView DataUserView
        {
            get { return dataUserView; }
            set { dataUserView = value; }
        }
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public long RowsReturned
        {
            get { return rowsReturned; }
            set { rowsReturned = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }
  
        public HistoryElement HistoryElement
        {
            get { return historyElement; }
            set { historyElement = value; }
        }

        public QueryExecutedEventArgs(HistoryElement element)
        {
            this.historyElement = element;
        }



    }
}
