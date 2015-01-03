using ActiveSharp.Database_Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ActiveSharp.Connection;

namespace ActiveSharp.Query
{
    abstract class Query
    {

        public static List<String> log;

        public static void addLog(String query)
        {
            if (Query.log == null)
            {
                log = new List<String>();
            }

            log.Add(query);
        }

        //TODO cosificar para poder agregar condicions con OR
        public List<String> whereConditions;

        public Type clazz;


        public Query(Type clazz)
        {
            this.whereConditions = new List<string>();
            this.clazz = clazz;
        }



        public String getTableName()
        {
            return "[" + Config.Instance.schema + "].[" + EntityManager.Instance.getTableName(clazz) + "]";
        }


        public Query addWhere(string key, ActiveRecord value)
        {
            if (value != null && key != null)
                this.addWhere(key, value.id.ToString(), "=");
            return this;
        }

        public Query addWhere(string key, string value)
        {
            if(value != null && key != null)
                this.addWhere(key, value, "=");
            return this;
        }

        public Query addWhere(string key, List<string> value)
        {
            this.addWhere(key, string.Join(", ", value.ToArray()), " IN ");
            return this;
        }

        public Query addWhere(string key, string value, string comparator)
        {
            this.whereConditions.Add(key + " " + comparator + " " + value);
            return this;
        }

        public Query addWhere(string condition)
        {
            this.whereConditions.Add(condition);
            return this;
        }

        
        public void addWhere(List<FetchCondition> conditions)
        {
            foreach (FetchCondition condition in conditions)
            {
                this.addWhere(condition.build());
            }
        }


        protected string buildWhere()
        {
            if (whereConditions.Count == 0)
            {
                return "";
            }
            return " WHERE " + string.Join(" AND ", whereConditions.ToArray());
        }


        public abstract String build();

    }
}
