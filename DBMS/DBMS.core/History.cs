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
using System.Collections;


namespace DBMS.core
{
   

    public class History : CollectionBase,System.Collections.Generic.IEnumerable<HistoryElement> , IDisposable
    {

        public History()
        {
            
        }

       
        public int Add(HistoryElement value)
        {
            return InnerList.Add(value);
        }

        public new IEnumerator<HistoryElement> GetEnumerator()
        {
            foreach (HistoryElement data in InnerList)
            {
                yield return data;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
