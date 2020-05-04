using System;
using MySql.Data.MySqlClient;

using GTK_Demo_Packet;

namespace GTK_Server.Database
{
    /*
     * this class helping user for login
     */
    public class CDBLoginManager : CDBDao
    {
        private Login User;
        private LoginResult Result { get; set;}
        
        public CDBLoginManager(Login User)
        {
            this.User = User;
            Result = new LoginResult();
            Result.packet_Type = PacketType.Login_RESULT;
        }

        public CDBLoginManager(byte[] User)
        {
            this.User = (Login)Packet.Deserialize(User);
            Result = new LoginResult();
            Result.packet_Type = PacketType.Login_RESULT;
        }

        /*
         * this function initalize LoginResult member 
         */
        private void SetResult(){
            Result.result = invalidIDorPass(User.id_str, User.pw_str);
            Result.msg = Result.result ? "로그인 성공" : "아이디나 비밀번호가 맞지 않습니다";
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
        public byte[] GetResultByByte(){
            SetResult();
            return Packet.Serialize(Result);
        }

        public string getID(){
            return User.id_str;
        }
    }
}
