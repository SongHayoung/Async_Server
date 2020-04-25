using System;
using MySql.Data.MySqlClient;

using GTK_Server.Handler;
using GTK_Demo_Packet;
namespace GTK_Server.Database
{
    /*
     * this class helps managing database
     */
    public class CDBManager{
        private static string DBInfo = "Server=127.0.0.1;Uid=root;Pwd=admin;";
        private const int Buf_Size = 1024 * 4;
        protected static MySqlConnection DB_conn;

        public CDBManager() { }

        public CDBManager(string dbinfo)
        {
            DB_conn = new MySqlConnection(dbinfo);
        }

        /*
         * Running Database Manager
         */
        public static void Run()
        {
            Console.WriteLine("Database Manager on Active");
            DB_conn = new MySqlConnection(DBInfo);
            DB_Setting();
            Handling();
            Console.WriteLine("Database Manager Join");
        }

        /*
         * this function calls profit manager to set and get data from Databse
         */
        public static void Handling()
        {
            while (Program.IsRunning())
            {
                CNetworkSession Session = CDataHandler.Handling_GetDBData();
                if (Session == null)
                    continue;
                DM_setLog("Working");
                CNetworkSession Result = new CNetworkSession();
                byte[] buffer;
                Result._socket = Session._socket;

                if (Session._packettype == PacketType.Login)
                {
                    CDBLoginManager lgM = new CDBLoginManager(Session._buffer);
                    buffer = lgM.GetResultByByte();
                    CDataHandler.Handling_ResultDBData(Result._socket, buffer, PacketType.Login_RESULT);
                }
                if (Session._packettype == PacketType.Member_REGISTER)
                {
                    CDBMemberRegisterManager rgM = new CDBMemberRegisterManager(Session._buffer);
                    buffer = rgM.GetResultByByte();
                    CDataHandler.Handling_ResultDBData(Result._socket, buffer, PacketType.Member_REGISTER_RESULT);
                }
            }
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
         * Initialize Database
         */
        private static void DB_Setting()
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
