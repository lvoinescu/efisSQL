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

namespace DBMS.core
{
    public enum QueryStatus { OK, Error};

    public class QueryResult : IDisposable
    {

        private DateTime startTime;
        private QueryStatus status;

        public QueryStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        private long duration;

        public long Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private Object result;

        public Object Result
        {
            get { return result; }
            set { result = value; }
        }

        private int errorNo;
        public int ErrorNo
        {
            get { return errorNo; }
            set { errorNo = value; }
        }

        public QueryResult(object result)
        {
            this.result = result;
        }

        public QueryResult(QueryStatus status, int errorNr, string message)
        {
            this.message = message;
            this.errorNo = errorNr;
            this.status = status;
        }

        public QueryResult(QueryStatus status, string message)
        {
            this.message = message;
            this.status = status;
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.result = null;
            this.message = null;
            GC.Collect();
        }

        #endregion
    }

}
