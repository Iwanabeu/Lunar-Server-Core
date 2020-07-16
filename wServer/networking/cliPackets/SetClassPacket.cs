using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace wServer.networking.cliPackets
{
    public class SetClassPacket : ClientPacket
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SetClassPacket));
        public bool[] Feats;
        public int subclass;
        public override PacketID ID
        {
            get { return PacketID.CHANGESUBCLASS; }
        }

        public override Packet CreateInstance()
        {
            return new SetClassPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            subclass = rdr.ReadInt16();
            Feats = new bool[28];
            for (int i = 0; i < Feats.Length; i++)
                Feats[i] = rdr.ReadBoolean();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Flush();
            wtr.Write(subclass);
            foreach (bool i in Feats)
                wtr.Write(i);
        }
    }
}
