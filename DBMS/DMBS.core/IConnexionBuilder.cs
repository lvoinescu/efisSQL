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
using System.Data;
using System.Data.Odbc;
namespace DBMS.core
{
    public interface IConnectionBuilder
    {

        bool Enabled { get; set;}
        bool Changed { get; set;}
        string GetString();
        string GetServerAddress();
        IConnectionSetting GetConnectionSettings(string name);
        object GetConnection();
        string GetDBEngine();
        IDBBrowser GetBrowser();
        IDBBrowser CreateBrowser();
        void ValidateConnection(string connectionName, IConnectionSetting connectionSetting);
        bool OpenConnection();
        bool TestConnection();

    }
}
