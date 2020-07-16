using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;

namespace wServer.logic.transitions
{
    public class RandomTransition : MultiTransition
    {
        private string[] texts;
        private int delay ;

        public RandomTransition(int delay, params string[] targetStates)
            : base(targetStates)
        {
            this.texts = targetStates;
            this.delay = delay;
        }

        protected override Tuple<bool,string> TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool;
            if (state == null) cool =this.delay;
            else cool = (int) state;
            string newstate = "";
            bool ret = false;
            if (cool <= 0)
            {
                ret = true;
                newstate = texts.RandomElement(new Random());
                cool = this.delay;
            }
            else
                cool -= time.thisTickTimes;

            state = cool;
            return new Tuple<bool, string>(ret,newstate);
        }
    }
}
