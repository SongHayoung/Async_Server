using System;

using GTK_Demo_Packet;
namespace GTK_Server.Handler
{
    public class CPacketHandler
    {
        public CPacketHandler()
        {
        }

        public static void Run()
        {
            Console.WriteLine("Handling Manager on Active");
            CPacketFactory PacketFactory = CPacketFactory.GetCPacketFactory();
            Handling(PacketFactory);
            Console.WriteLine("Handling Manager Join");
        }

        private static void Handling(CPacketFactory PacketFactory)
        {
            while (Program.IsRunning())
            {
                CDataSet Item = PacketFactory.GetRecvBuffer();
                if (Item == null)
                    continue;

                if (Item._packettype == PacketType.Login || Item._packettype == PacketType.Member_REGISTER)
                {
                    PacketFactory.SetDatabseBuffer(Item);
                }
            }
        }
    }
}
