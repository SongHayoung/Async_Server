using System;
using MySql.Data.MySqlClient;

namespace GTK_Server.Database
{
    public class CDBConnection : IDBConnection
    {
        public MySqlConnection makeConnection()
        {
            MySqlConnection DB_conn = new MySqlConnection("Server=127.0.0.1;Database=DemoDB;Uid=root;Pwd=admin;");
            return DB_conn;
        }
    }
}
