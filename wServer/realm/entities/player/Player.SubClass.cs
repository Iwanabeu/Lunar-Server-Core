using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;

namespace wServer.realm.entities.player
{
    partial class Player
    {
        public bool[] feats;
        public int subclass;
        public void Set_Subclass(RealmTime time, SetClassPacket pkt)
        {
            this.SendInfo("Changing subclass to: " + pkt.subclass + " " + pkt.Feats);
            if (this.Owner.Name != "Nexus")
            {
                this.SendInfo("You cannot change subclass outside of the nexus.");
                return;
            }
            if (verifySubclass(subclass, feats))
            {
                this.feats = pkt.Feats;
                this.subclass = pkt.subclass;
                this.SaveToCharacter();
                this.Client.Save();
                return;
            }
            this.SendInfo("Invalid setup.");
            return;


        }
        public bool verifySubclass(int subclass, bool[] feats)
        {
            return true;
        }
    }
}
