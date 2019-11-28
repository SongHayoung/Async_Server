using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using GTK_Demo_Packet;

namespace GTK_Server.Handler
{
    public class CPacketFactory
    {
        private static CPacketFactory PacketFactory = new CPacketFactory();
        private Stack<CDataSet> Recv_buffer = new Stack<CDataSet>();
        private Stack<CDataSet> Send_buffer = new Stack<CDataSet>();
        private Stack<CDataSet> Database_Buffer = new Stack<CDataSet>();

        private static object Recv_Lock = new object();
        private static object Send_Lock = new object();
        private static object Database_Lock = new object();

        private const int BUF_SIZE = 1024 * 4;

        private CPacketFactory() { }

        public static CPacketFactory GetCPacketFactory()
        {
            return PacketFactory;
        }

        public bool SetRecvBuffer(Socket socket, byte[] Item)
        {
            CDataSet Recv = new CDataSet(socket, Item);
            lock (Recv_Lock)
            {
                Recv_buffer.Push(Recv);
            }
            return true;
        }

        public CDataSet GetRecvBuffer()
        {
            CDataSet Item;
            lock (Send_Lock)
            {
                if(Send_buffer.Count>0)
                {
                    Item = Recv_buffer.Pop();
                }
                else
                {
                    Item = null;
                }
            }
            return Item;
        }

        public bool SetSendBuffer(CDataSet Item)
        {
            lock(Send_Lock)
            {
                Send_buffer.Push(Item);
            }
            return true;
        }

        public CDataSet GetSendBuffer()
        {
            CDataSet Item;
            lock (Send_Lock)
            {
                if (Send_buffer.Count > 0)
                {
                    Item = Send_buffer.Pop();
                }
                else
                {
                    Item = null;
                }
            }
            return Item;
        }

        public bool SetDatabseBuffer(CDataSet Item)
        {
            lock(Database_Lock)
            {
                Database_Buffer.Push(Item);
            }
            return true;
        }

        public CDataSet GetDatabaseBuffer()
        {
            CDataSet Item;
            lock(Database_Lock)
            {
                if(Database_Buffer.Count>0)
                {
                    Item = Database_Buffer.Pop();
                }
                else
                {
                    Item = null;
                }
            }
            return Item;
        }
    }
}
