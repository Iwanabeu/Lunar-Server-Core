using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        public int delayTillHorse = 0;
        public bool onHorse = false;
        public uint horseType;
        public void summonHorse()
        {
            if (onHorse) return;
            if (delayTillHorse > 0)
            {
                SendInfo("You can't summon your horse for: " + (int)delayTillHorse/1000 + " more seconds.");
                return;
            }
            SendInfo("Horse summoned.");
            onHorse = true;
            Stats[(int)Stat.Spd] += 100;
        }
        public void exitHorse()
        {
            if (!onHorse) return;
            SendInfo("Goodbye horse!");
            onHorse = false;
            Stats[(int)Stat.Spd] -= 100;
            delayTillHorse = 5000;
        }
        private void handleHorse(RealmTime time)
        {
            if (delayTillHorse > 0) delayTillHorse -= time.thisTickTimes;
        }
    }
}
