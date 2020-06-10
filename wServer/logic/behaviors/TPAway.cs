using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

namespace wServer.logic.behaviors
{
    public class TPAway : Behavior
    {
        //State storage: nothing

        private readonly double detectradius;
        private readonly float tpdist;
        

        public TPAway(double detectradius, double tpdist)
        {
            this.detectradius = detectradius;
            this.tpdist = (float)tpdist;

        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            var pos = new Position
            {
                X = host.X,
                Y = host.Y
            };
            Player p = host.getNearestPlayer(this.detectradius);
            if (host.X == p.X) {
                if (host.Y < p.Y) host.ValidateAndMove(host.X, host.Y - tpdist);
                else host.ValidateAndMove(host.X, host.Y + tpdist);
            }
            else {
                double angle = Math.Atan((host.Y-p.Y)/(host.X - p.X));
                host.Move(host.X + (float)Math.Cos(angle) * tpdist, host.Y + (float)Math.Sin(angle) * tpdist);
            }
            
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state) { }
    }
}
