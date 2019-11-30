using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using GTK_Server.Handler;
namespace GTK_Server.Network
{
    /*
     * this class help managing network
     */
    public class CNWManager
    {
        private const int Port = 5011;
        private const int Buf_Size = 1024 * 4;

        private static List<Socket> Clients = new List<Socket>();
        private static Socket Server;

        public CNWManager() { }

        /* 
         * Initalize NetworkManager
         * Running NetworkManager
         */
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

        /*
         * This function detect dead clients and accur sendAsync
         */
        private static void Handling()
        {
            while(Program.IsRunning())
            {
                foreach(Socket _Client in Clients)
                {
                    if(!_Client.Connected)
                    {
                        Clients.Remove(_Client);
                    }
                }
                CNetworkSession Session = CDataHandler.Handling_SendPacket();
                if (Session == null)
                    continue;
                NWM_log("Sending Packet");
                Socket Client = Session._socket;
                byte[] buffer = Session._buffer;
                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, buffer.Length);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Send);
                try
                {
                    Client.SendAsync(Asynce);
                }
                catch(SocketException se)
                {
                    Console.WriteLine("Socket Exception : " + se.ErrorCode + "Message : " + se.Message);
                }
            }
        }

        /*
         * Accept Client Async
         */
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

        /*
         * this completed event called when socket recieve as Async
         * this event call Recieve event again
         */
        private static void Recieve(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = (Socket)sender;
            bool CompleteAsync = false;
            if (Client.Connected )//&& e.BytesTransferred>0)
            {
                byte[] buffer = e.Buffer;
                CDataHandler.Handling_RecvPacket(Client, buffer);
                NWM_log("Receving Packet");
                CompleteAsync = Client.ReceiveAsync(e);
                if (!CompleteAsync)
                {   //this block is running at ReceiveAsync done as Sync
                    //need to consider this block to get useful way
                    //in multi-thread singleton pattern
                    Client.ReceiveAsync(e);
                }
            }
            else
            {
                Clients.Remove(Client);
                Client.Close();
            }
        }

        /*
         * Do nothing
         */
        private static void Send(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = (Socket)sender;
            if(Client.Connected)
            {

            }
        }

        private static void NWM_log(string str)
        {
            Console.WriteLine("Network Manager : " + str);
        }
    }
}
