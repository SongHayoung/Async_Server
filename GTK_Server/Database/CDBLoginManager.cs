using System;
using MySql.Data.MySqlClient;

using GTK_Demo_Packet;

namespace GTK_Server.Database
{
    /*
     * this class helping user for login
     */
    public class CDBLoginManager : CDBManager
    {
        private Login User;
        private LoginResult Result { get; set;}

        private static string DBInfo = "Server=127.0.0.1;Database=DemoDB;Uid=root;Pwd=admin;";

        public CDBLoginManager(Login User) : base(DBInfo)
        {
            this.User = User;
            Result = new LoginResult();
        }

        public CDBLoginManager(byte[] User) : base(DBInfo)
        {
            this.User = (Login)Packet.Deserialize(User);
            Result = new LoginResult();
        }

        /*
         * this function checking invalid ID or Password
         */
        private bool invalidIDorPass(string ID, string Pass)
        {
            MySqlCommand cmd;

            DB_conn = new MySqlConnection(DBInfo);
            DB_conn.Open();

            string s = "SELECT * FROM USER WHERE ID = @ID;";
            cmd = new MySqlCommand(s, DB_conn);
            cmd.Parameters.AddWithValue("@ID", ID);

            MySqlDataReader dr = cmd.ExecuteReader();

            if(!dr.Read())
            {
                dr.Close();
                DB_conn.Close();
                return false;
            }
            if(Pass.CompareTo(dr[1].ToString())!=0)
            {
                dr.Close();
                DB_conn.Close();
                return false;
            }

            dr.Close();
            DB_conn.Close();
            return true;
        }

        /*
         * this function check user already logined
         */
        private void DuplicatedLogin()
        {
            try
            {
                MySqlCommand cmd;

                DB_conn = new MySqlConnection(DBInfo);
                DB_conn.Open();

                string s = "INSERT INTO USER_MANAGEMENT(ID,IP,PORT)" + "VALUES(@ID,@IP,@PORT);";
                cmd = new MySqlCommand(s, DB_conn);
                cmd.Parameters.AddWithValue("@ID", this.User.id_str);
                cmd.Parameters.AddWithValue("@IP", this.User.ip_str);
                cmd.Parameters.AddWithValue("@PORT", this.User.port_str);
                cmd.ExecuteNonQuery();

                DB_conn.Close();
                DM_setLog("User logined ", this.User.id_str);
                Result.result = true;
                Result.packet_Type = PacketType.Login_RESULT;
                Result.msg = "로그인 성공";
                return ;
            }
            catch(Exception e)
            {
                DM_setLog(e.ToString());
                DB_conn.Close();
                Result.result = false;
                Result.packet_Type = PacketType.Login_RESULT;
                Result.msg = "중복된 접속입니다";
                return ;
            }
        }

        /*
         * this function initalize LoginResult member 
         */
        private void SetResult()
        {
            if (!invalidIDorPass(User.id_str, User.pw_str))
            {
                Result.result = false;
                Result.packet_Type = PacketType.Login_RESULT;
                Result.msg = "아이디나 비밀번호가 맞지 않습니다";
                return;
            }
            DuplicatedLogin();
        }

        /*
         * this function returns LoginResult member as LoginResult
         */
        public LoginResult GetResultByLoginResult()
        {
            SetResult();
            return Result;
        }

        /*
         * this function return LoginResult member as byte array
         */
        public byte[] GetResultByByte()
        {
            SetResult();
            return Packet.Serialize(Result);
        }
    }
}
