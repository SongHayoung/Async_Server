using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using GTK_Server.Handler;
using GTK_Demo_Packet;

namespace GTK_Server.Network
{
    public class CNWManager
    {
        private const int Port = 5011;
        private const int Buf_Size = 1024 * 4;

        private static List<Socket> Clients = new List<Socket>();
        private static Socket Server;

        public CNWManager()
        {
        }

        public static void Run()
        {
            Console.WriteLine("Netwrok Manager on Active");
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, Port);
            Server.Bind(EndPoint);
            Server.Listen(100);

            SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
            Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Accept);
            Server.AcceptAsync(Asynce);
            Handling();
            Console.WriteLine("Network Manager Join");
        }

        private static void Handling()
        {
            CPacketFactory PacketFactory = CPacketFactory.GetCPacketFactory();
            while(Program.IsRunning())
            {
                foreach(Socket _Client in Clients)
                {
                    if(!_Client.Connected)
                    {
                        Clients.Remove(_Client);
                    }
                }
                CDataSet Item = PacketFactory.GetSendBuffer();
                if (Item == null)
                    continue;
                Socket Client = Item._socket;
                byte[] buffer = Item._buffer;
                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, buffer.Length);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Send);
                Client.SendAsync(Asynce);
            }
        }

        private static void Accept(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = e.AcceptSocket;

            if(Clients!=null)
            {
                Clients.Add(Client);

                e.UserToken = Clients;

                byte[] buffer = new byte[Buf_Size];
                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, buffer.Length);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve);
                Client.ReceiveAsync(Asynce);

                Socket server = (Socket)sender;
                e.AcceptSocket = null;
                server.AcceptAsync(e);
            }
        }


        private static void Recieve(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = (Socket)sender;
            if (Client.Connected)
            {
                byte[] buffer = e.Buffer;

                CPacketFactory PacketFactory = CPacketFactory.GetCPacketFactory();
                PacketFactory.SetRecvBuffer(Client, buffer);

                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, Buf_Size);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve);
                Client.ReceiveAsync(Asynce);
            }
            else
            {

            }
        }

        private static void Send(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine("Packet Sended");
        }

    }
    /* package has client IP, Port, 
    public class CNetworkSession   
    {
        IPEndPoint EndPoint;
        
    }*/
}
