using System;

namespace DBMS.SQLite
{
    class SQLiteDataRow
    {
        public object Tag;
        private object[] data;

        public object[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public  SQLiteDataRow(object[] data)
        {
            this.Tag=null;
            this.data = data;
        }

        public SQLiteDataRow(object[] data, object tag)
        {
            this.Tag = tag;
            this.data = data;
        }
    }
}
