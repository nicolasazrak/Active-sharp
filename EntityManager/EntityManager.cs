using ActiveSharp.Connection;
using ActiveSharp.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ActiveSharp.Database_Helper
{
    class EntityManager
    {


        private static EntityManager instance;
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityManager();
                }
                return instance;
            }
        }

        private EntityManager(){ }


        public PropertyInfo[] getProperties(Type clazz)
        {
            return clazz.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => {
                //Esto evita que se devuelva como propiedad las que sean listas y la propiedad del nombre de la tabla
                return prop.PropertyType != typeof(List<>) && prop.Name != "table" ;
            }).ToArray();
        }

        public String getTableName(Type clazz)
        {
            Object instancia = Activator.CreateInstance(clazz);
            return clazz.GetProperty("table").GetGetMethod().Invoke(instancia, null).ToString();
        }

        public String tableWithSchemaName(Type clazz)
        {
            return "";
        }


        private Dictionary<String, String> getProperties(ActiveRecord item)
        {
            Dictionary<String, String> properties = new Dictionary<string, string>();
           
            foreach (PropertyInfo property in getProperties(item.GetType()))
            {

                if (property.PropertyType == typeof(List<>))
                {
                    //Si hay un 1 a muchos no deberia guardarlo
                }
                else if (property.Name == "table")
                {
                    //Ignorar
                }
                else if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(Decimal) || property.PropertyType == typeof(String))
                {
                    try
                    {
                        properties.Add(property.Name, property.GetGetMethod().Invoke(item, null).ToString());
                    }
                    catch (NullReferenceException e)
                    {

                    }
                    
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    //TODO
                    DateTime fecha = (DateTime) property.GetGetMethod().Invoke(item, null);
                    properties.Add(property.Name, fecha.ToShortDateString());
                }
                else
                {
                    //Aca obtengo el id del objeto relacionado
                    MethodInfo getPropAnidada =  property.GetGetMethod();                               // Obtengo el getter de la propiedad
                    Object objetoAnidado = getPropAnidada.Invoke(item, null);                           // Obtengo el valor (el otro objeto)

                    //Capaz el objeto anidado no esta seado y es null, probablemente haya un problema con las FK
                    if (objetoAnidado != null)
                    {
                        Type claseAnidada = objetoAnidado.GetType();                                        // Consigo la clase del objeto
                        MethodInfo getIdAnidado = claseAnidada.GetProperty("id").GetGetMethod();            // Consigo el getter del id del objeto
                        properties.Add(property.Name + "_id", getIdAnidado.Invoke(objetoAnidado, null).ToString()); //Consigo el id y lo guardo en el diccionario
                    }
                    
                }

            }
            return properties;
        }



        public void save(ActiveRecord item)
        {
            item.preSave();

            if (item.id != 0)
                this.update(item);
            else
                item.id = this.insert(item);

            item.afterSave();
        }

        public int insert(ActiveRecord item)
        {
            item.preInsert();
            InsertQuery query = new InsertQuery(item.GetType());
            foreach (KeyValuePair<string, string> properties in getProperties(item))
            {
                query.addKeyValue(properties.Key, properties.Value);
            }
            return query.exec();
        }


        public int update(ActiveRecord item)
        {
            UpdateQuery query = new UpdateQuery(item.GetType());
            foreach (KeyValuePair<string, string> properties in getProperties(item))
            {
                query.addKeyValue(properties.Key, properties.Value);
            }
            query.addWhere("id", item.id.ToString());
            Console.WriteLine(query.build());
            return query.exec();
        }

        public void delete(ActiveRecord item)
        {
            DeleteQuery query = new DeleteQuery(item.GetType());
            query.addWhere("id", item.id.ToString());
            Console.WriteLine(query.build());
            query.exec();
        }

        public void logicalDelete(ActiveRecord item)
        {
            UpdateQuery query = new UpdateQuery(item.GetType());
            query.addKeyValue("estado", "0");

            query.addWhere("id", item.id.ToString());
            Console.WriteLine(query.build());
            query.exec();
        }






        

        private SelectQuery<T> buildQuery<T>(List<FetchCondition> conditions)
        {

            SelectQuery<T> query = new SelectQuery<T>(typeof(T));

            query.addWhere(conditions);

            String tableName = getTableName(typeof(T));

            foreach (PropertyInfo property in getProperties(typeof(T)))
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(Decimal) || property.PropertyType == typeof(String))
                {
                    query.addSelect(tableName + "." + property.Name, tableName + "_" + property.Name);
                } 
                else if(property.PropertyType.IsSubclassOf(typeof(ActiveRecord)))
                {

                    /* Este es el caso donde hace el join a la propiedad */
                    String subTableName = getTableName(property.PropertyType);
                    foreach (PropertyInfo subProperty in getProperties(property.PropertyType))
                    {
                        /* Agrega los selects */
                        if (subProperty.PropertyType.IsPrimitive || subProperty.PropertyType == typeof(Decimal) || subProperty.PropertyType == typeof(String))
                        {
                            query.addSelect(subTableName + "." + subProperty.Name, subTableName + "_" + subProperty.Name);
                        } 
                    }

                    /* Agrega el join */
                    query.addLeftJoin(subTableName, tableName + "." + property.Name + "_id" + " = " + subTableName + ".id");
                }
            }

            return query;

        }


        public List<T> findList<T>(List<FetchCondition> conditions)
        {

            SelectQuery<T> query = EntityManager.Instance.buildQuery<T>(conditions);

            SqlCommand cmd = new SqlCommand(query.build(), ConnectionManager.Instance.getConnection());

            List<T> lista = new List<T>();

            //DEBUG
            //MessageBox.Show(query.build());

            using (SqlDataReader result = cmd.ExecuteReader())
            {
                while (result.Read())
                {
                    lista.Add(FillerHelper.buildObject<T>(result));
                }
            }

            return lista;
        }

        public List<T> findAll<T>()
        {
            return findList<T>(new List<FetchCondition>());
        }

        public List<T> findAllBy<T>(String key, String value)
        {
            FetchCondition condition = new FetchCondition();
            condition.setEquals(key, value);
            List<FetchCondition> condiciones = new List<FetchCondition>();
            condiciones.Add(condition);
            return findList<T>(condiciones);
        }

        public T findBy<T>(String key, String value)
        {
            List<T> lista =findAllBy<T>(key, value);
            if(lista.Count()==0)
            {
                return default(T);
            }
            return lista[0];
        }

        public T findById<T>(long id)
        {
            return findBy<T>(getTableName(typeof(T)) + ".id", id.ToString());
        }




    }
}
