using System;
using System.Net;
using System.Net.Sockets;

using GTK_Demo_Packet;
namespace GTK_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.CDBManager DBManager = new Database.CDBManager();
            Network.CNWManager NWManager = new Network.CNWManager();
            NWManager.Run();
            while (true)
            {
                
            }
        }
    }
}
