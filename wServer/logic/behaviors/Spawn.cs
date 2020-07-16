#region

using wServer.realm;
using wServer.realm.entities;
using System.Linq;

#endregion

namespace wServer.logic.behaviors
{
    public class Spawn : Behavior
    {
        //State storage: Spawn state
        private readonly ushort children;
        private readonly int initialSpawn;
        private readonly int maxChildren;
        private Cooldown coolDown;
        private int cooldownOffset = -1;
        private readonly string child;
        private bool once = false;

        public Spawn(string children, int maxChildren = 5, double initialSpawn = 0.5, Cooldown coolDown = new Cooldown() )
        {
            this.children = BehaviorDb.InitGameData.IdToObjectType[children];
            this.child = children;
            this.maxChildren = maxChildren;
            this.initialSpawn = (int) (maxChildren*initialSpawn);
            this.coolDown = coolDown.Normalize(0);
        }
        public Spawn(bool once, string children, Cooldown coolDown,int maxChild, double initialSpawn,int CoolDownOffset=0)
        {
            this.children = BehaviorDb.InitGameData.IdToObjectType[children];
            this.maxChildren = maxChild;
            this.initialSpawn = (int)(maxChildren * initialSpawn);
            this.coolDown = coolDown.Normalize(0);
            this.cooldownOffset = CoolDownOffset;
            this.once = once;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new SpawnState
            {
                CurrentNumber = initialSpawn,
                RemainingTime = cooldownOffset==-1?coolDown.Next(Random):coolDown.Next(Random)+cooldownOffset,
                
            };
            for (int i = 0; i < initialSpawn; i++)
            {
                Entity entity = Entity.Resolve(host.Manager, children);

                entity.Move(
                    host.X + (float) (Random.NextDouble()*0.5),
                    host.Y + (float) (Random.NextDouble()*0.5));
                if(host is Enemy && entity is Enemy)
                    (entity as Enemy).Terrain = (host as Enemy).Terrain;
                host.Owner.EnterWorld(entity);
            }
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (once) return;
            SpawnState spawn = (SpawnState) state;
            if (host is Enemy)
            {
                spawn.CurrentNumber = host.Owner.Enemies.Values.Where(e => e.Name.EqualsIgnoreCase(this.child)).Count();
            }
            if (spawn.RemainingTime <= 0 && spawn.CurrentNumber < maxChildren)
            {
                Entity entity = Entity.Resolve(host.Manager, children);
                entity.Move(host.X, host.Y);
                if (host is Enemy)
                    (entity as Enemy).Terrain = (host as Enemy).Terrain;
                host.Owner.EnterWorld(entity);
                spawn.RemainingTime = coolDown.Next(Random);
                spawn.CurrentNumber++;
            }
            else
                spawn.RemainingTime -= time.thisTickTimes;
        }

        private class SpawnState
        {
            public int CurrentNumber;
            public int RemainingTime;
        }
    }
}