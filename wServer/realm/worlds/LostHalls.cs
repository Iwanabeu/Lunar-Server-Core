using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.realm.worlds
{
    class LostHalls : World
    {
        public LostHalls()
        {
            Name = "Lost Halls";
            ClientWorldName = "Lost Halls";
            Background = 2;
            AllowTeleport = false;
            Difficulty = -1;

        }

        protected override void Init()
        {
            // temporary placeholder
            LoadMap("wServer.realm.worlds.maps.marketplace.jm", MapType.Json);
        }
        
    }
}
