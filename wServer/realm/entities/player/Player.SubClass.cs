using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;

namespace wServer.realm.entities.player
{
    partial class Player
    {
        enum Stat
        {
            Hp = 0,
            Mp = 1,
            Atk = 2,
            Def = 3,
            Spd = 4,
            Vit = 5,
            Wis = 6,
            Dex = 7
        }
        internal static class Feat
        {
            public static int Arcane_Adept = 0;
            public static int Accelerated_Casting = 1;
            public static int Combat_Caster = 2;
            public static int Prodigious_Mind = 3;
            public static int Unfazed = 4;
            public static int Mage_Aura = 5;
            public static int Revealer_Of_Secrets = 6;
            public static int Disciple_Of_War = 7;
            public static int Superior_Parry = 8;
            public static int Feral_Assailant = 9;
            public static int Deadly_Precision = 10;
            public static int Eagle_Eyed_Sentry = 11;
            public static int Full_Counter = 12;
            public static int Great_Weapons_Master = 13;
            public static int Tough_Skinned = 14;
            public static int Bolstered_Resilience = 15;
            public static int Show_No_Weakness = 16;
            public static int Man_Of_Steel = 17;
            public static int Raging_Vitality = 18;
            public static int Deny_Death = 19;
            public static int Indomitable = 20;
            public static int Lucky = 21;
            public static int Mobile = 22;
            public static int Adrenaline_Rush = 23;
            public static int Blessed_By_The_Gods = 24;
            public static int Slippery = 25;
            public static int Night_Vision = 26;
            public static int Master_Thief = 27;
            public static int One_Foot_In_The_Grave = 28;
            public static int Heros_Last_Stand = 29;
            public static int Hunters_Mark = 30;
            public static int Drunken_Brawler = 31;
            public static int Mammoth_Charge = 32;
            public static int High_Roller = 33;
            public static int Pot_Fiend = 34;
            public static int Greater_Teleportation = 35;
            public static int Mender_of_Wounds = 36;
            public static int Arch_Magus = 37;
            public static int Craven_Hearted = 38;
            public static int Second_Chance = 39;


        }
        public bool[] feats;
        public int subclass = -1;
        List<StatBoost> StatBoosts = new List<StatBoost>();
        int[] subclassCooldowns = { 0, 0, 0, 0, 0, 0 };
        public void Set_Subclass(RealmTime time, SetClassPacket pkt)
        {
            if (this.Owner.Name != "Nexus")
            {
                this.SendInfo("You cannot change subclass outside of the nexus.");
                return;
            }
            if (verifySubclass(subclass, feats))
            {
                this.feats = pkt.Feats;
                this.subclass = pkt.subclass;
                this.SaveToCharacter();
                this.Client.Save();

                UpdateCount++;
                return;
            }
            this.SendInfo("Invalid setup.");
            return;


        }
        public bool verifySubclass(int subclass, bool[] feats)
        {
            return true;
        }
        public int getBonusHp()
        {
            int result = 0;
            result += feats[Feat.High_Roller] ? (int)(-.4 * MaxHp) : 0;
            result += feats[Feat.Arch_Magus] ? ((int)(Stats[(int)Stat.Wis] * -.07)) : 0;
            return result;
        }
        public int getBonusMp()
        {
            int result = 0;
            result += feats[Feat.Prodigious_Mind] ? 30 : 0;
            result += feats[Feat.Arcane_Adept] ? 15 : 0;
            result += feats[Feat.Arch_Magus] ? ((int)(MaxMp * .07)) : 0;
            return result;
        }
        public int getBonusAtk()
        {
            int result = 0;
            if (feats[Feat.Pot_Fiend] && HP < 600) result -= ((int)(Stats[(int)Stat.Atk] * .2));
            result += feats[Feat.Deadly_Precision] ? 1 : 0;
            result += feats[Feat.Feral_Assailant] ? 2 : 0;
            result += feats[Feat.Disciple_Of_War] ? 3 : 0;
            return result;
        }
        public int getBonusDef()
        {
            int result = 0;
            result += feats[Feat.Man_Of_Steel] ? 3 : 0;
            result += feats[Feat.Show_No_Weakness] ? 1 : 0;
            result += feats[Feat.Tough_Skinned] ? 3 : 0;
            return result;
        }
        public int getBonusSpd()
        {
            int result = 0;
            if (feats[Feat.Pot_Fiend] && HP < 200) result -= ((int)(Stats[(int)Stat.Spd] * .2));
            result += feats[Feat.Master_Thief] ? 1 : 0;
            result += feats[Feat.Mobile] ? 4 : 0;
            return result;
        }
        public int getBonusVit()
        {
            int result = 0;
            if (feats[Feat.Pot_Fiend] && HP < 600) result -= ((int)(Stats[(int)Stat.Vit] * .2));
            result += feats[Feat.Arch_Magus] ? ((int)(Stats[(int)Stat.Vit] * -.05)) : 0;
            return result;
        }
        public int getBonusWis()
        {
            int result = 0;
            if (feats[Feat.Pot_Fiend] && HP < 200) result -= ((int)(Stats[(int)Stat.Wis] * .2));
            result += feats[Feat.Unfazed] ? 1 : 0;
            result += feats[Feat.Prodigious_Mind] ? 1 : 0;
            result += feats[Feat.Accelerated_Casting] ? 1 : 0;
            result += feats[Feat.Arcane_Adept] ? 2 : 0;
            result += feats[Feat.Arch_Magus] ? ((int)(Stats[(int)Stat.Wis] * .07)) : 0;
            return result;
        }
        public int getBonusDex()
        {
            int result = 0;
            if (feats[Feat.Pot_Fiend] && HP < 600) result -= ((int)(Stats[(int)Stat.Dex] * .2));
            result += feats[Feat.Great_Weapons_Master] ? 2 : 0;
            result += feats[Feat.Feral_Assailant] ? 1 : 0;
            result += feats[Feat.Master_Thief] ? 1 : 0;
            return result;
        }
        public double getLootBoost()
        {
            double result = 1;
            result += feats[Feat.High_Roller] ? .2 : 0;
            result += feats[Feat.Lucky] ? .05 : 0;
            result += feats[Feat.Blessed_By_The_Gods] ? .03 : 0;
            result += feats[Feat.Master_Thief] ? .05 : 0;
            return result;
        }
        public ProjectileDesc applyProjectileEffects(ProjectileDesc pd)
        {
            if (feats[Feat.Great_Weapons_Master] && this.Random.Next(1, 10) == 1)
            {

                pd.MinDamage = (int)(pd.MinDamage * 1.1);
                pd.MaxDamage = (int)(pd.MaxDamage * 1.1);
            }
            if (feats[Feat.Eagle_Eyed_Sentry] && this.Random.Next(1, 20) == 1)
            {
                pd.Effects = new ConditionEffect[pd.Effects.Length + 1];
                pd.Effects[pd.Effects.Length - 1] = new ConditionEffect
                {
                    DurationMS = 2000,
                    Effect = ConditionEffectIndex.Slowed
                };
            }
            if (feats[Feat.Hunters_Mark] && this.Random.Next(1, 20) == 1)
            {
                pd.Effects = new ConditionEffect[pd.Effects.Length + 1];
                pd.Effects[pd.Effects.Length - 1] = new ConditionEffect
                {
                    DurationMS = 3000,
                    Effect = ConditionEffectIndex.Marked
                };
            }
            if (HasConditionEffect(ConditionEffectIndex.Inspired))
            {
                pd.LifetimeMS = (int)(pd.LifetimeMS *1.25);
            }
            return pd;
        }
        internal class StatBoost
        {
            public int type;
            public int amount;
            public int duration;
            public int stat;
            public StatBoost(int type, int amount, int duration, int stat)
            {
                this.type = type;
                this.amount = amount;
                this.duration = duration;
                this.stat = stat;
            }
        }
        public int applyOnHitEffects(int damage)
        {
            if (feats[Feat.Combat_Caster])
            {
                if (Random.Next(1, 5) == 1)
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.AttBoost,
                            DurationMS=3000
                        },
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.WisBoost,
                            DurationMS=3000
                        }
                        });
                    addStatBoost(3000, Stat.Atk, 0.1, 1);
                    addStatBoost(3000, Stat.Wis, 0.1, 2);
                }
            }
            if (feats[Feat.Raging_Vitality])
            {
                if (Random.Next(1, 5) == 1)
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.VitBoost,
                            DurationMS=3000
                        }
                        });
                    addStatBoost(3000, Stat.Vit, 0.15, 3);
                }
            }
            if (feats[Feat.Bolstered_Resilience])
            {
                if (Random.Next(1, 5) == 1)
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.DefBoost,
                            DurationMS=3000
                        }
                        });
                    addStatBoost(3000, Stat.Def, 0.15, 4);
                }
            }
            if (feats[Feat.Adrenaline_Rush])
            {
                if (Random.Next(1, 5) == 1)
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.SpdBoost,
                            DurationMS=3000
                        },
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.DexBoost,
                            DurationMS=3000
                        }
                        });
                    addStatBoost(3000, Stat.Spd, .1, 5);
                    addStatBoost(3000, Stat.Dex, .1, 6);
                }
            }
            if (feats[Feat.Full_Counter])
            {
                if (Random.Next(1, 20) == 1)
                {
                    Client.SendPacket(new EffectTextPacket { Message = "Full Counter" });
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Damaging,
                            DurationMS=3000
                        }
                        });
                }
            }
            if (feats[Feat.Indomitable])
            {
                if (HP < 600)
                {
                    if (subclassCooldowns[0] <= 0)
                    {
                        Client.SendPacket(new EffectTextPacket { Message = "Indomitable" });
                        ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Armored,
                            DurationMS=3000
                        }
                        });
                        subclassCooldowns[0] = 10000;
                    }

                }

            }
            if (feats[Feat.Deny_Death])
            {
                if (HP < 300)
                {
                    if (subclassCooldowns[1] <= 0)
                    {
                        Client.SendPacket(new EffectTextPacket { Message = "Deny Death" });
                        ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Healing,
                            DurationMS=3000
                        }
                        });
                        subclassCooldowns[1] = 10000;
                    }
                }
            }
            if (feats[Feat.Craven_Hearted])
            {
                if (HP < 500)
                {
                    if (subclassCooldowns[2] <= 0)
                    {
                        Client.SendPacket(new EffectTextPacket { Message = "Craven Hearted" });
                        ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Speedy,
                            DurationMS=3000
                        }
                        });
                        subclassCooldowns[2] = 10000;
                    }
                }
            }
            if (feats[Feat.Slippery])
            {
                if (Random.Next(1, 20) == 1)
                {
                    Client.SendPacket(new EffectTextPacket { Message = "Slippery" });
                    return 0;
                }
            }
            if (feats[Feat.One_Foot_In_The_Grave])
            {
                if (HP < 100)
                {
                    if (subclassCooldowns[4] <= 0)
                    {
                        Client.SendPacket(new EffectTextPacket { Message = "One Foot in the Grave" });
                        ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Invulnerable,
                            DurationMS=1500
                        }
                        });
                        ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Invisible,
                            DurationMS=3000
                        }
                        });
                        subclassCooldowns[4] = 10000;
                    }
                }
            }
            if (feats[Feat.Heros_Last_Stand])
            {
                if (HP < 200)
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect{
                            Effect=ConditionEffectIndex.AttBoost,
                            DurationMS=3000
                        }
                    });
                    addStatBoost(3000, Stat.Atk, 0.3, 7);
                }
            }

            return damage;

        }
        private void addStatBoost(int duration, Stat stat, double percentChange, int type)
        {
            int amount = (int)(percentChange * Stats[(int)stat]);
            StatBoost newStatBoost = new StatBoost(type, amount, duration, (int)stat);
            for (int i = 0; i < StatBoosts.Count; i++)
            {
                if (StatBoosts[i].type == type)
                {
                    Stats[(int)stat] -= StatBoosts[i].amount;
                    Stats[(int)stat] += amount;
                    StatBoosts[i] = newStatBoost;
                    return;
                }
            }
            Stats[(int)stat] += amount;
            StatBoosts.Add(newStatBoost);
        }
        public void HandleSubclassEffects(RealmTime time)
        {
            int count = StatBoosts.Count;
            int loc = 0;
            for (int i = 0; i < count; i++)
            {
                if (StatBoosts[loc].duration == -1)
                {
                    loc++;
                    continue;
                }
                int duration = StatBoosts[loc].duration - time.thisTickTimes;
                if (duration <= 0)
                {
                    Stats[StatBoosts[loc].stat] -= StatBoosts[loc].amount;
                    StatBoosts.RemoveAt(loc);

                }
                else
                {
                    StatBoosts[loc].duration = duration;
                    loc++;
                }
                i++;
            }

            for (int i = 0; i < subclassCooldowns.Length; i++)
            {
                subclassCooldowns[i] = Math.Max(subclassCooldowns[i] - time.thisTickTimes, 0);
            }


        }
        public bool canApply(ConditionEffect eff)
        {
            switch (eff.Effect)
            {
                case ConditionEffectIndex.Weak: return feats[Feat.Show_No_Weakness];
                case ConditionEffectIndex.Darkness: return feats[Feat.Night_Vision];
                case ConditionEffectIndex.Unstable: return feats[Feat.Deadly_Precision];
                case ConditionEffectIndex.Silenced: return feats[Feat.Unfazed];
                case ConditionEffectIndex.Drunk:
                    if (feats[Feat.Drunken_Brawler])
                    {
                        addStatBoost(eff.DurationMS, Stat.Atk, 0.1, 8);
                        ApplyConditionEffect(new ConditionEffect[]{ new ConditionEffect()
                        {
                            Effect = ConditionEffectIndex.AttBoost,
                            DurationMS = eff.DurationMS
                        }});
                    }
                    return true;
                case ConditionEffectIndex.Slowed:
                    if (feats[Feat.Mammoth_Charge])
                    {
                        addStatBoost(eff.DurationMS, Stat.Spd, 0.2, 9);
                        ApplyConditionEffect(new ConditionEffect[]{ new ConditionEffect()
                        {
                            Effect = ConditionEffectIndex.SpdBoost,
                            DurationMS = eff.DurationMS
                        }});
                        addStatBoost(eff.DurationMS, Stat.Atk, 0.2, 10);
                        ApplyConditionEffect(new ConditionEffect[]{ new ConditionEffect()
                        {
                            Effect = ConditionEffectIndex.AttBoost,
                            DurationMS = eff.DurationMS
                        }});
                        addStatBoost(eff.DurationMS, Stat.Def, -0.1, 11);
                        ApplyConditionEffect(new ConditionEffect[]{ new ConditionEffect()
                        {
                            Effect = ConditionEffectIndex.Def_debuff,
                            DurationMS = eff.DurationMS
                        }});
                    }
                    return true;

                default: return true;
            }
        }
        public void onAbilityUse()
        {
            if (feats[Feat.Mage_Aura])
            {
                if (Random.Next(1, 20) == 1)
                {
                    if (subclassCooldowns[3] <= 0)
                    {
                        this.Aoe(5, true, player =>
                        {
                            if (player is Player && (player as Player).subclass == 1) player.ApplyConditionEffect(new ConditionEffect[]{
                            new ConditionEffect
                            {
                            Effect=ConditionEffectIndex.Energized,
                            DurationMS=3000
                            }
                            });
                        });
                        List<Packet> pkts = new List<Packet>();
                        pkts.Add(new ShowEffectPacket
                        {
                            EffectType = EffectType.AreaBlast,
                            TargetId = Id,
                            Color = new ARGB(0x0000000ff),
                            PosA = new Position { X = 5 }
                        });
                        BroadcastSync(pkts, p => this.Dist(p) < 25);

                    }
                    subclassCooldowns[3] = 5000;
                }
            }
            if (feats[Feat.Mage_Aura])
            {
                if (Random.Next(1, 20) == 1)
                {
                    if (subclassCooldowns[5] <= 0)
                    {
                        var amountHN = 100;
                        var rangeHN = 3.5f;
                        List<Packet> pkts = new List<Packet>();
                        this.Aoe(rangeHN, true, p => { if ((p as Player).subclass == 1) ActivateHealHp(p as Player, amountHN, pkts); });
                        pkts.Add(new ShowEffectPacket
                        {
                            EffectType = EffectType.AreaBlast,
                            TargetId = Id,
                            Color = new ARGB(0xffffffff),
                            PosA = new Position { X = rangeHN }
                        });
                        BroadcastSync(pkts, p => this.Dist(p) < 25);
                    }
                }

            }
        }
        public bool affectBullet(Projectile p)
        {
            if (feats[Feat.Man_Of_Steel])
            {
                //p.Destroy(false);
                return true;
            }
            return false;
        }
        public void removeBuffs()
        {
            for (int i = 0; i < StatBoosts.Count; i++)
            {
                StatBoost temp = StatBoosts[i];
                Stats[temp.stat] -= temp.amount;
            }
            StatBoosts.Clear();
        }
        public bool onDeath()
        {
            if (feats[Feat.Second_Chance])
            {
                if (Random.Next(1, 4) == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool allowTP()
        {
            return feats[Feat.Greater_Teleportation];
        }
        public void usePot(int type)
        {
            if (type == 0)
            {
                if (feats[Feat.Pot_Fiend])
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Damaging,
                            DurationMS=3000
                        },
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Berserk,
                            DurationMS=3000
                        }
                    });
                }

            }
            if (type == 1)
            {
                if (feats[Feat.Pot_Fiend])
                {
                    ApplyConditionEffect(new ConditionEffect[]{
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Energized,
                            DurationMS=3000
                        },
                        new ConditionEffect
                        {
                            Effect=ConditionEffectIndex.Speedy,
                            DurationMS=3000
                        }
                    });
                }

            }
        }
    }

}
    


