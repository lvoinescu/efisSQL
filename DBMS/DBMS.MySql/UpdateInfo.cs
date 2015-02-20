using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.MySQL
{


    public enum UpdateType { Update, Insert };



    public class UpdateInfo
    {
        public List<UpdatedCell> elements;
        public UpdateType tipUpdate;

        public UpdateInfo(UpdateType type)
        {
            this.tipUpdate = type;
            elements = new List<UpdatedCell>();
        }

    }
}
