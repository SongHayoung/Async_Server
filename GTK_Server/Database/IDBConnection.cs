using System;
using MySql.Data.MySqlClient;
namespace GTK_Server.Database
{
    public interface IDBConnection
    {
        public MySqlConnection makeConnection();
    }
}
