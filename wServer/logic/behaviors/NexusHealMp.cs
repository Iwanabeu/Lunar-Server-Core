﻿#region

using System;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;

#endregion

namespace wServer.logic.behaviors
{
    public class NexusHealMp : Behavior
    {
        //State storage: cooldown timer

        private readonly int amount;
        private readonly double range;
        private Cooldown coolDown;

        public NexusHealMp(double range, int amount, Cooldown coolDown = new Cooldown())
        {
            this.range = (float)range;
            this.amount = amount;
            this.coolDown = coolDown.Normalize();
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;

            if (cool <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Sick)) return;

                Player entity = host.GetNearestEntity(range, null) as Player;

                if (entity != null)
                {
                    int maxMp = entity.Stats[1] + entity.Boost[1] + entity.getBonusMp();
                    
                    int newMp = Math.Min(maxMp, entity.Mp+ amount);
                    if (newMp != entity.Mp)
                    {
                        int n = newMp - entity.Mp;
                        entity.Mp = newMp;
                        entity.UpdateCount++;
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Potion,
                            TargetId = entity.Id,
                            Color = new ARGB(0x000000ff)
                        }, null);
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Trail,
                            TargetId = host.Id,
                            PosA = new Position { X = entity.X, Y = entity.Y },
                            Color = new ARGB(0x000000ff)
                        }, null);
                        entity.Owner.BroadcastPacket(new NotificationPacket
                        {
                            ObjectId = entity.Id,
                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                            Color = new ARGB(0x000000ff)
                        }, null);
                    }
                }
                cool = coolDown.Next(Random);
            }
            else
                cool -= time.thisTickTimes;

            state = cool;
        }
    }
}