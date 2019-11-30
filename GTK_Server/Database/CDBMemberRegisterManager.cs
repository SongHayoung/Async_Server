using System;
using MySql.Data.MySqlClient;

using GTK_Demo_Packet;

namespace GTK_Server.Database
{
    /*
     * this class helping user for register
     */
    public class CDBMemberRegisterManager : CDBManager
    {
        private MemberRegister NewMember;
        private MemberRegisterResult Result;

        private const string DBInfo = "Server=127.0.0.1;Database=DemoDB;Uid=root;Pwd=admin;";

        public CDBMemberRegisterManager(MemberRegister NewMember) : base(DBInfo)
        {
            NewMember = this.NewMember;
            Result = new MemberRegisterResult();
        }

        public CDBMemberRegisterManager(byte[] NewMember) : base(DBInfo)
        {
            this.NewMember = (MemberRegister)Packet.Deserialize(NewMember);
            Result = new MemberRegisterResult();
        }

        /*
         * this function helps registing new member at database
         */
        private bool RegistMember(string ID, string Pass)
        {
            MySqlCommand cmd;

            DB_conn = new MySqlConnection(DBInfo);
            DB_conn.Open();

            string s = "SELECT * FROM USER WHERE ID = @ID;";
            cmd = new MySqlCommand(s, DB_conn);
            cmd.Parameters.AddWithValue("@ID", ID);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                dr.Close();
                DB_conn.Close();
                Result.msg = new string("이미 존재하는 아이디 입니다.");
                Result.packet_Type = PacketType.Member_REGISTER_RESULT;
                Result.result = false;
                return false;
            }
            dr.Close();
            s = new string("INSERT INTO USER(ID,Pass)" + "VALUES(@ID,@Pass);");
            cmd = new MySqlCommand(s, DB_conn);
            cmd.Parameters.AddWithValue("@ID", this.NewMember.id_str);
            cmd.Parameters.AddWithValue("@Pass", this.NewMember.pw_str);
            cmd.ExecuteNonQuery();

            DB_conn.Close();
            return true;
        }

        /*
         * this function initalizing MemberRegisterResult member
         */
        private void SetResult()
        {
            if (RegistMember(NewMember.id_str, NewMember.pw_str))
            {
                DM_setLog("New User Registered");
                Result.msg = new string("회원 가입 성공");
                Result.packet_Type = PacketType.Member_REGISTER_RESULT;
                Result.result = true;
            }
        }

        /*
         * this function return MemberRegisterResult member as MemberRegisterResult
         */
        public MemberRegisterResult GetResultByRegisterResult()
        {
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

    }
}
