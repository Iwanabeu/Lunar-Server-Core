#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic
{
    public abstract class MultiTransition : IStateChildren
    {
        [ThreadStatic] private static Random rand;
        private readonly string[] targetStates;
        IDictionary<string, State> states;

        public MultiTransition(string[] targetStates)
        {
            this.targetStates = targetStates;
        }

        public State TargetState { get; private set; }

        protected static Random Random
        {
            get
            {
                if (rand == null) rand = new Random();
                return rand;
            }
        }

        public bool Tick(Entity host, RealmTime time)
        {
            object state;
            if (!host.StateStorage.TryGetValue(this, out state))
                state = null;
            Tuple<bool, String> ret = TickCore(host, time, ref state);
            if (ret.Item1)
            {
                try
                {
                    host.SwitchTo(states[ret.Item2]);
                }
                catch (System.Collections.Generic.KeyNotFoundException e)
                {
                    throw new Exception("State not found: " + ret.Item2 + "\n" + e.StackTrace);
                }
            }

            if (state == null)
                host.StateStorage.Remove(this);
            else
                host.StateStorage[this] = state;
            return ret.Item1;
        }

        protected abstract Tuple<bool,String> TickCore(Entity host, RealmTime time, ref object state);

        internal void Resolve(IDictionary<string, State> states)
        {
            this.states = states;
        }
    }
}