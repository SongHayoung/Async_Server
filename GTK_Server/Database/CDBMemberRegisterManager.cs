using System;
using MySql.Data.MySqlClient;

using GTK_Demo_Packet;

namespace GTK_Server.Database
{
    /*
     * this class helping user for register
     */
    public class CDBMemberRegisterManager : CDBDao
    {
        private MemberRegister NewMember;
        private MemberRegisterResult Result;

        public CDBMemberRegisterManager(MemberRegister NewMember) : base()
        {
            NewMember = this.NewMember;
            Result = new MemberRegisterResult();
        }

        public CDBMemberRegisterManager(byte[] NewMember) : base()
        {
            this.NewMember = (MemberRegister)Packet.Deserialize(NewMember);
            Result = new MemberRegisterResult();
        }

        /*
         * this function helps registing new member at database
         */
        private bool RegistMember(string ID, string Pass)
        {
            if (ID == "" || Pass == ""){
                setResultMsg("계정을 입력하세요.", false);
                return false;
            }
            if (invalidIDorPass(ID, Pass)){
                setResultMsg("이미 존재하는 아이디 입니다.", false);
                return false;
            }
            MySqlCommand cmd;
            MySqlConnection DB = DB_conn.makeConnection();
            DB.Open();
            string s = new string("INSERT INTO USER(ID,Pass)" + "VALUES(@ID,@Pass);");
            cmd = new MySqlCommand(s, DB);
            cmd.Parameters.AddWithValue("@ID", this.NewMember.id_str);
            cmd.Parameters.AddWithValue("@Pass", this.NewMember.pw_str);
            cmd.ExecuteNonQuery();

            DB.Close();
            return true;
        }

        /*
         * this function initalizing MemberRegisterResult member
         */
        private void SetResult()
        {
            if (RegistMember(NewMember.id_str, NewMember.pw_str)){
                DM_setLog("New User Registered");
                setResultMsg("회원 가입 성공", true);
            }
        }

        /*
         * this function return MemberRegisterResult member as MemberRegisterResult
         */
        public MemberRegisterResult GetResultByRegisterResult(){
            SetResult();
            return Result;
        }

        /*
         * this function return MemberRegisterResult member as byte array
         */
        public byte[] GetResultByByte()
        {
            SetResult();
            return Packet.Serialize(Result);
        }
        private void setResultMsg(string msg, bool result)
        {
            Result.packet_Type = PacketType.Member_REGISTER_RESULT;
            Result.result = result;
            Result.msg = msg;
        }
    }
}
