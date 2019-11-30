using System;
using System.Net.Sockets;

using GTK_Demo_Packet;
namespace GTK_Server.Handler
{
    public class CDataHandler
    {
        public CDataHandler() { }
        /*
         * Running PacketHandler
         */
        public static void Run()
        {
            Console.WriteLine("Handling Manager on Active");
            Handling();
            Console.WriteLine("Handling Manager Join");
        }

        /*
         * this function helps moving Recv Sessions to right Stack at PacketFactory
         */
        private static void Handling()
        {
            CDataFactory DataFactory = CDataFactory.GetDataFactory();
            while (Program.IsRunning())
            {
                CNetworkSession Session = DataFactory.GetRecvBuffer();
                if (Session == null)
                    continue;

                if (Session._packettype == PacketType.Login || Session._packettype == PacketType.Member_REGISTER)
                {
                    DataFactory.SetDatabseBuffer(Session);
                }
            }
        }

        public static void Handling_RecvPacket(Socket socket, byte[] buffer)
        {
            CNetworkSession RecvSession = new CNetworkSession(socket, buffer);
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            PacketFactory.SetRecvBuffer(RecvSession);
        }

        public static CNetworkSession Handling_SendPacket()
        {
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            return PacketFactory.GetSendBuffer();
        }

        public static CNetworkSession Handling_GetDBData()
        {
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            return PacketFactory.GetDatabaseBuffer();
        }

        public static void Handling_ResultDBData(CNetworkSession Session)
        {
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            if(PacketFactory.SetSendBuffer(Session))
            {
                Console.WriteLine("Packet Handler : SetSendBuffer Error");
            }
        }

        public static void Handling_ResultDBData(Socket s, byte[] buffer, PacketType packettype)
        {
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            CNetworkSession Session = new CNetworkSession(s, buffer, packettype);
            if(buffer.Length==0)
            {
                Console.WriteLine("Packet Handler : No Data in Result Buffer From DataBase");
                return;
            }
            if (PacketFactory.SetSendBuffer(Session))
            {
                Console.WriteLine("Packet Handler : SetSendBuffer Error");
                return;
            }
        }
    }
}
