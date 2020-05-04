using System;
using MySql.Data.MySqlClient;
namespace GTK_Server.Database
{
    public class CDBDao
    {
        public IDBConnection DB_conn;
        public CDBDao(){
            DB_conn = new CDBConnection();
        }
        protected static void DM_setLog(string str1)
        {
            Console.WriteLine("Database Manager : " + str1);
        }
        protected static void DM_setLog(string str1, string str2)
        {
            Console.WriteLine("Database Manager : " + str1 + str2);
        }

    }
}
