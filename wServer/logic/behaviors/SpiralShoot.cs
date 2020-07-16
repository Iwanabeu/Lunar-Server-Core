using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;
using wServer.realm.entities;
using wServer.networking.svrPackets;

namespace wServer.logic.behaviors
{
    class SpiralShoot : CycleBehavior
    {
        int cooldownOffset;
        int projectileIndex;
        int shotCount;
        int armCount;
        double startAngle;
        double angleBetweenShots;
        int msBetweenShots;
        double angleBetweenArms;

        public SpiralShoot(int projectileIndex, int shotCount, int armCount, double startAngle, double angleBetweenShots, int cooldownOffset, int msBetweenShots, double angleBetweenArms)
        {
            this.cooldownOffset = cooldownOffset;
            this.projectileIndex = projectileIndex;
            this.shotCount = shotCount;
            this.armCount = armCount;
            this.startAngle = startAngle * Math.PI / 180;
            this.angleBetweenShots = angleBetweenShots * Math.PI / 180;
            this.msBetweenShots = msBetweenShots;
            this.angleBetweenArms = angleBetweenArms * Math.PI / 180;
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new Tuple<int, double>(cooldownOffset, startAngle);
        }
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {

            if (state == null) return;
            Tuple<int, double> stateData = (Tuple<int, double>)state;
            int cool = stateData.Item1;
            double angle = stateData.Item2;
            Status = CycleStatus.NotStarted;
            if (cool <= 0 || cool<time.thisTickTimes)
            {
                ProjectileDesc desc = host.ObjectDesc.Projectiles[projectileIndex];
                int dmg;
                if (host is Character)
                    dmg = (host as Character).Random.Next(desc.MinDamage, desc.MaxDamage);
                else
                    dmg = Random.Next(desc.MinDamage, desc.MaxDamage);
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) return;
                byte prjId = 0;
                
                Position prjPos = new Position { X = host.X, Y = host.Y };
                for (int i = 0; i < armCount; i++)
                {
                    

                    Projectile prj = host.CreateProjectile(
                        desc, host.ObjectType, dmg, time.tickTimes,
                        prjPos, (float)(angle + (i * angleBetweenArms)));
                    host.Owner.EnterWorld(prj);
                    if (i == 0)
                        prjId = prj.ProjectileId;
                }

                host.Owner.BroadcastPacket(new ShootPacket
                {
                    BulletId = prjId,
                    OwnerId = host.Id,
                    Position = prjPos,
                    Angle = (float)angle,
                    Damage = (short)dmg,
                    BulletType = (byte)desc.BulletType,
                    AngleInc = (float)angleBetweenArms,
                    NumShots = (byte)armCount,
                }, null);
                cool = this.msBetweenShots;
                angle = angle + this.angleBetweenShots;
            }
            else
            {
                cool -= time.thisTickTimes;
                Status = CycleStatus.InProgress;
                
            }
            state = new Tuple<int, double>(cool, angle);
        }
    }
}
