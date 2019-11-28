using System;
using System.Net.Sockets;

using GTK_Demo_Packet;

namespace GTK_Server.Handler
{
    public class CDataSet
    {
        public Socket _socket;
        public byte[] _buffer;
        public PacketType _packettype;

        public CDataSet() { }
        public CDataSet(Socket s, byte[] b)
        {
            _socket = s;
            _buffer = b;
            _packettype = Packet.GetPacketType(b);
        }

        public CDataSet(Socket s, byte[] b, PacketType p)
        {
            _socket = s;
            _buffer = b;
            _packettype = p;
        }
    }
}
