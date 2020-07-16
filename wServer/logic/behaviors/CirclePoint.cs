using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Game;
using wServer.realm;

namespace wServer.logic.behaviors
{
    class CirclePoint : CycleBehavior
    {
        private readonly float X;
        private readonly float Y;
        private readonly float radius;
        private readonly float radiusVariance;
        private readonly float speed;
        private readonly float speedVariance;
        private bool backwards = false;
        public CirclePoint(int X, int Y, double radius, double radiusVariance, double speed, double speedVariance)
        {
            this.X = X;
            this.Y = Y;
            this.radius = (float)radius;
            this.radiusVariance = (float)radiusVariance;
            this.speed = (float)speed;
            this.speedVariance = (float)speedVariance;
        }
        public CirclePoint(int X, int Y, double radius, double radiusVariance, double speed, double speedVariance, bool backwards=false)
        {
            this.X = X;
            this.Y = Y;
            this.radius = (float)radius;
            this.radiusVariance = (float)radiusVariance;
            this.speed = (float)speed;
            this.speedVariance = (float)speedVariance;
            this.backwards = backwards;
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new CircleState
            {
                Speed = speed + speedVariance * (float)(Random.NextDouble() * 2 - 1),
                Radius = radius + radiusVariance * (float)(Random.NextDouble() * 2 - 1)
            };
        }
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            //var en = host.GetNearestEntity(20, null);
            //var player = en as Player;

            //if (en == null)
            //{
            //    return;
            //}
            
            CircleState s = (CircleState)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) return;


            double angle;
            if (host.Y == this.Y && host.X == this.X) //small offset
                angle = Math.Atan2(host.Y - this.Y + (Random.NextDouble() * 2 - 1),
                    host.X - this.X + (Random.NextDouble() * 2 - 1));
            else
                angle = Math.Atan2(host.Y - this.Y, host.X - this.X);
            float angularSpd = host.GetSpeed(host.CurrentState.Name.Contains("fast")?s.Speed*2:s.Speed) / s.Radius;
            angle += angularSpd * (time.thisTickTimes / 1000f) * (this.backwards?-1:1);

            double x = this.X + Math.Cos(angle) * radius;
            double y = this.Y + Math.Sin(angle) * radius;
            Vector2 vect = new Vector2((float)x, (float)y) - new Vector2(host.X, host.Y);
            vect.Normalize();
            vect *= host.GetSpeed(host.CurrentState.Name.Contains("fast") ? s.Speed * 2 : s.Speed) * (time.thisTickTimes / 1000f);

            host.ValidateAndMove(host.X + vect.X, host.Y + vect.Y);
            host.UpdateCount++;

            Status = CycleStatus.InProgress;
        }

        private class CircleState
        {
            public float Radius;
            public float Speed;
        }
    }
}
