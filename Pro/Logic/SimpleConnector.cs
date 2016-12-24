using System;
using MySql.Data.MySqlClient;

namespace Pro.Logic
{
    public class SimpleConnector
    {
        private static MySqlConnection connection = null;

        public static MySqlConnection Connection
        {
            get { return connection; }
        }

        public static void Connect(String server = "localhost", String userId = "root", String password = "1111", String dbName = "test_db")
        {
            if (connection == null)
            {
                String connectionString = String.Format(
                    @"server={0};userid={1};password={2};database={3}",
                    server,
                    userId,
                    password,
                    dbName
                );
                SimpleConnector.connection = new MySqlConnection(connectionString);
            }
        }

        public static void Close()
        {
            SimpleConnector.connection.Dispose();
        }
    }
}