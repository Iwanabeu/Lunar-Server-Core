#region

using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class Marketplace : World
    {
        public Marketplace()
        {
            Name = "Marketplace";
            ClientWorldName = "Marketplace";
            Background = 2;
            AllowTeleport = false;
            Difficulty = -1;
            
        }

        protected override void Init()
        {
            // temporary placeholder
            LoadMap("wServer.realm.worlds.maps.marketplace.jm", MapType.Json);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new Marketplace());
        }
    }
}