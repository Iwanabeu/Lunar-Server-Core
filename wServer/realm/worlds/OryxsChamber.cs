using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking;

namespace wServer.realm.worlds
{
    class OryxsChamber : World
    {
        public OryxsChamber()
        {
            Name = "Oryx's Chamber";
            ClientWorldName = "server.Oryx's_Chamber";
            Background = 0;
            AllowTeleport = false;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.oryxchamber.jm", MapType.Json);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new OryxsChamber());
        }
    }
}
