using System;
using MySql.Data.MySqlClient;

namespace GTK_Server.Database
{
    public class CDBConnFactory
    {
        public CDBManager cDBManager()
        {
            return new CDBManager(connection());
        }
        public IDBConnection connection()
        {
            return new CDBConnection();
        }
    }
}
