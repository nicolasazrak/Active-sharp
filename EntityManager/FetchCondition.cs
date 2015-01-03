using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveSharp.Database_Helper
{
    class FetchCondition
    {

        String key, value, comparator;

        public void setEquals(string key, string value)
        {
            this.key = key;
            this.value = "'" + value + "'";
            this.comparator = "=";
        }

        public void setEquals(string key, long value)
        {
            this.key = key;
            this.value = value.ToString();
            this.comparator = "=";
        }

        public void setEqualsWithFunction(string key, string value)
        {
            this.key = key;
            this.value = value;
            this.comparator = "=";
        }

        public void setNotEquals(string key, long value)
        {
            this.key = key;
            this.value = value.ToString();
            this.comparator = "<>";
        }

        public void setLike(string key, string value)
        {
            this.key = key;
            this.value = "'%" + value + "%'";
            this.comparator = "LIKE";
        }

        public void setBetween(string key, string value1, string value2)
        {
            this.key = key;
            this.value = value1.ToString() + " AND " + value2.ToString();
            this.comparator = "BETWEEN";
        }

        public void setNotBetween(string key, string value1, string value2)
        {
            this.key = key;
            this.value = value1.ToString() + " AND " + value2.ToString();
            this.comparator = "NOT BETWEEN";
        }

        public string build()
        {
            return this.key + " " + this.comparator + " " + this.value;
        }



    }
}
