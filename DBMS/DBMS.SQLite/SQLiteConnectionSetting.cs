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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Security;
using DBMS.core;

namespace DBMS.SQLite
{
    [XmlInclude(typeof(ConnectionSetting))]
    [XmlRootAttribute("SQLiteConnection", Namespace = "", IsNullable = false)]
    public class SQLiteConnectionSetting : ConnectionSetting , IConnectionSetting
    {
        /// <summary>
        /// Default constructor for this class (required for serialization).
        /// </summary>
        public SQLiteConnectionSetting()
        {
        }
        
        public SQLiteConnectionSetting(string connectionName):base()
        {
            this.ConnectionName = connectionName;
        }

       
        private string dataSource;
        [XmlElement(DataType = "string")]
        public string DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        
        private string password;
        [XmlElement(DataType = "string")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        
        private bool usePre33xFormat;
        [System.ComponentModel.DefaultValueAttribute(true)]
        [XmlElement(DataType = "boolean")]
        public bool UsePre33xFormat
        {
            get { return usePre33xFormat; }
            set { usePre33xFormat = value; }
        }


        private bool readOnly;
        [XmlElement(DataType = "boolean")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        
        private bool useUTF16;
        [XmlElement(DataType = "boolean")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseUTF16
        {
            get { return useUTF16; }
            set { useUTF16 = value; }
        }

        
        private bool isNew;
        [XmlIgnore]  
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }
    }

}
