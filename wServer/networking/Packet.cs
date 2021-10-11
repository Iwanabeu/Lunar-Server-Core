#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using wServer.networking.svrPackets;

#endregion

namespace wServer.networking
{
    public abstract class Packet
    {
        public static Dictionary<PacketID, Packet> Packets = new Dictionary<PacketID, Packet>();
        private log4net.ILog log = log4net.LogManager.GetLogger("YEP");
        static Packet()
        {
            foreach (Type i in typeof (Packet).Assembly.GetTypes())
                if (typeof (Packet).IsAssignableFrom(i) && !i.IsAbstract)
                {
                    Packet pkt = (Packet) Activator.CreateInstance(i);
                    if (!(pkt is ServerPacket))
                        if (!Packets.ContainsKey(pkt.ID))
                            Packets.Add(pkt.ID, pkt);
                }
        }

        public abstract PacketID ID { get; }
        public abstract Packet CreateInstance();

        public abstract void Crypt(Client client, byte[] dat, int offset, int len);

        public void Read(Client client, byte[] body, int offset, int len)
        {
            //new RC4(new byte[] { 0x31, 0x1f, 0x80, 0x69, 0x14, 0x51, 0xc7, 0x1d, 0x09, 0xa1, 0x3a, 0x2a, 0x6e }).Crypt(body, offset, len);
            //Crypt(client, body, offset, len);
            Read(client, new NReader(new MemoryStream(body)));
        }

        public int Write(Client client, byte[] buff, int offset)
        {

            //log.Error(this);
            MemoryStream s = new MemoryStream(buff, offset + 5, buff.Length - offset - 5);
            Write(client, new NWriter(s));

            int len = (int)s.Position;

            //new RC4(new byte[] { 0x72, 0xc5, 0x58, 0x3c, 0xaf, 0xb6, 0x81, 0x89, 0x95, 0xcd, 0xd7, 0x4b, 0x80 }).Crypt(buff, offset, len);
            //Crypt(client, buff, offset + 5, len);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len + 5)), 0, buff, offset, 4);
            buff[offset + 4] = (byte) ID;

            //log.LogBuffer(buff, offset, len+5);
            return len + 5;
        }

        protected abstract void Read(Client client, NReader rdr);
        protected abstract void Write(Client client, NWriter wtr);

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder("{");
            PropertyInfo[] arr = GetType().GetProperties();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i != 0) ret.Append(", ");
                ret.AppendFormat("{0}: {1}", arr[i].Name, arr[i].GetValue(this, null));
            }
            ret.Append("}");
            return ret.ToString();
        }
    }

    public class NopPacket : Packet
    {
        public override PacketID ID
        {
            get { return (PacketID) 255; }
        }

        public override Packet CreateInstance()
        {
            return new NopPacket();
        }

        public override void Crypt(Client client, byte[] dat, int offset, int len)
        {
        }

        protected override void Read(Client client, NReader rdr)
        {
        }

        protected override void Write(Client client, NWriter wtr)
        {
        }
    }
}