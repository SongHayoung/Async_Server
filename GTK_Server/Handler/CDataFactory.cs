using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using GTK_Demo_Packet;
namespace GTK_Server.Handler
{
    /*
     * this class contain Sessions for each Manager needs
     */
    public class CDataFactory
    {
        private static CDataFactory DataFactory = new CDataFactory();
        private ConcurrentQueue<CNetworkSession> Recv_buffer = new ConcurrentQueue<CNetworkSession>();
        private ConcurrentQueue<CNetworkSession> Send_buffer = new ConcurrentQueue<CNetworkSession>();
        private ConcurrentQueue<CNetworkSession> Database_Buffer = new ConcurrentQueue<CNetworkSession>();
        private static ConcurrentDictionary<string, CNetworkSession> Clients = new ConcurrentDictionary<string, CNetworkSession>();
        private static object RecvObj = new object();
        private static object SendObj = new object();
        private static object DBObj = new object();
        private const int BUF_SIZE = 1024 * 4;

        private CDataFactory() { }

        /*
         * Return PacketFactory
         */
        public static CDataFactory GetDataFactory()
        {
            return DataFactory;
        }

        /*
         * set Session at receive buffer
         */
        public bool SetRecvBuffer(Socket socket, byte[] buffer)
        {
            CNetworkSession Recv = new CNetworkSession(socket, buffer);
            Recv_buffer.Enqueue(Recv);
            WakeThread(RecvObj);
            return true;
        }

        public bool SetRecvBuffer(CNetworkSession Session)
        {
            Recv_buffer.Enqueue(Session);
            WakeThread(RecvObj);
            return true;
        }


        /*
         * get Session from receive buffer
         */
        public CNetworkSession GetRecvBuffer()
        {
            CNetworkSession Session = null;
            if (Recv_buffer.Count > 0)
                Recv_buffer.TryDequeue(out Session);
            else
                SleepThread(RecvObj);

            return Session;
        }

        /*
         * set Session at send buffer
         */
        public bool SetSendBuffer(CNetworkSession Session)
        {
            Send_buffer.Enqueue(Session);
            WakeThread(SendObj);
            return true;
        }

        /*
         * get Session from send buffer
         */
        public CNetworkSession GetSendBuffer()
        {
            CNetworkSession Session = null;
            if (Send_buffer.Count > 0)
                Send_buffer.TryDequeue(out Session);
            else
                SleepThread(SendObj);
            return Session;
        }

        /*
         * set Session at database buffer
         */
        public bool SetDatabseBuffer(CNetworkSession Session)
        {

            Database_Buffer.Enqueue(Session);
            WakeThread(DBObj);
            return true;
        }

        /*
         * get Session from database buffer
         */
        public CNetworkSession GetDatabaseBuffer()
        {
            CNetworkSession Session = null;

            if (Database_Buffer.Count > 0)
                Database_Buffer.TryDequeue(out Session);
            else
                SleepThread(DBObj);

            return Session;
        }

        public bool SetClients(CNetworkSession Session, string id)
        {
            return Clients.TryAdd(id, new CNetworkSession(Session._socket, Session._buffer));
        }

        public bool isLogined(CNetworkSession Session)
        {
            string id = ((Login)Packet.Deserialize(Session._buffer)).id_str;
            return Clients.ContainsKey(id);
        }

        public void doHeartBeat()
        {
            byte[] buffer;
            HeartBeat heartBeat = new HeartBeat();
            DateTime Starttime = DateTime.Now;
            TimeSpan Limit = new TimeSpan(0, 10, 0);
            CNetworkSession eraseSession;
            foreach (KeyValuePair<string, CNetworkSession> sessions in Clients)
            {
                if (sessions.Value._socket.Connected)
                {
                    TimeSpan diff = Starttime - sessions.Value._datetime;
                    if (TimeSpan.Compare(Limit, diff) == -1)
                    {
                        sessions.Value._socket.Close();
                        Clients.TryRemove(sessions.Key, out eraseSession);
                    }
                    else
                    {
                        heartBeat.id_str = sessions.Key;
                        buffer = Packet.Serialize(heartBeat);
                        if (!SetSendBuffer(new CNetworkSession(sessions.Value._socket, buffer, PacketType.Heart_Beat)))
                            Console.WriteLine("DATA FACTORY : Heart Beat Error\n");
                        else
                            Console.WriteLine("DATA FACTORY : HeartBeat Sending " + sessions.Key + " " + sessions.Value._datetime.ToString("yyyy/MM/dd hh:mm:ss"));
                    }
                }
                else
                {
                    sessions.Value._socket.Close();
                    Clients.Remove(sessions.Key, out eraseSession);
                }
            }

        }
        public bool setHeartBeat(string id)
        {
            Clients[id]._datetime = DateTime.Now;
            Console.WriteLine("Update Heart Beat time " + id + " " + Clients[id]._datetime.ToString("yyyy/MM/dd hh:mm:ss"));

            return true;
        }
        public void freelock()
        {
            WakeThread(DBObj);
            WakeThread(RecvObj);
            WakeThread(SendObj);
        }
        private void SleepThread(object sleeplock)
        {
            lock (sleeplock)
                System.Threading.Monitor.Wait(sleeplock);
        }

        private void WakeThread(object wakelock)
        {
            lock (wakelock)
                System.Threading.Monitor.Pulse(wakelock);
        }
    }
}