
using wServer.realm;
using wServer.realm.entities.player;

namespace wServer.logic.behaviors
{
    public class Debug :CycleBehavior
    {
        private int Range;
        public Debug(int AcquireRange)
        {
            this.Range = AcquireRange;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            Entity en = host.GetNearestEntity(Range,null);
            Player nearest = host.getNearestPlayer(this.Range);
            if (nearest == null) return;
            if (nearest.inDebug)
            {
                nearest.SendInfo("Entity: " + host.Name +" at position: (" + host.X + ", " + host.Y + ") is in state: "+host.CurrentState.Name);
            }
        }
    }
}
