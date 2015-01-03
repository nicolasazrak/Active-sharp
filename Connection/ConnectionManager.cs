using ActiveSharp.Connection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ActiveSharp.Connection
{

    class ConnectionManager
    {

        private static ConnectionManager instance;
        public static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectionManager();
                }
                return instance;
            }
        }

        private SqlConnection conexion;

        private ConnectionManager() { }


        public void connect(String conectionString)
        {
            conexion = new SqlConnection(conectionString);
            conexion.Open();

            new SqlCommand("USE [" + Config.Instance.database + "] ", conexion).ExecuteNonQuery();
        }


        public SqlConnection getConnection()
        {
            return conexion;
        }

        public void close()
        {
            conexion.Close();
        }



    }
}
