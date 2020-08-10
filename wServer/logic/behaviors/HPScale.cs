using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

namespace wServer.logic.behaviors
{
    public class HPScale : Behavior
    {
        private readonly double percentPerPerson;

        public HPScale(double percentPerPerson)
        {
            this.percentPerPerson = percentPerPerson;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            int numPlayers = host.Owner.Players.Where(p => host.Dist(p.Value) <= 20).Count();
            int newHP =(int) (host.ObjectDesc.MaxHP * (1 + (numPlayers *percentPerPerson)));
            Enemy e = (host as Enemy);
            e.maxHP = e.HP = newHP;
        }
    }
}
