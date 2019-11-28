using System.Net.Sockets;

using GTK_Demo_Packet;

namespace GTK_Server.Handler
{
    /*
     * Class for Session
     */
    public class CNetworkSession
    {
        public Socket _socket;
        public byte[] _buffer;
        public PacketType _packettype;

        public CNetworkSession() { }
        public CNetworkSession(Socket s, byte[] b)
        {
            _socket = s;
            _buffer = b;
            _packettype = Packet.GetPacketType(b);
        }

        public CNetworkSession(Socket s, byte[] b, PacketType p)
        {
            _socket = s;
            _buffer = b;
            _packettype = p;
        }
    }
}
