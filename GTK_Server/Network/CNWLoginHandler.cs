using System;
using System.Net;
using System.Net.Sockets;

using GTK_Demo_Packet;
using GTK_Server.Database;

namespace GTK_Server.Network
{
    public class CNWLoginHandler
    {
        private Login login;
        private LoginResult loginresult;
        public CNWLoginHandler(Login lg)
        {
            login = new Login();
            loginresult = new LoginResult();
            login = lg;

            Start();
        }

        private void Start()
        {
            CDBLoginManager lgM = new CDBLoginManager(login);
            loginresult = lgM.GetResult();
        }

        public byte[] GetResultAsByte()
        {
            return Packet.Serialize(loginresult);
        }

        public LoginResult GetResultAsLoginResult()
        {
            return loginresult;
        }

    }
}
