using System;
using GTK_Demo_Packet;

namespace GTK_Server.Network
{
    public class CNWPacketHandler
    {
        private byte[] Recv_buffer;
        private byte[] Send_buffer;
        private PacketType Packet_type;
        private const int BUF_SIZE = 1024 * 4;

        public CNWPacketHandler(byte[] recv)
        {
            Recv_buffer = new byte[BUF_SIZE];
            Send_buffer = new byte[BUF_SIZE];
            Array.Copy(recv, 0, Recv_buffer, 0, recv.Length);
            Packet_type = Packet.GetPacketType(recv);
        }

        public byte[] GetSendBuffer()
        {
            Handleing();

            return Send_buffer;
        }

        public int GetSendBufferLength()
        {
            return Send_buffer.Length;
        }

        private void SetSendBuffer(byte[] buffer)
        {
            Array.Copy(buffer, 0, Send_buffer, 0, buffer.Length);
        }

        private void Handleing()
        {
            switch(Packet_type)
            {
                case PacketType.Login:
                    {
                        Login login = (Login)Packet.Deserialize(Recv_buffer);
                        CNWLoginHandler loginHandler = new CNWLoginHandler(login);
                        SetSendBuffer(loginHandler.GetResultAsByte());
                    }
                    break;

                case PacketType.Member_REGISTER:
                    {
                        MemberRegister register = (MemberRegister)Packet.Deserialize(Recv_buffer);
                        CNWMemberRegisterHandler registHandler = new CNWMemberRegisterHandler(register);
                        SetSendBuffer(registHandler.GetResultAsByte());
                    }
                    break;
            }
        }


    }
}
