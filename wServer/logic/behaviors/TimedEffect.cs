using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Game;
using wServer.realm;

namespace wServer.logic.behaviors
{
    class TimedEffect : CycleBehavior
    {
        private ConditionEffectIndex effect;
        private int duration;
        public TimedEffect(ConditionEffectIndex effect,int duration=1000)
        {
            this.effect = effect;
            this.duration = duration;
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            host.ApplyConditionEffect(new ConditionEffect
            {
                Effect = this.effect,
                DurationMS = this.duration

            });
        }
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            
        }
    }
}
