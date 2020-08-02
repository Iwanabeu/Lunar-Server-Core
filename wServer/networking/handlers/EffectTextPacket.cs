namespace wServer.networking.svrPackets
{
    public class EffectTextPacket : ServerPacket
    {
        public string Message { get; set; }

        public override PacketID ID
        {
            get { return PacketID.SENDEFFECTTEXT; }
        }

        public override Packet CreateInstance()
        {
            return new EffectTextPacket();
        }

        protected override void Read(Client psr, NReader rdr)
        {
            Message = rdr.ReadString();
        }

        protected override void Write(Client psr, NWriter wtr)
        {
            wtr.WriteUTF(Message);
        }
    }
}