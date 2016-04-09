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

namespace DBMS.core
{
    public delegate void QueryExecutedHandler(object sender,QueryExecutedEventArgs e);
    public delegate void TableBoundedHandler(object sender, TableBoundedArg e);
    public delegate void TreeNodeRefreshedHandler(object sender, TreeNodeRefreshedHandler e);
    public delegate void NewDataUserViewRequestedHandler(object sender, object e);
    public delegate void DisconnectRequestedEventHandler(object sender, object e);



    public enum SQLNodeType { Server, Database, Table, View, Procedure, Function, Trigger, Column, Index }
    public enum SQLNodeStatus { Refreshed, Unset }
}
