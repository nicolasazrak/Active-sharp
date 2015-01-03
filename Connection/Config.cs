using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ActiveSharp.Connection
{
    public class Config
    {

        private static Config instance;

        public static Config Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = new Config();
                }
                return instance;
            } 
        }


        public String server { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String database { get; set; }
        public String schema { get; set; }

        private Config()
        {
            parseConfig();
        }

        private void parseConfig()
        {

            XmlDocument doc = new XmlDocument(); // Create an XML document object
            doc.Load("../../persistence.xml"); // Load the XML document from the specified file

            XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("properties");

            foreach (XmlNode node in nodes)
            {
                server = node.SelectSingleNode("server").InnerText;
                username = node.SelectSingleNode("username").InnerText;
                password = node.SelectSingleNode("password").InnerText;
                database = node.SelectSingleNode("database").InnerText;
                schema = node.SelectSingleNode("schema").InnerText;
            }


        }


    }
}
