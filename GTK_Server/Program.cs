using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using GTK_Server.Network;
using GTK_Server.Handler;
using GTK_Server.Database;

namespace GTK_Server
{
    class Program
    {
        private static bool Running = true;

        static void Main(string[] args)
        {
            Thread NWManager = new Thread(new ThreadStart(CNWManager.Run));
            Thread DBManager = new Thread(new ThreadStart(CDBManager.Run));
            Thread HandlingManager = new Thread(new ThreadStart(CDataHandler.Run));

            NWManager.Start();
            DBManager.Start();
            HandlingManager.Start();

            string cmd;
            while (IsRunning())
            {
                cmd = Console.ReadLine();
                if (cmd.CompareTo("QUIT") == 0)
                    Running = false;
            }

            NWManager.Join();
            DBManager.Join();
            HandlingManager.Join();
        }

        public static bool IsRunning()
        {
            return Running;
        }
    }
}
