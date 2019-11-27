using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace GTK_Server.Database
{
    public class CDBManager
    {
        public string DBInfo = "Server=127.0.0.1;Uid=root;Pwd=admin;";
        public MySqlConnection DB_conn;
        public CDBManager()
        {
            DB_conn = new MySqlConnection(DBInfo);
            DB_Setting();
        }

        public CDBManager(string dbinfo)
        {
            DB_conn = new MySqlConnection(dbinfo);
        }

        public void DM_setLog(string str1)
        {
            Console.WriteLine("Database Manager : {0}", str1);
        }
        public void DM_setLog(string str1, string str2)
        {
            Console.WriteLine("Database Manager : {0} {1}", str1, str2);
        }

        public void DB_Setting()
        {
            try
            {
                DB_conn.Open();
                DM_setLog("Checking Database Exists");

                string s;
                s = new string("SHOW DATABASES LIKE 'DemoDB';");

                MySqlCommand cmd = new MySqlCommand(s, DB_conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                bool db_exsist = true;

                if (!dr.Read())
                {
                    db_exsist = false;
                }
                dr.Close();

                if (!db_exsist)
                {
                    cmd.Parameters.Clear();
                    DM_setLog("Creating New Database");
                    s = new string("CREATE DATABASE DemoDB;");
                    cmd = new MySqlCommand(s, DB_conn);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    s = new string("USE DemoDB;");
                    cmd = new MySqlCommand(s, DB_conn);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd = new MySqlCommand(s, DB_conn);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    s = new string("CREATE TABLE USER(ID VARCHAR(255) NOT NULL, Pass VARCHAR(255) NOT NULL); ");
                    cmd = new MySqlCommand(s, DB_conn);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    s = new string("CREATE TABLE USER_MANAGEMENT (ID VARCHAR(255) NOT NULL,IP VARCHAR(255) NOT NULL,PORT VARCHAR(255) NOT NULL);");
                    cmd = new MySqlCommand(s, DB_conn);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    DB_conn.Close();
                    DM_setLog("Database Created");
                }
                else
                {
                    DM_setLog("Database Exists");
                    DB_conn.Close();
                }
            }
            catch (Exception er)
            {
                DM_setLog(er.ToString());
            }
        }
    }
}
