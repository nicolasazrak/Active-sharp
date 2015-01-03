using ActiveSharp.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ActiveSharp.Database_Helper
{
    class FillerHelper
    {


        public static T buildObject<T>(SqlDataReader result)
        {
            T item = (T) Activator.CreateInstance(typeof(T));

            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            String tableName = EntityManager.Instance.getTableName(typeof(T));

            foreach (PropertyInfo property in propertyInfos)
            {

                if (property.Name == "table")
                {
                    //la ignoro
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(item, Int32.Parse(result[tableName + "_" + property.Name].ToString()), null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    property.SetValue(item, Single.Parse(result[tableName + "_" + property.Name].ToString()), null);
                }
                else if (property.PropertyType == typeof(long))
                {
                    property.SetValue(item, Convert.ToInt64(result[tableName + "_" + property.Name].ToString()), null);
                }
                else if (property.PropertyType == typeof(Boolean))
                {
                    property.SetValue(item, Boolean.Parse(result[tableName + "_" + property.Name].ToString()), null);
                }
                else if (property.PropertyType == typeof(String))
                {
                    property.SetValue(item, result[tableName + "_" + property.Name].ToString(), null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    property.SetValue(item, DateTime.Parse(result[tableName + "_" + property.Name].ToString()), null);
                } 
                else if (property.PropertyType.IsSubclassOf(typeof(ActiveRecord)))
                {

                    //Le agrega la nueva instancia
                    Object instancia = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(item, instancia, null);

                    String subTableName = EntityManager.Instance.getTableName(property.PropertyType);

                    /* Como soluciono este copy past???? */
                    foreach (PropertyInfo subProperty in EntityManager.Instance.getProperties(property.PropertyType))
                    {

                        if (subProperty.PropertyType == typeof(int))
                        {
                            subProperty.SetValue(instancia, Int32.Parse(result[subTableName + "_" + subProperty.Name].ToString()), null);
                        }
                        else if (subProperty.PropertyType == typeof(float))
                        {
                            String value = result[subTableName + "_" + subProperty.Name].ToString();
                            subProperty.SetValue(instancia, Single.Parse(value), null);
                        }
                        else if (subProperty.PropertyType == typeof(long))
                        {
                            String value = result[subTableName + "_" + subProperty.Name].ToString();
                            subProperty.SetValue(instancia, Convert.ToInt64(value), null);
                        }
                        else if (subProperty.PropertyType == typeof(Boolean))
                        {
                            subProperty.SetValue(instancia, Boolean.Parse(result[subTableName + "_" + subProperty.Name].ToString()), null);
                        }
                        else if (subProperty.PropertyType == typeof(String))
                        {
                            subProperty.SetValue(instancia, result[subTableName + "_" + subProperty.Name].ToString(), null);
                        }
                        else if (subProperty.PropertyType == typeof(DateTime))
                        {
                            subProperty.SetValue(instancia, DateTime.Parse(result[subTableName + "_" + subProperty.Name].ToString()), null);
                        } 

                    }

                }



            }

            return item;

        }



    }
}
