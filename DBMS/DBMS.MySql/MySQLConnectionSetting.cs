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
using System.Xml.Serialization;
using DBMS.core;

namespace DBMS.MySQL
{
    [XmlInclude(typeof(ConnectionSetting))]
    [XmlRootAttribute("MySqlConnection", Namespace = "", IsNullable = false)]
    public class MySqlConnectionSetting : ConnectionSetting , IConnectionSetting
    {
        /// <summary>
        /// Default constructor for this class (required for serialization).
        /// </summary>
        public MySqlConnectionSetting()
        {
        }
        
        public MySqlConnectionSetting(string connectionName):base()
        {
            this.ConnectionName = connectionName;
        }

       
        private string host;
        [XmlElement(DataType = "string")]
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        
        private string userName;
        [XmlElement(DataType = "string")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        
        private string port;
        [XmlElement(DataType = "string")]
        public string Port
        {
            get { return port; }
            set { port = value; }
        }
        
        private string charSet;
        [XmlElement(DataType = "string")]
        public string CharSet
        {
            get { return charSet; }
            set { charSet = value; }
        }
        
        private string password;
        [XmlElement(DataType = "string")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string databases;
        [XmlElement(DataType = "string")]
        public string Databases
        {
            get { return databases; }
            set { databases = value; }
        }

    }

}
