
using wServer.networking.cliPackets;

namespace wServer.networking.handlers
{

    internal class SetClassHandler : PacketHandlerBase<SetClassPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.CHANGESUBCLASS; }

        }
        protected override void HandlePacket(Client client, SetClassPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => client.Player.Set_Subclass(t, packet),realm.PendingPriority.Networking);
        }
    }
}
