using System;

namespace DBMS.core
{
    public delegate void DelegateTableNeedsRefreshed(object sender, TableRefreshArgs e);
    public delegate void DelegateSavedQueryHandler(object sender, QuerySavedEvent e);

}
