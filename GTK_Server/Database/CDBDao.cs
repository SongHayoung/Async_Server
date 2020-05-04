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

        /*
         * this function checking invalid ID or Password
         */
        public bool invalidIDorPass(string ID, string Pass)
        {
            MySqlCommand cmd;
            MySqlConnection DB = DB_conn.makeConnection();
            DB.Open();

            string s = "SELECT * FROM USER WHERE ID = @ID;";
            cmd = new MySqlCommand(s, DB);
            cmd.Parameters.AddWithValue("@ID", ID);

            MySqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                dr.Close();
                DB.Close();
                return false;
            }
            if (Pass.CompareTo(dr[1].ToString()) != 0)
            {
                dr.Close();
                DB.Close();
                return false;
            }

            dr.Close();
            DB.Close();
            return true;
        }

    }
}
