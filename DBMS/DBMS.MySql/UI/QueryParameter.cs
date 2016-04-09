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

namespace DBMS.MySQL
{
    public class QueryParameter
    {
        private string parameterName;

        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }
        private byte[] parameterValue;

        public byte[] ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }

        public QueryParameter(string parName, byte[] value)
        {
            this.parameterName = parName;
            this.parameterValue = value;
        }
    }
}
