using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;
using wServer.realm.entities;

namespace wServer.logic.behaviors
{
    class SpecificBehavior : Behavior
    {
        string Name;
        int cd;
        public SpecificBehavior(string Name, int cd)
        {
            this.Name = Name;
            this.cd = cd;
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = this.cd;
        }
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {

            if (this.Name == "close bridge 1")
            {
                if (host.CurrentState.Name == "KeepMoving") return;
                if (host.Y < 185 || host.Y > 200) return;
                if (host.Y <= 193)
                {
                    host.Move(host.X, host.Y + 1);
                }
                else if (host.Y > 193)
                {
                    host.Move(host.X, host.Y - 1);
                }
                return;


            }
            else if (this.Name == "bridge 1 suicide")
            {
                if (host.Y == 192.5)
                {
                    State suicide = new State(new Suicide());
                    (host as Enemy).Death(time);
                }
            }
            else if (this.Name == "Spawn Pillar")
            {
                String[] states_of_void = {"Phase 17 1 a", "Phase 17 1 b", "Phase 17 2", "Phase 17 3 Start", "Phase 17 3 a", "Phase 17 3 b", "Phase 17 4", "Death" };
                int tobemade = 1;
                int cool = (int)state;
                cool -= time.thisTickTimes;
                if (cool <= 0)
                {
                    IEnumerable<Enemy> enemies = host.Owner.Enemies.Values.Where(e => e.Name == "LH Colossus Pillar");
                    foreach (Enemy e in enemies)
                    {
                        if (e.X == 63 && e.Y == 62) tobemade *= 2;
                        if (e.X == 63 && e.Y == 76) tobemade *= 3;
                        if (e.X == 77 && e.Y == 62) tobemade *= 5;
                        if (e.X == 77 && e.Y == 76) tobemade *= 7;
                    }
                    
                    if (tobemade % 2 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Pillar");
                        entity.Move(63, 62);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 3 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Pillar");
                        entity.Move(63, 76);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 5 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Pillar");
                        entity.Move(77, 62);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 7 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Pillar");
                        entity.Move(77, 76);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }


                    cool = 30000;
                }
                state = cool;
            }
            else if (this.Name == "LH Balls 1")
            {
                int tobemade = 1;
                int cool = (int)state;
                cool -= time.thisTickTimes;
                if (cool <= 0)
                {
                    IEnumerable<Enemy> enemies = host.Owner.Enemies.Values.Where(e => e.Name.Contains("LH Colossus Rock"));
                    foreach (Enemy e in enemies)
                    {
                        if (e.Name == "LH Colossus Rock 1") tobemade *= 2;
                        if (e.Name == "LH Colossus Rock 2") tobemade *= 3;
                        if (e.Name == "LH Colossus Rock 3") tobemade *= 5;
                    }
                    if (tobemade % 2 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 1");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 3 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 2");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 5 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 3");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    cool = 3000;
                }
                state = cool;

            }
            else if (this.Name == "LH Balls 2")
            {
                int tobemade = 1;
                int cool = (int)state;
                cool -= time.thisTickTimes;
                if (cool <= 0)
                {
                    IEnumerable<Enemy> enemies = host.Owner.Enemies.Values.Where(e => e.Name.Contains("LH Colossus Rock"));
                    foreach (Enemy e in enemies)
                    {
                        if (e.Name == "LH Colossus Rock 4") tobemade *= 2;
                        if (e.Name == "LH Colossus Rock 5") tobemade *= 3;
                        if (e.Name == "LH Colossus Rock 6") tobemade *= 5;
                    }
                    if (tobemade % 2 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 4");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 3 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 5");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    if (tobemade % 5 != 0)
                    {
                        Entity entity = Entity.Resolve(host.Manager, "LH Colossus Rock 6");
                        entity.Move(host.X, host.Y);
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                        host.Owner.EnterWorld(entity);
                    }
                    cool = 3000;
                }
                state = cool;

            }
            else if (this.Name == "LH Color Changer")
            {
                
                if ((host as Enemy).HP < host.ObjectDesc.MaxHP * 0.8)
                {
                    (host as Enemy).AltTextureIndex = 5;
                    host.UpdateCount++;
                }
                if ((host as Enemy).HP < host.ObjectDesc.MaxHP * 0.6)
                {
                    (host as Enemy).AltTextureIndex = 6;
                    host.UpdateCount++;
                }
                if ((host as Enemy).HP < host.ObjectDesc.MaxHP * 0.4)
                {
                    (host as Enemy).AltTextureIndex = 7;
                    host.UpdateCount++;
                }
                if ((host as Enemy).HP < host.ObjectDesc.MaxHP * 0.2)
                {
                    (host as Enemy).AltTextureIndex = 8;
                    host.UpdateCount++;
                }
            }
        }
        
    }
}
