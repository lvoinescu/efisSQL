using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.MySQL
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
