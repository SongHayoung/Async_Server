using System;
using System.Net.Sockets;
using System.Threading;
using GTK_Demo_Packet;
namespace GTK_Server.Handler
{
    public class CDataHandler
    {
        public CDataHandler() { }
        /*
         * Running DataHandler
         */
        private static System.Timers.Timer HeartBeatTimer = new System.Timers.Timer();
        public static void Run()
        {
            Console.WriteLine("Handling Manager on Active");
            Thread.CurrentThread.Name = "DHThread";
            Handling();
            Console.WriteLine("Handling Manager Join");
        }

        /*
         * this function helps moving Recv Sessions to right Queue at PacketFactory
         */
        private static void Handling()
        {
            CDataFactory DataFactory = CDataFactory.GetDataFactory();
           
            HeartBeatTimer.Interval = 5000;
            HeartBeatTimer.Elapsed += new System.Timers.ElapsedEventHandler(Handling_HeartBeat);
            HeartBeatTimer.Start();
            while (Program.IsRunning())
            {
                CNetworkSession Session = DataFactory.GetRecvBuffer();
                if (Session == null) continue;
                HM_log("Working");
                if (Session._packettype == PacketType.Login || Session._packettype == PacketType.Member_REGISTER)
                {
                    DataFactory.SetDatabseBuffer(Session);
                }
                else if(Session._packettype == PacketType.Heart_Beat)
                {
                    HM_log("REVEICE HEART BEAT FROM " + ((HeartBeat)Packet.Deserialize(Session._buffer)).id_str);
                    DataFactory.setHeartBeat(((HeartBeat)Packet.Deserialize(Session._buffer)).id_str);
                }
            }
        }

        public static void Handling_RecvPacket(Socket socket, byte[] buffer){
            CNetworkSession RecvSession = new CNetworkSession(socket, buffer);
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            if(RecvSession._packettype == PacketType.Login){
                if (!PacketFactory.isLogined(RecvSession)){
                    PacketFactory.SetRecvBuffer(RecvSession);
                }
                else{
                    LoginResult loginResult = new LoginResult();
                    loginResult.msg = "이미 접속이 되어있는 계정입니다";
                    loginResult.result = false;
                    buffer = Packet.Serialize(loginResult);
                    CNetworkSession failSession = new CNetworkSession(socket, buffer, PacketType.Login_RESULT);
                    PacketFactory.SetSendBuffer(failSession);
                }
            }
            else
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
            if(!PacketFactory.SetSendBuffer(Session))
            {
                HM_log("SetSendBuffer Error");
            }
        }

        public static void Handling_ResultDBData(Socket s, byte[] buffer, PacketType packettype)
        {
            CDataFactory PacketFactory = CDataFactory.GetDataFactory();
            CNetworkSession Session = new CNetworkSession(s, buffer, packettype);
            if (buffer.Length==0){
                HM_log("No Data in Result Buffer From DataBase");
                return;
            }
            if (!PacketFactory.SetSendBuffer(Session)){
                HM_log("SetSendBuffer Error");
                return;
            }
        }

        public static void Handling_ResultDBData(Socket s, byte[] buffer, PacketType packettype, string id)
        {
            CNetworkSession Session = new CNetworkSession(s, buffer, packettype);
            if (packettype == PacketType.Login_RESULT){
                LoginResult result = (LoginResult)Packet.Deserialize(buffer);
                if (result.result)
                {
                    CDataFactory PacketFactory = CDataFactory.GetDataFactory();
                    PacketFactory.SetClients(Session, id);
                }
            }
            Handling_ResultDBData(Session);
        }

        private static void Handling_HeartBeat(object sender, System.Timers.ElapsedEventArgs e)
        {
            HM_log("Heart BEAT");
            CDataFactory DataFactory = CDataFactory.GetDataFactory();
            DataFactory.doHeartBeat();
        }

        private static void HM_log(string str)
        {
            Console.WriteLine("Handling Manager : " + str);
        }
    }
}
