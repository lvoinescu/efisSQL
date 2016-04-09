using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DBMS.core
{
    public delegate void DelegateTableNeedsRefreshed(object sender, TableRefreshArgs e);
    public delegate void DelegateSavedQueryHandler(object sender, QuerySavedEvent e);

}
