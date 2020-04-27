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

        public CDBLoginManager(Login User, IDBConnection DB_Conn) : base(DB_conn)
        {
            this.User = User;
            Result = new LoginResult();
        }

        public CDBLoginManager(byte[] User, IDBConnection DB_Conn) : base(DB_conn)
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
            MySqlConnection DB = DB_conn.makeConnection();
            DB.Open();

            string s = "SELECT * FROM USER WHERE ID = @ID;";
            cmd = new MySqlCommand(s, DB);
            cmd.Parameters.AddWithValue("@ID", ID);

            MySqlDataReader dr = cmd.ExecuteReader();

            if(!dr.Read())
            {
                dr.Close();
                DB.Close();
                return false;
            }
            if(Pass.CompareTo(dr[1].ToString())!=0)
            {
                dr.Close();
                DB.Close();
                return false;
            }

            dr.Close();
            DB.Close();
            return true;
        }

        /*
         * this function check user already logined
         */
        private void DuplicatedLogin()
        {

            MySqlCommand cmd;
            MySqlConnection DB = DB_conn.makeConnection();
            DB.Open();
            try
            {

                string s = "INSERT INTO USER_MANAGEMENT(ID,IP,PORT)" + "VALUES(@ID,@IP,@PORT);";
                cmd = new MySqlCommand(s, DB);
                cmd.Parameters.AddWithValue("@ID", this.User.id_str);
                cmd.Parameters.AddWithValue("@IP", this.User.ip_str);
                cmd.Parameters.AddWithValue("@PORT", this.User.port_str);
                cmd.ExecuteNonQuery();

                DB.Close();
                DM_setLog("User logined ", this.User.id_str);
                Result.result = true;
                Result.packet_Type = PacketType.Login_RESULT;
                Result.msg = "로그인 성공";
                return ;
            }
            catch(Exception e)
            {
                DM_setLog(e.ToString());
                DB.Close();
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
