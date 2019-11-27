using System;
using System.IO;
using MySql.Data.MySqlClient;

using GTK_Demo_Packet;

namespace GTK_Server.Database
{
    public class CDBMemberRegisterManager : CDBManager
    {
        private MemberRegister NewMember;
        private MemberRegisterResult Result;

        private new const string DBInfo = "Server=127.0.0.1;Database=DemoDB;Uid=root;Pwd=admin;";

        public CDBMemberRegisterManager(MemberRegister NewMember) : base(DBInfo)
        {
            NewMember = this.NewMember;
        }

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
                Result.result = false;
                return false;
            }

            s = new string("INSERT INTO USER(ID,Pass)" + "VALUES(@ID,@Pass);");
            cmd = new MySqlCommand(s, DB_conn);
            cmd.Parameters.AddWithValue("@ID", this.NewMember.id_str);
            cmd.Parameters.AddWithValue("@Pass", this.NewMember.pw_str);
            cmd.ExecuteNonQuery();

            dr.Close();
            DB_conn.Close();
            return true;
        }

        public MemberRegisterResult GetResult()
        {
            if (RegistMember(NewMember.id_str, NewMember.pw_str))
            {
                DM_setLog("New User Registered");
                Result.msg = new string("성공");
                Result.result = true;
            }
               
            return Result;
        }
    }
}
