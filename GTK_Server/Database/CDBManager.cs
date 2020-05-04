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
        private const int Buf_Size = 1024 * 4;

        /*
         * Running Database Manager
         */
        public static void Run()
        {
            Console.WriteLine("Database Manager on Active");
            DB_Setting();
            Handling();
            Console.WriteLine("Database Manager Join");
        }

        /*
         * this function calls profit manager to set and get data from Databse
         */
        public static void Handling()
        {
            CNetworkSession Session = null;
            while (Program.IsRunning())
            {
                Session = CDataHandler.Handling_GetDBData();
                if (Session == null)
                    continue;
                DM_setLog("Working");
                if (Session._packettype == PacketType.Login)
                {
                    CDBLoginManager lgM = new CDBLoginManager(Session._buffer);
                    Login lg = (Login)Packet.Deserialize(Session._buffer);
                    CDataHandler.Handling_ResultDBData(Session._socket, lgM.GetResultByByte(), PacketType.Login_RESULT, lg.id_str);
                }
                if (Session._packettype == PacketType.Member_REGISTER)
                {
                    CDBMemberRegisterManager rgM = new CDBMemberRegisterManager(Session._buffer);
                    CDataHandler.Handling_ResultDBData(Session._socket, rgM.GetResultByByte(), PacketType.Member_REGISTER_RESULT);
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
            MySqlConnection DB = new MySqlConnection("Server=127.0.0.1;Uid=root;Pwd=admin;");
            try
            {
                DB.Open();
                DM_setLog("Checking Database Exists");

                string s;
                s = new string("SHOW DATABASES LIKE 'DemoDB';");

                MySqlCommand cmd = new MySqlCommand(s, DB);
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
                    cmd = new MySqlCommand(s, DB);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    s = new string("USE DemoDB;");
                    cmd = new MySqlCommand(s, DB);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd = new MySqlCommand(s, DB);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    s = new string("CREATE TABLE USER(ID VARCHAR(255) NOT NULL PRIMARY KEY, Pass VARCHAR(255) NOT NULL); ");
                    cmd = new MySqlCommand(s, DB);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    s = new string("CREATE TABLE USER_MANAGEMENT (ID VARCHAR(255) NOT NULL,IP VARCHAR(255) NOT NULL,PORT VARCHAR(255) NOT NULL);");
                    cmd = new MySqlCommand(s, DB);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    DB.Close();
                    DM_setLog("Database Created");
                }
                else
                {
                    DM_setLog("Database Exists");
                    DB.Close();
                }
            }
            catch (Exception er)
            {
                DM_setLog(er.ToString());
            }
        }
    }
}
