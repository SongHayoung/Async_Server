using System.Collections.Generic;
using System.Net.Sockets;

namespace GTK_Server.Handler
{
    public class CPacketFactory
    {
        private static CPacketFactory PacketFactory = new CPacketFactory();
        private Stack<CNetworkSession> Recv_buffer = new Stack<CNetworkSession>();
        private Stack<CNetworkSession> Send_buffer = new Stack<CNetworkSession>();
        private Stack<CNetworkSession> Database_Buffer = new Stack<CNetworkSession>();

        private static object Recv_Lock = new object();
        private static object Send_Lock = new object();
        private static object Database_Lock = new object();

        private const int BUF_SIZE = 1024 * 4;

        private CPacketFactory() { }

        /*
         * Return PacketFactory
         */
        public static CPacketFactory GetCPacketFactory()
        {
            return PacketFactory;
        }

        /*
         * set Session at receive buffer
         */
        public bool SetRecvBuffer(Socket socket, byte[] buffer)
        {
            CNetworkSession Recv = new CNetworkSession(socket, buffer);
            lock (Recv_Lock)
            {
                Recv_buffer.Push(Recv);
            }
            return true;
        }

        /*
         * get Session from receive buffer
         */
        public CNetworkSession GetRecvBuffer()
        {
            CNetworkSession Session;
            lock (Send_Lock)
            {
                if(Send_buffer.Count>0)
                {
                    Session = Recv_buffer.Pop();
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
                Send_buffer.Push(Session);
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
                    Session = Send_buffer.Pop();
                }
                else
                {
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
                Database_Buffer.Push(Session);
            }
            return true;
        }

        /*
         * get Session from database buffer
         */
        public CNetworkSession GetDatabaseBuffer()
        {
            CNetworkSession Session;
            lock(Database_Lock)
            {
                if(Database_Buffer.Count>0)
                {
                    Session = Database_Buffer.Pop();
                }
                else
                {
                    Session = null;
                }
            }
            return Session;
        }
    }
}
