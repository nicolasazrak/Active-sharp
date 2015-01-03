using ActiveSharp.Connection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ActiveSharp.Query
{
    class InsertQuery : Query
    {

        private Dictionary<string, string> values;

        public InsertQuery(Type clazz) : base(clazz)
        {
            this.values = new Dictionary<string,string>();
        }


        public void addKeyValue(String key, String value)
        {
            this.addKeyValue(key, value, true);
        }

        public void addKeyValue(String key, String value, Boolean ignoreSpaces)
        {
            if ((value != "" || value.Length > 0) || !ignoreSpaces)
                this.values.Add(key, value);
        }


        public override String build()
        {
            List<String> keys = new List<String>();
            List<String> values = new List<String>();
            foreach(KeyValuePair<String, String> value in this.values){
                if (value.Key != "id")
                {
                    keys.Add(value.Key);
                    //CHECK NULL
                    if (value.Value != "")
                    {
                        //TODO check number or date
                        values.Add("'" + value.Value + "'");
                    }
                    else
                    {
                        values.Add("NULL");
                    }
                        
                }
            }
            String query = "INSERT INTO " + getTableName() + " (" + string.Join(", ", keys.ToArray()) + ") VALUES  (" + string.Join(", ", values.ToArray()) + ") ;";
            Query.addLog(query);
            return query;
        }

        public int exec()
        {
            SqlCommand command = new SqlCommand(build() + "; SELECT @@identity", ConnectionManager.Instance.getConnection());
            //MessageBox.Show(build());
            int newId = System.Convert.ToInt32(command.ExecuteScalar());
            return newId;
        }


    }
}