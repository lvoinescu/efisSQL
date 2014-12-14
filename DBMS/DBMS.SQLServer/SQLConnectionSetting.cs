using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace SqlControls.DBEngine.SQLServer
{
    [XmlInclude(typeof(ConnectionSetting))]
    [XmlRootAttribute("SQLConnection", Namespace = "", IsNullable = false)]
    public class SQLConnectionSetting : ConnectionSetting, IConnectionSetting
    {
        /// <summary>
        /// Default constructor for this class (required for serialization).
        /// </summary>
        public SQLConnectionSetting()
        {
        }
        
        public SQLConnectionSetting(string connectionName):base()
        {
            this.connectionName = connectionName;
        }

        [XmlElement(DataType = "string")]
        private string host;
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        [XmlElement(DataType = "string")]
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }


        [XmlElement(DataType = "string")]
        private string authentication;
        public string Authentication
        {
            get { return authentication; }
            set { authentication = value; }
        }
       
        [XmlElement(DataType = "string")]
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        [XmlElement(DataType = "string")]
        public string type;



    }


}
