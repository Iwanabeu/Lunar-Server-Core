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


        }
    }
}
