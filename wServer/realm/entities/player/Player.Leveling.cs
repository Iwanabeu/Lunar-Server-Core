#region

using System;
using System.Collections.Generic;
using System.Linq;
using wServer.networking.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private static readonly Dictionary<string, Tuple<int, int, int>> questDat =
            new Dictionary<string, Tuple<int, int, int>> //Priority, Min, Max
            {
                {"Scorpion Queen", Tuple.Create(1, 1, 6)},
                {"Bandit Leader", Tuple.Create(1, 1, 6)},
                {"Hobbit Mage", Tuple.Create(3, 3, 8)},
                {"Undead Hobbit Mage", Tuple.Create(3, 3, 8)},
                {"Giant Crab", Tuple.Create(3, 3, 8)},
                {"Desert Werewolf", Tuple.Create(3, 3, 8)},
                {"Sandsman King", Tuple.Create(4, 4, 9)},
                {"Goblin Mage", Tuple.Create(4, 4, 9)},
                {"Elf Wizard", Tuple.Create(4, 4, 9)},
                {"Dwarf King", Tuple.Create(5, 5, 10)},
                {"Swarm", Tuple.Create(6, 6, 11)},
                {"Shambling Sludge", Tuple.Create(6, 6, 11)},
                {"Great Lizard", Tuple.Create(7, 7, 12)},
                {"Wasp Queen", Tuple.Create(8, 7, 1000)},
                {"Horned Drake", Tuple.Create(8, 7, 1000)},
                {"Deathmage", Tuple.Create(5, 6, 11)},
                {"Great Coil Snake", Tuple.Create(6, 6, 12)},
                {"Lich", Tuple.Create(9, 6, 1000)},
                {"Actual Lich", Tuple.Create(9, 7, 1000)},
                {"Ent Ancient", Tuple.Create(10, 7, 1000)},
                {"Actual Ent Ancient", Tuple.Create(10, 7, 1000)},
                {"Oasis Giant", Tuple.Create(11, 8, 1000)},
                {"Phoenix Lord", Tuple.Create(11, 9, 1000)},
                {"Ghost King", Tuple.Create(12, 10, 1000)},
                {"Actual Ghost King", Tuple.Create(12, 10, 1000)},
                {"Cyclops God", Tuple.Create(13, 10, 1000)},
                {"Red Demon", Tuple.Create(15, 15, 1000)},
                {"Lucky Djinn", Tuple.Create(15, 15, 1000)},
                {"Lucky Ent", Tuple.Create(15, 15, 1000)},
                {"Skull Shrine", Tuple.Create(16, 15, 1000)},
                {"Pentaract", Tuple.Create(16, 15, 1000)},
                {"Cube God", Tuple.Create(16, 15, 1000)},
                {"Grand Sphinx", Tuple.Create(16, 15, 1000)},
                {"Lord of the Lost Lands", Tuple.Create(16, 15, 1000)},
                {"Hermit God", Tuple.Create(16, 15, 1000)},
                {"Ghost Ship", Tuple.Create(16, 15, 1000)},
                {"Unknown Giant Golem", Tuple.Create(16, 15, 1000)},
                {"Evil Chicken God", Tuple.Create(20, 1, 1000)},
                {"Bonegrind The Butcher", Tuple.Create(20, 1, 1000)},
                {"Dreadstump the Pirate King", Tuple.Create(20, 1, 1000)},
                {"Arachna the Spider Queen", Tuple.Create(20, 1, 1000)},
                {"Stheno the Snake Queen", Tuple.Create(20, 1, 1000)},
                {"Mixcoatl the Masked God", Tuple.Create(20, 1, 1000)},
                {"Limon the Sprite God", Tuple.Create(20, 1, 1000)},
                {"Septavius the Ghost God", Tuple.Create(20, 1, 1000)},
                {"Davy Jones", Tuple.Create(20, 1, 1000)},
                {"Lord Ruthven", Tuple.Create(20, 1, 1000)},
                {"Archdemon Malphas", Tuple.Create(20, 1, 1000)},
                {"Elder Tree", Tuple.Create(20, 1, 1000)},
                {"Thessal the Mermaid Goddess", Tuple.Create(20, 1, 1000)},
                {"Dr. Terrible", Tuple.Create(20, 1, 1000)},
                {"Horrific Creation", Tuple.Create(20, 1, 1000)},
                {"Masked Party God", Tuple.Create(20, 1, 10000)},
                {"Stone Guardian Left", Tuple.Create(20, 1, 1000)},
                {"Stone Guardian Right", Tuple.Create(20, 1, 1000)},
                {"Oryx the Mad God 1", Tuple.Create(20, 1, 1000)},
                {"Oryx the Mad God 2", Tuple.Create(20, 1, 1000)},
            };

        public Entity Quest { get; private set; }

        private static int GetExpGoal(int level)
        {
            if(level<=20) return 50 + (level - 1)*100;
            //From levels 21-50 manual xp requirements.
            switch (level) {
                case 21:
                    return 2466;
                case 22:
                    return 3288;
                case 23:
                    return 4384;
                case 24:
                    return 5845;
                case 25:
                    return 7793;
                case 26:
                    return 10390;
                case 27:
                    return 13853;
                case 28:
                    return 18470;
                case 29:
                    return 24626;
                case 30:
                    return 32834;
                case 31:
                    return 43778;
                case 32:
                    return 58370;
                case 33:
                    return 77826;
                case 34:
                    return 103768;
                case 35:
                    return 138347;
                case 36:
                    return 184462;
                case 37:
                    return 245949;
                case 38:
                    return 327932;
                case 39:
                    return 437242;
                case 40:
                    return 582989;
                case 41:
                    return 777318;
                case 42:
                    return 1036424;
                case 43:
                    return 1381898;
                case 44:
                    return 1842530;
                case 45:
                    return 2456706;
                case 46:
                    return 3275608;
                case 47:
                    return 4367477;
                case 48:
                    return 5823302;
                case 49:
                    return 7314402;
                case 50:
                    return 9752536;
                default:
                    return 99999999;
            }
        }

        public static int GetLevelExp(int level)
        {
            if (level == 1) return 0;
            if(level<=20) return 50*(level - 1) + (level - 2)*(level - 1)*50;
            //From levels 21-50 manual xp summation.
            switch (level) {
                case 21:
                    return 20505;
                case 22:
                    return 23793;
                case 23:
                    return 28177;
                case 24:
                    return 34022;
                case 25:
                    return 41815;
                case 26:
                    return 52205;
                case 27:
                    return 66058;
                case 28:
                    return 84528;
                case 29:
                    return 109154;
                case 30:
                    return 141988;
                case 31:
                    return 185766;
                case 32:
                    return 244136;
                case 33:
                    return 321962;
                case 34:
                    return 425730;
                case 35:
                    return 564077;
                case 36:
                    return 748539;
                case 37:
                    return 994488;
                case 38:
                    return 1322420;
                case 39:
                    return 1759662;
                case 40:
                    return 2342651;
                case 41:
                    return 3119969;
                case 42:
                    return 4156393;
                case 43:
                    return 5538291;
                case 44:
                    return 7380821;
                case 45:
                    return 9837527;
                case 46:
                    return 13113135;
                case 47:
                    return 17480612;
                case 48:
                    return 23303914;
                case 49:
                    return 30618316;
                case 50:
                    return 40370852;
                default:
                    return 0;
            }
        }

        private static int GetFameGoal(int fame)
        {
            if (fame >= 2000) return 0;
            if (fame >= 800) return 2000;
            if (fame >= 400) return 800;
            if (fame >= 150) return 400;
            return fame >= 20 ? 150 : 0;
        }

        public int GetStars()
        {
            var ret = 0;
            foreach (var i in Client.Account.Stats.ClassStates)
            {
                if (i.BestFame >= 2000) ret += 5;
                else if (i.BestFame >= 800) ret += 4;
                else if (i.BestFame >= 400) ret += 3;
                else if (i.BestFame >= 150) ret += 2;
                else if (i.BestFame >= 20) ret += 1;
            }
            return ret;
        }

        private static float Dist(Entity a, Entity b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return (float) Math.Sqrt(dx*dx + dy*dy);
        }

        private Entity FindQuest()
        {
            Entity ret = null;
            try
            {
                float bestScore = 0;
                foreach (var i in Owner.Quests.Values
                    .OrderBy(quest => MathsUtils.DistSqr(quest.X, quest.Y, X, Y)).Where(i => i.ObjectDesc != null && i.ObjectDesc.Quest))
                {
                    Tuple<int, int, int> x;
                    if (!questDat.TryGetValue(i.ObjectDesc.ObjectId, out x)) continue;

                    if ((Level < x.Item2 || Level > x.Item3)) continue;
                    var score = (20 - Math.Abs((i.ObjectDesc.Level ?? 0) - Level))*x.Item1 -
                                //priority * level diff
                                Dist(this, i)/100; //minus 1 for every 100 tile distance
                    if (score < 0)
                        score = 1;
                    if (!(score > bestScore)) continue;
                    bestScore = score;
                    ret = i;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return ret;
        }

        private void HandleQuest(RealmTime time)
        {
            if (time.tickCount%500 != 0 && Quest?.Owner != null) return;
            var newQuest = FindQuest();
            if (newQuest == null || newQuest == Quest) return;
            Owner.Timers.Add(new WorldTimer(100, (w, t) =>
            {
                Client.SendPacket(new QuestObjIdPacket
                {
                    ObjectId = newQuest.Id
                });
            }));
            Quest = newQuest;
        }

        private void CalculateFame()
        {
            int newFame;
            if (Experience < 200*1000) newFame = Experience/1000;
            else newFame = 200 + (Experience - 200*1000)/1000;
            if (newFame == Fame) return;
            Fame = newFame;
            int newGoal;
            var state =
                Client.Account.Stats.ClassStates.SingleOrDefault(_ => Utils.FromString(_.ObjectType) == ObjectType);
            if (state != null && state.BestFame > Fame)
                newGoal = GetFameGoal(state.BestFame);
            else
                newGoal = GetFameGoal(Fame);
            if (newGoal > FameGoal)
            {
                Owner.BroadcastPacket(new NotificationPacket
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFF00FF00),
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Class Quest Complete!\"}}",
                }, null);
                Stars = GetStars();
            }
            FameGoal = newGoal;
            UpdateCount++;
        }

        private bool CheckLevelUp()
        {
            if (Experience - GetLevelExp(Level) >= ExperienceGoal && Level < 51)
            {
                Level++;
                ExperienceGoal = GetExpGoal(Level);
                if (Level <= 20)
                {
                    foreach (var i in Manager.GameData.ObjectTypeToElement[ObjectType].Elements("LevelIncrease"))
                    {
                        var rand = new Random();
                        var min = int.Parse(i.Attribute("min").Value);
                        var max = int.Parse(i.Attribute("max").Value) + 1;
                        var xElement = Manager.GameData.ObjectTypeToElement[ObjectType].Element(i.Value);
                        if (xElement == null) continue;
                        var limit =
                            int.Parse(
                                xElement.Attribute("max").Value);
                        var idx = StatsManager.StatsNameToIndex(i.Value);
                        Stats[idx] += rand.Next(min, max);
                        if (Stats[idx] > limit) Stats[idx] = limit;
                    }
                }
                else
                {
                    foreach (var i in Manager.GameData.ObjectTypeToElement[ObjectType].Elements("LevelIncrease"))
                    {
                        
                        var xElement = Manager.GameData.ObjectTypeToElement[ObjectType].Element(i.Value);
                        if (xElement == null) continue;
                        
                        var idx = StatsManager.StatsNameToIndex(i.Value);
                        Stats[idx] += 1;
                    }
                }
                HP = Stats[0] + Boost[0];
                Mp = Stats[1] + Boost[1];

                UpdateCount++;

                if (Level == 20)
                {
                    foreach (var i in Owner.Players.Values)
                        i.SendInfo(Name + " achieved level 20");
                    XpBoosted = false;
                    XpBoostTimeLeft = 0;
                }
                if (Level == 51)
                {
                    foreach (var i in Manager.Clients.Values)
                        i.Player.SendInfo(Name + " has achieved Godhood");
                    XpBoosted = false;
                    XpBoostTimeLeft = 0;
                }
                Quest = null;
                return true;
            }
            CalculateFame();
            return false;
        }

        public bool EnemyKilled(Enemy enemy, int exp, bool killer)
        {
            if (enemy == Quest)
                Owner.BroadcastPacket(new NotificationPacket
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFF00FF00),
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Quest Complete!\"}}",
                }, null);
            if (exp > 0)
            {
                if(XpBoosted)
                    Experience += exp * 2;
                else
                    Experience += exp;
                UpdateCount++;
                foreach (var i in Owner.PlayersCollision.HitTest(X, Y, 16).Where(i => i != this).OfType<Player>())
                {
                    try
                    {
                        i.Experience += i.XpBoosted ? exp * 2 : exp;
                        i.UpdateCount++;
                        i.CheckLevelUp();
                        if (Random.Next(1, 100000) <= 50)
                            Client.GiftCodeReceived("LevelUp");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
            FameCounter.Killed(enemy, killer);
            return CheckLevelUp();
        }
    }
}