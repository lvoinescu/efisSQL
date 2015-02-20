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
using System.Configuration;
using System.Security;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
namespace efisSQL
{





    [XmlRootAttribute("ConfigurationManager", Namespace = "", IsNullable = false)]
    public class ConfigurationManager
    {
        [XmlArray("Connections"), XmlArrayItem("Connection",typeof(DBMS.core.ConnectionSetting))]
        public System.Collections.ArrayList Connections = new System.Collections.ArrayList();


        public ConfigurationManager()
        {
        }


    }
}
