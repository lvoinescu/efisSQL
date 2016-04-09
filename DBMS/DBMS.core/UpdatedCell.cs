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
    public class UpdatedCell
    {
        public object newValue;
        public object oldValue;
        public string columnName;
        public int columnIndex;

        public UpdatedCell(object newValue, object oldValue, string columnName, int columnIndex)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
            this.columnName = columnName;
            this.columnIndex = columnIndex;
        }

    }
}
