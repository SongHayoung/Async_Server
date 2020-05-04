using System;
using System.Collections.Generic;
using System.Net.Sockets;

using GTK_Demo_Packet;
namespace GTK_Server.Handler
{
    /*
     * this class contain Sessions for each Manager needs
     */
    public class CDataFactory
    {
        private static CDataFactory DataFactory = new CDataFactory();
        private Queue<CNetworkSession> Recv_buffer = new Queue<CNetworkSession>();
        private Queue<CNetworkSession> Send_buffer = new Queue<CNetworkSession>();
        private Queue<CNetworkSession> Database_Buffer = new Queue<CNetworkSession>();
        private static Dictionary<string, CNetworkSession> Clients = new Dictionary<string, CNetworkSession>();
        private static object Recv_Lock = new object();
        private static object Send_Lock = new object();
        private static object Database_Lock = new object();
        private static object Clients_Lock = new object();
        private const int BUF_SIZE = 1024 * 4;

        private CDataFactory() { }

        /*
         * Return PacketFactory
         */
        public static CDataFactory GetDataFactory(){
            return DataFactory;
        }

        /*
         * set Session at receive buffer
         */
        public bool SetRecvBuffer(Socket socket, byte[] buffer)
        {
            CNetworkSession Recv = new CNetworkSession(socket, buffer);
            lock (Recv_Lock)
            {
                Recv_buffer.Enqueue(Recv);
            }
            return true;
        }

        public bool SetRecvBuffer(CNetworkSession Session)
        {
            lock(Recv_Lock)
            {
                Recv_buffer.Enqueue(Session);
            }
            return true;
        }


        /*
         * get Session from receive buffer
         */
        public CNetworkSession GetRecvBuffer()
        {
            CNetworkSession Session;
            lock (Recv_Lock)
            {
                if (Recv_buffer.Count>0)
                {
                    Session = Recv_buffer.Dequeue();
                }
                else
                {
                    Session = null;
                }
            }
            return Session;
        }

        /*
         * set Session at send buffer
         */
        public bool SetSendBuffer(CNetworkSession Session)
        {
            lock(Send_Lock)
            {
                Send_buffer.Enqueue(Session);
            }
            return true;
        }

        /*
         * get Session from send buffer
         */
        public CNetworkSession GetSendBuffer()
        {
            CNetworkSession Session;
            lock (Send_Lock)
            {
                if (Send_buffer.Count > 0)
                {
                    Session = Send_buffer.Dequeue();
                }
                else{
                    Session = null;
                }
            }
            return Session;
        }

        /*
         * set Session at database buffer
         */
        public bool SetDatabseBuffer(CNetworkSession Session)
        {
            lock(Database_Lock)
            {
                Database_Buffer.Enqueue(Session);
            }
            return true;
        }

        /*
         * get Session from database buffer
         */
        public CNetworkSession GetDatabaseBuffer() {
            CNetworkSession Session;
            lock(Database_Lock){
                if(Database_Buffer.Count>0) {
                    Session = Database_Buffer.Dequeue();
                }
                else {
                    Session = null;
                }
            }
            return Session;
        }

        public void SetClients(CNetworkSession Session, string id){
            lock (Clients_Lock)
            {
                Clients.Add(id, new CNetworkSession(Session._socket, Session._buffer));
                Console.WriteLine("Added " + id);
            }
        }

        public bool isLogined(CNetworkSession Session){
            string id = ((Login)Packet.Deserialize(Session._buffer)).id_str;
            bool ret = false;
            lock (Clients_Lock)
                ret = Clients.ContainsKey(id);
            return ret;
        }

        public void doHeartBeat() {
            byte[] buffer;
            HeartBeat heartBeat = new HeartBeat();
            DateTime Starttime = DateTime.Now;
            TimeSpan Limit = new TimeSpan(0, 10, 0);
            lock(Clients_Lock)
            foreach(KeyValuePair<string,CNetworkSession> sessions in Clients) {
                if (sessions.Value._socket.Connected){
                    TimeSpan diff = Starttime - sessions.Value._datetime;
                    if (TimeSpan.Compare(Limit,diff)==-1) {
                        sessions.Value._socket.Close();
                        Clients.Remove(sessions.Key);
                    }
                    else {
                        heartBeat.id_str = sessions.Key;
                        buffer = Packet.Serialize(heartBeat);
                        if (!SetSendBuffer(new CNetworkSession(sessions.Value._socket, buffer, PacketType.Heart_Beat)))
                            Console.WriteLine("DATA FACTORY : Heart Beat Error\n");
                        else
                            Console.WriteLine("DATA FACTORY : HeartBeat Sending " + sessions.Key + " " + sessions.Value._datetime.ToString("yyyy/MM/dd hh:mm:ss"));
                    }
                }
                else{
                    sessions.Value._socket.Close();
                    Clients.Remove(sessions.Key);
                }
            }
        }
        public bool setHeartBeat(string id) {
            Clients[id]._datetime = DateTime.Now;
            Console.WriteLine("Update Heart Beat time " + id + " " + Clients[id]._datetime.ToString("yyyy/MM/dd hh:mm:ss"));
            return true;
        }
    }
}
