using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using GTK_Demo_Packet;

namespace GTK_Server.Network
{
    public class CNWManager
    {
        private const int Port = 5011;
        private const int Buf_Size = 1024 * 4;

        private List<Socket> Clients;
        private Socket Server;

        public CNWManager()
        {
            Clients = new List<Socket>();
        }

        public void Run()
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, Port);
            Server.Bind(EndPoint);
            Server.Listen(100);

            SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
            Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Accept);
            Server.AcceptAsync(Asynce);
        }

        private void Accept(object sender, SocketAsyncEventArgs e)
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


        private void Recieve(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = (Socket)sender;
            if (Client.Connected)
            {
                Client.Receive(e.Buffer);
                byte[] buffer = e.Buffer;

                CNWPacketHandler PacketHandler = new CNWPacketHandler(buffer);
                Array.Copy(PacketHandler.GetSendBuffer(), 0, buffer, 0, PacketHandler.GetSendBufferLength());

                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, Buf_Size);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Send);
                Client.SendAsync(Asynce);
            }
            else
            {

            }
        }

        private void Send(object sender, SocketAsyncEventArgs e)
        {
            Socket Client = (Socket)sender;
            if(Client.Connected)
            {
                Client.Send(e.Buffer);
                byte[] buffer = e.Buffer;

                SocketAsyncEventArgs Asynce = new SocketAsyncEventArgs();
                Asynce.SetBuffer(buffer, 0, Buf_Size);
                Asynce.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve);
                Client.ReceiveAsync(Asynce);
            }
        }

    }
    /* package has client IP, Port, 
    public class CNetworkSession   
    {
        IPEndPoint EndPoint;
        
    }*/
}
