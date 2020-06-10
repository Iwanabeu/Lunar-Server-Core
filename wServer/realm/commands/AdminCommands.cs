#region

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.setpieces;
using wServer.realm.worlds;
using System.Collections.Concurrent;

#endregion

namespace wServer.realm.commands
{
    internal class TestCommand : Command
    {
        public TestCommand()
            : base("t", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Entity en = Entity.Resolve(player.Manager, "Zombie Wizard");
            en.Move(player.X, player.Y);
            player.Owner.EnterWorld(en);
            player.UpdateCount++;
            //player.Client.SendPacket(new DeathPacket
            //{
            //    AccountId = player.AccountId,
            //    CharId = player.Client.Character.CharacterId,
            //    Killer = "mountains.beholder",
            //    obf0 = 10000,
            //    obf1 = 10000
            //});
            return true;
        }
    }

    internal class AddGiftCodeCommand : Command
    {
        public AddGiftCodeCommand()
            : base("gcode", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(args[0]))
                    player.Manager.FindPlayer(args[0])?.Client.GiftCodeReceived("LevelUp");
                else
                    player.Client.GiftCodeReceived("LevelUp");
            }
            catch (Exception) { }
            return true;
        }
    }

    internal class posCmd : Command
    {
        public posCmd()
            : base("pos", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo("X: " + (int)player.X + " - Y: " + (int)player.Y);
            return true;
        }

    }

    internal class BanCommand : Command
    {
        public BanCommand() :
            base("ban", permLevel: 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Player not found");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE accounts SET banned=1 WHERE id=@accId;";
                cmd.Parameters.AddWithValue("@accId", p.AccountId);
                cmd.ExecuteNonQuery();
            });
            return true;
        }
    }


    internal class AddWorldCommand : Command
    {
        public AddWorldCommand()
            : base("addworld", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Task.Factory.StartNew(() => GameWorld.AutoName(1, true)).ContinueWith(_ => player.Manager.AddWorld(_.Result), TaskScheduler.Default);
            return true;
        }
    }


    internal class SpawnCommand : Command
    {
        public SpawnCommand()
            : base("spawn", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            int num;
            if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    player.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType) ||
                    !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }
                int c = int.Parse(args[0]);
                if (!(player.Client.Account.Rank > 2) && c > 100)
                {
                    player.SendError("Maximum spawn count is set to 100!");
                    return false;
                }
                if (player.Client.Account.Rank > 2 && c > 100)
                {
                    player.SendInfo("Not for you.");
                    return false;
                }
                for (int i = 0; i < num; i++)
                {
                    Entity entity = Entity.Resolve(player.Manager, objType);
                    entity.Move(player.X, player.Y);
                    player.Owner.EnterWorld(entity);
                }
                player.SendInfo("Success!");
            }
            else
            {
                string name = string.Join(" ", args);
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    player.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType) ||
                    !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }
                Entity entity = Entity.Resolve(player.Manager, objType);
                entity.Move(player.X, player.Y);
                player.Owner.EnterWorld(entity);
            }
            return true;
        }
    }

    internal class AddEffCommand : Command
    {
        public AddEffCommand()
            : base("addeff", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /addeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = -1
                });
                {
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class RemoveEffCommand : Command
    {
        public RemoveEffCommand()
            : base("remeff", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /remeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = 0
                });
                player.SendInfo("Success!");
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class GiveCommand : Command
    {
        public GiveCommand()
            : base("give", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give <Itemname>");
                return false;
            }
            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Player not found");
                return false;
            }
            else
            {
                string[] nameparts = new string[args.Length - 1];
                Array.ConstrainedCopy(args, 1, nameparts, 0, args.Length - 1);
                string name = string.Join(" ", nameparts).Trim();
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType))
                {
                    player.SendError("Unknown type!");
                    return false;
                }
                Item item;
                if (p.Manager.GameData.Items.TryGetValue(objType, out item) && (!p.Manager.GameData.Items[objType].Secret || p.Client.Account.Rank >= 4))
                {
                    for (int i = 4; i < p.Inventory.Length; i++)
                        if (p.Inventory[i] == null)
                        {
                            p.Inventory[i] = item;
                            p.UpdateCount++;
                            p.SaveToCharacter();
                            player.SendInfo("Success!");
                            break;
                        }
                }
                else
                {
                    player.SendError("Item cannot be given!");
                    return false;
                }
                return true;
            }

        }
    }

    internal class TpCommand : Command
    {
        public TpCommand()
            : base("tp", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return false;
                }
                player.Move(x + 0.5f, y + 0.5f);
                if (player.Pet != null)
                    player.Pet.Move(x + 0.5f, y + 0.5f);
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            return true;
        }
    }

    class KillAll : Command
    {
        public KillAll() : base("killAll", permLevel: 3) { }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var iterations = 0;
            var lastKilled = -1;
            var killed = 0;

            var mobName = args.Aggregate((s, a) => string.Concat(s, " ", a));
            while (killed != lastKilled)
            {
                lastKilled = killed;
                foreach (var i in player.Owner.Enemies.Values.Where(e =>
                    e.ObjectDesc?.ObjectId != null && e.ObjectDesc.ObjectId.ContainsIgnoreCase(mobName)))
                {
                    i.Death(time);
                    killed++;
                }
                if (++iterations >= 5)
                    break;
            }

            player.SendInfo($"{killed} enemy killed!");
            return true;
        }
    }

    internal class Kick : Command
    {
        public Kick()
            : base("kick", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.SendInfo("Player Disconnected");
                        i.Value.Client.Disconnect();
                    }
                }
            }
            catch
            {
                player.SendError("Cannot kick!");
                return false;
            }
            return true;
        }
    }

    internal class Mute : Command
    {
        public Mute()
            : base("mute", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /mute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.MuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Muted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot mute!");
                return false;
            }
            return true;
        }
    }

    internal class Max : Command
    {
        public Max()
            : base("max", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                var p = player.Manager.FindPlayer(args[0]);
                if (p == null)
                {
                    player.Stats[0] = player.ObjectDesc.MaxHitPoints;
                    player.Stats[1] = player.ObjectDesc.MaxMagicPoints;
                    player.Stats[2] = player.ObjectDesc.MaxAttack;
                    player.Stats[3] = player.ObjectDesc.MaxDefense;
                    player.Stats[4] = player.ObjectDesc.MaxSpeed;
                    player.Stats[5] = player.ObjectDesc.MaxHpRegen;
                    player.Stats[6] = player.ObjectDesc.MaxMpRegen;
                    player.Stats[7] = player.ObjectDesc.MaxDexterity;
                    player.SaveToCharacter();
                    player.Client.Save();
                    player.UpdateCount++;
                    player.SendInfo("Success");
                }
                else
                {
                    p.Stats[0] = player.ObjectDesc.MaxHitPoints;
                    p.Stats[1] = player.ObjectDesc.MaxMagicPoints;
                    p.Stats[2] = player.ObjectDesc.MaxAttack;
                    p.Stats[3] = player.ObjectDesc.MaxDefense;
                    p.Stats[4] = player.ObjectDesc.MaxSpeed;
                    p.Stats[5] = player.ObjectDesc.MaxHpRegen;
                    p.Stats[6] = player.ObjectDesc.MaxMpRegen;
                    p.Stats[7] = player.ObjectDesc.MaxDexterity;
                    p.SaveToCharacter();
                    p.Client.Save();
                    p.UpdateCount++;
                    player.SendInfo("Success");
                }
            }
            catch
            {
                player.SendError("Error while maxing stats");
                return false;
            }
            return true;
        }
    }

    internal class UnMute : Command
    {
        public UnMute()
            : base("unmute", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unmute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.UnmuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Unmuted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot unmute!");
                return false;
            }
            return true;
        }
    }

    internal class OryxSay : Command
    {
        public OryxSay()
            : base("osay", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);
            player.SendEnemy("Oryx the Mad God", saytext);
            return true;
        }
    }

    internal class SWhoCommand : Command //get all players from all worlds (this may become too large!)
    {
        public SWhoCommand()
            : base("swho", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("All conplayers: ");

            foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    Player[] copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                        }
                    }
                }
            }
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString);
            return true;
        }
    }

    internal class Announcement : Command
    {
        public Announcement()
            : base("announce", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);

            foreach (Client i in player.Manager.Clients.Values)
            {
                i.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = " " + saytext
                });
            }
            return true;
        }
    }

    internal class Summon : Command
    {
        public Summon()
            : base("summon", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault || player.Owner is PetYard)
            {
                player.SendInfo("You cant summon in this world.");
                return false;
            }
            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
            {
                if (i.Value.Player.Name.EqualsIgnoreCase(args[0]))
                {
                    Packet pkt;
                    if (i.Value.Player.Owner == player.Owner)
                    {
                        i.Value.Player.Move(player.X, player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = i.Value.Player.Id,
                            Position = new Position(player.X, player.Y)
                        };
                        i.Value.Player.UpdateCount++;
                        player.SendInfo("Player summoned!");
                    }
                    else
                    {
                        pkt = new ReconnectPacket
                        {
                            GameId = player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = player.Owner.PortalKey,
                            KeyTime = -1,
                            Name = player.Owner.Name,
                            Port = -1
                        };
                        player.SendInfo("Player will connect to you now!");
                    }

                    i.Value.SendPacket(pkt);

                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class KillPlayerCommand : Command
    {
        public KillPlayerCommand()
            : base("killplayer", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (Client i in player.Manager.Clients.Values)
            {
                if (i.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    i.Player.HP = 0;
                    i.Player.Death("Admin");
                    player.SendInfo("Player killed!");
                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class KillSpecificCommand : Command
    {
        public KillSpecificCommand()
           : base("kill", 3)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            string name = string.Join(" ", args.ToArray());
            int numEnemies = 0;
            foreach (Enemy i in player.Owner.Enemies.Values)
            {
                if (i.Name.ToLower() == name.ToLower())
                {
                    i.Death(time);
                    numEnemies++;
                }

            }
            if (numEnemies == 0)
            {
                player.SendError("Error. could not find enemy: " + name + ".");
                return false;
            }
            else
            {
                player.SendInfo("Success! Killed " + numEnemies + " Enemies.");
                return true;
            }
        }
    }
    internal class RestartCommand : Command
    {
        public RestartCommand()
            : base("restart", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        world.BroadcastPacket(new TextPacket
                        {
                            Name = "@ANNOUNCEMENT",
                            Stars = -1,
                            BubbleTime = 0,
                            Text =
                                "Server restarting soon. Please be ready to disconnect. Estimated server down time: 30 Seconds - 1 Minute"
                        }, null);
                    }
                }
            }
            catch
            {
                player.SendError("Cannot say that in announcement!");
                return false;
            }
            return true;
        }
    }
    // adding commands


    //class VitalityCommand : ICommand
    //{
    //    public string Command { get { return "vit"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /vit <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[5] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DefenseCommand : ICommand
    //{
    //    public string Command { get { return "def"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /def <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[3] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class AttackCommand : ICommand
    //{
    //    public string Command { get { return "att"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /att <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[2] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DexterityCommand : ICommand
    //{
    //    public string Command { get { return "dex"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /dex <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[7] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class LifeCommand : ICommand
    //{
    //    public string Command { get { return "hp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /hp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[0] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class ManaCommand : ICommand
    //{
    //    public string Command { get { return "mp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /mp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[1] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class SpeedCommand : ICommand
    //{
    //    public string Command { get { return "spd"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[4] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class WisdomCommand : ICommand
    //{
    //    public string Command { get { return "wis"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[6] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class Ban : ICommand
    //{
    //    public string Command { get { return "ban"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /ban <username>");
    //        }
    //        try
    //        {
    //            using (Database dbx = new Database())
    //            {
    //                var cmd = dbx.CreateQuery();
    //                cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
    //                cmd.Parameters.AddWithValue("@name", args[0]);
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    player.SendInfo("Could not ban");
    //                }
    //                else
    //                {
    //                    foreach (var i in player.Owner.Players)
    //                    {
    //                        if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
    //                        {
    //                            i.Value.Client.Disconnect();
    //                            player.SendInfo("Account successfully Banned");
    //                            log.InfoFormat(args[0] + " was Banned.");
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            player.SendInfo("Could not ban");
    //        }
    //    }
    //}

    //class UnBan : ICommand
    //{
    //    public string Command { get { return "unban"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /unban <username>");
    //        }
    //        try
    //        {
    //            using (Database dbx = new Database())
    //            {
    //                var cmd = dbx.CreateQuery();
    //                cmd.CommandText = "UPDATE accounts SET banned=0, rank=1 WHERE name=@name";
    //                cmd.Parameters.AddWithValue("@name", args[0]);
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    player.SendInfo("Could not unban");
    //                }
    //                else
    //                {
    //                    player.SendInfo("Account successfully Unbanned");
    //                    log.InfoFormat(args[1] + " was Unbanned.");

    //                }
    //            }
    //        }
    //        catch
    //        {
    //            player.SendInfo("Could not unban, please unban in database");
    //        }
    //    }
    //}

    //class Rank : ICommand
    //{
    //    public string Command { get { return "rank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /rank <username> <number>\n0: Player\n1: Donator\n2: Game Master\n3: Developer\n4: Head Developer\n5: Admin");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@rank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change rank");
    //                    }
    //                    else
    //                        player.SendInfo("Account rank successfully changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class GuildRank : ICommand
    //{
    //    public string Command { get { return "grank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /grank <username> <number>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guildRank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild rank. Use 10, 20, 30, 40, or 50 (invisible)");
    //                    }
    //                    else
    //                        player.SendInfo("Guild rank successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild rank has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class ChangeGuild : ICommand
    //{
    //    public string Command { get { return "setguild"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /setguild <username> <guild id>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guild=@guild WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guild", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild.");
    //                    }
    //                    else
    //                        player.SendInfo("Guild successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change guild, please change in database.                                Use /setguild <username> <guild id>");
    //            }
    //        }
    //    }
    //}

    internal class TqCommand : Command
    {
        public TqCommand()
            : base("tq", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Quest == null)
            {
                player.SendError("Player does not have a quest!");
                return false;
            }
            player.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            if (player.Pet != null)
                player.Pet.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            player.UpdateCount++;
            player.Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.Quest.X,
                    Y = player.Quest.Y
                }
            }, null);
            player.SendInfo("Success!");
            return true;
        }
    }

    class GodMode : Command
    {
        public GodMode()
            : base("god", 3)
        { }
        public string Command { get { return "god"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {

                if (player.HasConditionEffect(ConditionEffects.Invincible))
                {
                    player.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = ConditionEffectIndex.Invincible,
                        DurationMS = 0
                    });
                    player.SendInfo("Godmode Off");
                }
                else
                {

                    player.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = ConditionEffectIndex.Invincible,
                        DurationMS = -1
                    });
                    player.SendInfo("Godmode On");
                }
                return true;
            }
            else
            {
                if (p.HasConditionEffect(ConditionEffects.Invincible))
                {
                    p.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = ConditionEffectIndex.Invincible,
                        DurationMS = 0
                    });
                    player.SendInfo("Godmode Off");
                }
                else
                {

                    p.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = ConditionEffectIndex.Invincible,
                        DurationMS = -1
                    });
                    player.SendInfo("Godmode On");
                }
                return true;
            }
        }
    }

    internal class LevelCommand : Command
    {
        public LevelCommand()
            : base("level", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /level <amount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    int newLevel = int.Parse(args[0]);
                    if (newLevel > 50 || newLevel<1)
                    {
                        player.SendInfo("Level must be greater than 0 and less than 51.");
                        return false;
                    }
                    int xp = Player.GetLevelExp(newLevel);
                    player.Client.Character.Level = newLevel;
                    player.Client.Player.Level = newLevel;
                    player.Client.Player.Experience = xp;
                    player.Client.Character.Exp = xp;
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class SetCommand : Command
    {
        public SetCommand()
            : base("setStat", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    string stat = args[0].ToLower();
                    int amount = int.Parse(args[1]);
                    switch (stat)
                    {
                        case "health":
                        case "hp":
                            player.Stats[0] = amount;
                            break;
                        case "mana":
                        case "mp":
                            player.Stats[1] = amount;
                            break;
                        case "atk":
                        case "attack":
                            player.Stats[2] = amount;
                            break;
                        case "def":
                        case "defence":
                            player.Stats[3] = amount;
                            break;
                        case "spd":
                        case "speed":
                            player.Stats[4] = amount;
                            break;
                        case "vit":
                        case "vitality":
                            player.Stats[5] = amount;
                            break;
                        case "wis":
                        case "wisdom":
                            if (amount > 999) break;
                            player.Stats[6] = amount;
                            break;
                        case "dex":
                        case "dexterity":
                            player.Stats[7] = amount;
                            break;
                        default:
                            player.SendError("Invalid Stat");
                            player.SendHelp("Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity");
                            player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                            return false;
                    }
                    player.SaveToCharacter();
                    player.Client.Save();
                    player.UpdateCount++;
                    player.SendInfo("Success");
                }
                catch
                {
                    player.SendError("Error while setting stat");
                    return false;
                }
                return true;
            }
            else if (args.Length == 3)
            {
                foreach (Client i in player.Manager.Clients.Values)
                {
                    if (i.Account.Name.EqualsIgnoreCase(args[0]))
                    {
                        try
                        {
                            string stat = args[1].ToLower();
                            int amount = int.Parse(args[2]);
                            switch (stat)
                            {
                                case "health":
                                case "hp":
                                    i.Player.Stats[0] = amount;
                                    break;
                                case "mana":
                                case "mp":
                                    i.Player.Stats[1] = amount;
                                    break;
                                case "atk":
                                case "attack":
                                    i.Player.Stats[2] = amount;
                                    break;
                                case "def":
                                case "defence":
                                    i.Player.Stats[3] = amount;
                                    break;
                                case "spd":
                                case "speed":
                                    i.Player.Stats[4] = amount;
                                    break;
                                case "vit":
                                case "vitality":
                                    i.Player.Stats[5] = amount;
                                    break;
                                case "wis":
                                case "wisdom":
                                    i.Player.Stats[6] = amount;
                                    break;
                                case "dex":
                                case "dexterity":
                                    i.Player.Stats[7] = amount;
                                    break;
                                default:
                                    player.SendError("Invalid Stat");
                                    player.SendHelp("Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity");
                                    player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                                    return false;
                            }
                            i.Player.SaveToCharacter();
                            i.Player.Client.Save();
                            i.Player.UpdateCount++;
                            player.SendInfo("Success");
                        }
                        catch
                        {
                            player.SendError("Error while setting stat");
                            return false;
                        }
                        return true;
                    }
                }
                player.SendError(string.Format("Player '{0}' could not be found!", args));
                return false;
            }
            else
            {
                player.SendHelp("Usage: /setStat <Stat> <Amount>");
                player.SendHelp("or");
                player.SendHelp("Usage: /setStat <Player> <Stat> <Amount>");
                player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                return false;
            }
        }
    }

    internal class SetpieceCommand : Command
    {
        public SetpieceCommand()
            : base("setpiece", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            ISetPiece piece = (ISetPiece)Activator.CreateInstance(Type.GetType(
                "wServer.realm.setpieces." + args[0], true, true));
            piece.RenderSetPiece(player.Owner, new IntPoint((int)player.X + 1, (int)player.Y + 1));
            return true;
        }
    }

    internal class ListCommands : Command
    {
        public ListCommands() : base("commands", permLevel: 3) { }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Dictionary<string, Command> cmds = new Dictionary<string, Command>();
            Type t = typeof(Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
            StringBuilder sb = new StringBuilder("");
            Command[] copy = cmds.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(copy[i].CommandName);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    // custom commands for Lunar:
    class MaxStar : Command
    {
        public MaxStar()
            : base("maxstar", 3)
        { }
        public string Command { get { return "maxstar"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {

                var p = player.Manager.FindPlayer(args[0]);
                if (p == null)
                {
                    player.SendError("Invalid Player Name");
                    return false;
                }
                else
                {
                    int[] chars = { 768, 775, 782, 784, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806 };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        int charid = chars[i];
                        player.Manager.Database.DoActionAsync(db =>
                        {
                            var cmd = db.CreateQuery();
                            cmd.CommandText = "SELECT * FROM classstats  WHERE accId=@accId AND objType=@charType;";
                            cmd.Parameters.AddWithValue("@accId", p.AccountId);
                            cmd.Parameters.AddWithValue("@charType", charid);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (!rdr.HasRows)
                            {
                                rdr.Close();
                                cmd = db.CreateQuery();
                                cmd.CommandText = "INSERT INTO classstats VALUES(@accId,@charType,20,2000);";
                                cmd.Parameters.AddWithValue("@accId", p.AccountId);
                                cmd.Parameters.AddWithValue("@charType", charid);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                rdr.Close();
                                cmd = db.CreateQuery();
                                cmd.CommandText = "UPDATE classstats SET bestFame=2000,bestLv = 20 WHERE accId=@accId AND objType=@charType";
                                cmd.Parameters.AddWithValue("@accId", p.AccountId);
                                cmd.Parameters.AddWithValue("@charType", charid);
                                cmd.ExecuteNonQuery();
                            }


                        }
                        );

                    }
                    p.UpdateCount++;
                    player.SendInfo("Success!");
                    return true;
                }



            }
            catch (SystemException e)
            {
                player.SendError(e.Message);
                player.SendError(e.StackTrace);
            }
            return true;
        }
    }
    class resetStar : Command
    {
        public resetStar()
            : base("resetstar", 3)
        {
        }
        public string Command { get { return "resetstar"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {

                var p = player.Manager.FindPlayer(args[0]);
                if (p == null)
                {
                    player.SendError("Invalid Player Name");
                    return false;
                }
                else
                {
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE classstats SET bestFame=0, bestLv = 0 WHERE accId=@accId;";
                        cmd.Parameters.AddWithValue("@accId", p.AccountId);
                        cmd.ExecuteNonQuery();

                    }
                    );
                    p.UpdateCount++;
                    player.SendInfo("Success!");
                    return true;
                }
            }
            catch (SystemException e)
            {
                player.SendError(e.Message);
                player.SendError(e.StackTrace);
                return false;
            }

        }
    }
    class visitRealm : Command
    {
        public visitRealm()
            : base("realm")
        {
        }
        public string Command { get { return "realm"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                World realm = player.Client.Manager.Monitor.GetRandomRealm();
                player.Client.Reconnect(new ReconnectPacket
                {
                    GameId = realm.Id,
                    Host = "",
                    IsFromArena = false,
                    Key = realm.PortalKey,
                    KeyTime = -1,
                    Name = realm.Name,
                    Port = -1

                });
                player.SendInfo("Success!");
                return true;
            }
            catch (SystemException e)
            {
                player.SendError(e.Message);
                player.SendError(e.StackTrace);
                return false;
            }
        }
    }
    class visitVault : Command
    {
        public visitVault()
            : base("vault")
        {
        }
        public string Command { get { return "vault"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                World vault = player.Client.Manager.PlayerVault(player.Client);
                player.Client.Reconnect(new ReconnectPacket
                {
                    GameId = vault.Id,
                    Host = "",
                    IsFromArena = false,
                    Key = vault.PortalKey,
                    KeyTime = -1,
                    Name = vault.Name,
                    Port = -1

                });
                player.SendInfo("Success!");
                return true;
            }
            catch (SystemException e)
            {
                player.SendError(e.Message);
                player.SendError(e.StackTrace);
                return false;
            }
        }
    }
    class debug : Command
    {
        public debug() : base("debug", 3)
        { }
        public string Command { get { return "debug"; } }
        public int RequiredRank { get { return 3; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.inDebug = !player.inDebug;
            player.SendInfo("Debug turned " + (player.inDebug ? "On" : "Off"));
            return true;
        }

    }
    class Revive : Command
    {
        public Revive()
            : base("revive", 3)
        { }
        public string Command { get { return "revive"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Invalid Player Name");
                return false;
            }
            else
            {


                player.Manager.Database.DoActionAsync(db =>
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT 1 FROM characters  WHERE accId=@accId ORDER BY deathTime DESC;";
                    cmd.Parameters.AddWithValue("@accId", p.AccountId);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                    {
                        rdr.Close();
                        player.SendError("No characters to revive!");

                    }
                    else
                    {
                        rdr.Close();
                        var cmd2 = db.CreateQuery();
                        cmd2.CommandText = "UPDATE characters SET dead=0,deathTime=\"0000-00-00 00:00:00\" WHERE accId =@accId ORDER BY deathTime DESC LIMIT 1;";
                        cmd2.Parameters.AddWithValue("@accId", p.AccountId);
                        cmd2.ExecuteNonQuery();
                        player.SendInfo("Success! Go to home menu to see change.");

                    }

                });
                return true;
            }

        }
    }
    class Visit : Command
    {
        public Visit()
            : base("visit", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Invalid Player Name");
                return false;
            }
            foreach (Client i in player.Manager.Clients.Values)
            {
                Packet pkt;
                if (i.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    if (i.Player.Owner == player.Owner)
                    {
                        player.Move(i.Player.X, i.Player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = player.Id,
                            Position = new Position(i.Player.X, i.Player.Y)
                        };
                        player.UpdateCount++;
                        player.SendInfo("You will be teleported shortly.");
                    }
                    else
                    {
                        pkt = new ReconnectPacket
                        {
                            GameId = i.Player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = i.Player.Owner.PortalKey,
                            KeyTime = -1,
                            Name = i.Player.Owner.Name,
                            Port = -1
                        };
                        player.SendInfo("You will be connected shortly.");

                    }
                    player.Client.SendPacket(pkt);
                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }
    class LeftToMax : Command
    {
        public LeftToMax()
            : base("left")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            List<string> result = new List<string>();


            if (player.Stats[0] < player.ObjectDesc.MaxHitPoints) result.Add("HP potions: " + ((int)(player.ObjectDesc.MaxHitPoints - player.Stats[0]) / 5));
            if (player.Stats[1] < player.ObjectDesc.MaxMagicPoints) result.Add("MP potions: " + ((int)(player.ObjectDesc.MaxMagicPoints - player.Stats[1]) / 5));
            if (player.Stats[2] < player.ObjectDesc.MaxAttack) result.Add("Attack potions: " + (player.ObjectDesc.MaxAttack - player.Stats[2]));
            if (player.Stats[3] < player.ObjectDesc.MaxDefense) result.Add("Defense potions: " + (player.ObjectDesc.MaxDefense - player.Stats[3]));
            if (player.Stats[4] < player.ObjectDesc.MaxSpeed) result.Add("Speed potions: " + (player.ObjectDesc.MaxSpeed - player.Stats[4]));
            if (player.Stats[5] < player.ObjectDesc.MaxHpRegen) result.Add("Vitality potions: " + (player.ObjectDesc.MaxHpRegen - player.Stats[5]));
            if (player.Stats[6] < player.ObjectDesc.MaxMpRegen) result.Add("Wisdom potions: " + (player.ObjectDesc.MaxMpRegen - player.Stats[6]));
            if (player.Stats[7] < player.ObjectDesc.MaxDexterity) result.Add("Dexterity potions: " + (player.ObjectDesc.MaxDexterity - player.Stats[7]));
            if (result.Count == 0) player.SendInfo("You're Maxed!");
            else player.SendInfo(string.Join('\n'.ToString(), result));
            return true;
        }
    }
    class Pet : Command
    {
        public Pet()
            : base("pet", 3)
        { }
        public string Command { get { return "pet"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Invalid Player Name");
                return false;
            }
            else
            {


                player.Manager.Database.DoActionAsync(db =>
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT petId FROM pets  WHERE accId=@accId";
                    cmd.Parameters.AddWithValue("@accId", p.AccountId);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    bool hasPet = rdr.HasRows;
                    int id;
                    if (!hasPet)
                    {
                        rdr.Close();
                        cmd = db.CreateQuery();
                        cmd.CommandText = "SELECT petId FROM pets ORDER BY petId DESC LIMIT 1;";
                        MySqlDataReader rdr2 = cmd.ExecuteReader();
                        rdr2.Read();
                        if (rdr2.HasRows) id = rdr2.GetInt32("petId");
                        else id = 1;
                        rdr2.Close();
                        cmd = db.CreateQuery();
                        cmd.CommandText = "INSERT INTO pets VALUES(@accId,@petId,32613,\"{ pets.Jalapeno_Skin}\",32865,0,4,100,'407,408,406','100,100,100','0,0,0');";
                        cmd.Parameters.AddWithValue("@accId", p.AccountId);
                        cmd.Parameters.AddWithValue("@petId", id);
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {
                        rdr.Read();
                        id = rdr.GetInt32("petId");
                    }

                    rdr.Close();
                    if (p.Client.Character.Pet == null)
                    {
                        var cmd2 = db.CreateQuery();
                        cmd2.CommandText = "UPDATE characters SET pet=1,petId=@petId WHERE charId=@charId AND AccId=@accId";
                        cmd2.Parameters.AddWithValue("@charId", p.Client.Character.CharacterId);
                        cmd2.Parameters.AddWithValue("@petId", id);
                        cmd2.Parameters.AddWithValue("@accId", p.AccountId);
                        cmd2.ExecuteNonQuery();

                        PetItem petitem = db.GetPet(id, p.Client.Account);
                        entities.Pet pet = new entities.Pet(player.Manager, petitem, null);
                        p.Client.SendPacket(new UpdatePetPacket
                        {
                            PetId = pet.PetId
                        });
                        p.Client.Player.Pet = pet;
                        pet.PlayerOwner = p.Client.Player;
                        p.Client.Player.SaveToCharacter();

                        p.Owner.EnterWorld(pet);
                    }
                    else
                    {
                        player.SendError("Error: player: " + args[0] + " already has a pet.");
                    }




                });
                return true;
            }

        }
    }
    class noPet : Command
    {
        public noPet()
           : base("nopet", 3)
        { }
        public string Command { get { return "nopet"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Invalid Player Name");
                return false;
            }
            else
            {


                player.Manager.Database.DoActionAsync(db =>
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT petId FROM characters  WHERE accId=@accId AND charId=@charId";
                    cmd.Parameters.AddWithValue("@accId", p.AccountId);
                    cmd.Parameters.AddWithValue("@charId", p.Client.Character.CharacterId);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    rdr.Read();
                    int petid = rdr.GetInt32("petId");
                    rdr.Close();
                    if (petid == -1)
                    {
                        player.SendError("Player has no pet");
                    }
                    else
                    {

                        var cmd2 = db.CreateQuery();
                        cmd2.CommandText = "UPDATE characters SET pet=-1,petId=-1 WHERE accId=@accId AND charId=@charId";
                        cmd2.Parameters.AddWithValue("@accId", p.AccountId);
                        cmd2.Parameters.AddWithValue("@charId", p.Client.Character.CharacterId);
                        cmd2.ExecuteNonQuery();

                        p.Client.Player.UpdateCount++;
                        p.Client.SendPacket(new UpdatePetPacket
                        {
                            PetId = -1
                        });
                        p.Owner.LeaveWorld(p.Pet);
                        p.Pet = null;
                        p.Client.Player.SaveToCharacter();

                    }


                });
                return true;
            }

        }
    }
    class Where : Command
    {
        public Where()
            : base("where")
        { }
        public string Command { get { return "where"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendError("Invalid Player Name");
                return false;
            }
            player.SendInfo("Player " + args[0] + " is in world: " + p.Owner.Name);
            return true;
        }
    }
    class Godlands : Command
    {
        public Godlands()
            : base("glands")
        { }
        public string Command { get { return "glands"; } }
        public int RequiredRank { get { return 3; } }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner.Name.Contains("NexusPortal"))
            {
                player.Move(1478, 1086);
                player.ApplyConditionEffect(ConditionEffectIndex.Invincible, 2000);
                player.ApplyConditionEffect(ConditionEffectIndex.Invisible, 2000);
                return true;
            }
            else
            {
                player.SendError("You aren't in the realm.");
                return false;
            }
        }
    }
    class CloseRealm : Command
    {
        public CloseRealm()
            : base("close", 3)
        { }
        public string Command { get { return "glands"; } }
        public int RequiredRank { get { return 3; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner.Name.Contains("NexusPortal"))
            {
                new Oryx(player.Owner as GameWorld).CloseRealm();
                return true;
            }
            else
            {
                player.SendError("You're not in a realm.");
                return false;
            }
        }
    }
    class CreateParty : Command
    {
        public CreateParty()
            : base("pcreate")
        { }
        public string Command { get { return "pcreate"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.inParty)
            {
                player.SendInfo("You must leave your current party before creating a new one!");
                return false;
            }
            else
            {
                player.Manager.Database.DoActionAsync(db =>
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET inParty=1,party=\"\",currInv=@invId WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", player.AccountId);
                    cmd.Parameters.AddWithValue("invId", player.AccountId);
                    player.SendInfo("Party created!");
                    cmd.ExecuteNonQuery();
                });
                player.inParty = true;
                player.currentinvite = int.Parse(player.AccountId);
                player.party = "";
                return true;

            }
        }
    }
    class InviteParty : Command
    {
        public InviteParty()
            : base("pinv")
        { }
        public string Command { get { return "pinv"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args[0].ToLower() == player.Name.ToLower())
            {
                player.SendInfo("You cannot invite yourself!");
                return false;
            }
            if (!player.inParty)
            {
                player.SendInfo("You aren't in a party!");
                return false;
            }
            if (player.currentinvite != int.Parse(player.AccountId))
            {
                player.SendInfo("You must be the party leader to invite!");
                return false;
            }
            if (player.party.Split(' ').Count() == 5)
            {
                player.SendInfo("Your party is at capacity!");
                return false;
            }


            var p = player.Manager.FindPlayer(args[0]);
            if (p == null)
            {
                player.SendInfo("Invalid Player Name");
                return false;
            }

            p.currentinvite = int.Parse(player.AccountId);

            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE characters SET currInv = @id WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@accId", int.Parse(p.AccountId));
                cmd.Parameters.AddWithValue("@id", int.Parse(player.AccountId));
                cmd.ExecuteNonQuery();
            });
            player.SendInfo("You invited " + p.Name + " to the party!");
            p.SendInfo("You were invited to " + player.Name + "'s party");
            return true;

        }


    }
    class AcceptParty : Command
    {
        public AcceptParty()
            : base("pacc")
        { }
        public string Command { get { return "pacc"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.inParty)
            {
                player.SendInfo("You must leave your current party first!");
                return false;
            }
            if (player.currentinvite == -1)
            {
                player.SendInfo("You don't have any pending invites!");
                return false;
            }

            int leader = -1;

            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM characters  WHERE accId=@accId LIMIT 1;";
                cmd.Parameters.AddWithValue("@accId", player.currentinvite.ToString());
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                if (rdr.HasRows) leader = rdr.GetInt32("currInv");

                rdr.Close();
                if (leader != player.currentinvite)
                {

                    player.SendInfo(leader.ToString());
                    player.SendInfo(player.currentinvite.ToString());
                    player.SendInfo("You must be invited by the leader of the party!");
                    player.currentinvite = -1;

                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET currInv=-1 WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", player.currentinvite);
                    cmd.ExecuteNonQuery();


                }
                else
                {
                    string party = "";

                    cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT * FROM characters WHERE accId=@accId LIMIT 1";
                    cmd.Parameters.AddWithValue("@accId", player.currentinvite);
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    if (rdr.HasRows) party = rdr.GetString("party");
                    rdr.Close();

                    if (party.Split(' ').Count() == 5)
                    {
                        player.currentinvite = -1;

                        cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE characters SET currInv=-1 WHERE accId=@accId;";
                        cmd.Parameters.AddWithValue("@accId", int.Parse(player.AccountId));
                        cmd.ExecuteNonQuery();

                        player.SendInfo("Party at capacity!");
                    }
                    party += party == "" ? player.AccountId : " " + player.AccountId;
                    foreach (String i in party.Split(' '))
                    {
                        int pid = int.Parse(i);



                        cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE characters SET party=@party WHERE accId = @accId;";
                        cmd.Parameters.AddWithValue("@accId", pid);
                        cmd.Parameters.AddWithValue("@party", party);
                        cmd.ExecuteNonQuery();
                        cmd = db.CreateQuery();
                        cmd.CommandText = "SELECT * FROM accounts WHERE id=@accId LIMIT 1;";
                        cmd.Parameters.AddWithValue("@accId", pid);
                        rdr = cmd.ExecuteReader();
                        rdr.Read();
                        String name = rdr.GetString("name");
                        Player p = player.Manager.FindPlayer(name);
                        if (p != null && p != player)
                        {
                            p.party = party;
                            p.SendInfo(player.Name + " has joined your party!");

                        }
                        rdr.Close();


                    }
                    string name2 = "";

                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET party=@party WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", player.currentinvite);
                    cmd.Parameters.AddWithValue("@party", party);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT * FROM accounts WHERE id=@accId LIMIT 1;";
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    name2 = rdr.GetString("name");
                    rdr.Close();
                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET inParty = 1, party=@party WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", int.Parse(player.AccountId));
                    cmd.Parameters.AddWithValue("@party", party);
                    cmd.ExecuteNonQuery();
                    Player p2 = player.Manager.FindPlayer(name2);

                    player.inParty = true;
                    player.party = party;

                    if (p2 != null) p2.SendInfo("Player: " + player.Name + " has joined your party!");
                    player.SendInfo("You have joined " + name2 + "'s party!");
                }





            });


            return true;

        }
    }
    class ListParty : Command
    {
        public ListParty()
            : base("plist")
        { }
        public string Command { get { return "plist"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.inParty)
            {
                player.SendInfo("You're not in a party!");
                return true;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM accounts  WHERE id=@accId LIMIT 1;";
                cmd.Parameters.AddWithValue("@accId", player.currentinvite);
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                String name = rdr.GetString("name");
                player.SendInfo("Party Leader: " + name);
                rdr.Close();

                player.SendInfo("Party Members:");
                foreach (String i in player.party.Split(' '))
                {

                    cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT * FROM accounts  WHERE id=@accId LIMIT 1;";
                    cmd.Parameters.AddWithValue("@accId", i);
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    if (rdr.HasRows)
                    {
                        name = rdr.GetString("name");
                        player.SendInfo(name);
                    }
                    rdr.Close();

                }
            });
            if (player.party == "") return true;

            return true;

        }
    }
    class LeaveParty : Command
    {
        public LeaveParty()
            : base("pleave")
        { }
        public string Command { get { return "pleave"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.currentinvite == int.Parse(player.AccountId))
            {
                player.SendInfo("You cannot leave a party you own, do /pclose to close the party instead!");
                return false;
            }
            if (!player.inParty)
            {
                player.SendInfo("You're not in a party!");
                return true;
            }
            string party = "";
            player.Manager.Database.DoActionAsync(db =>
            {
                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM characters WHERE accId=@accId LIMIT 1;";
                cmd.Parameters.AddWithValue("@accId", player.currentinvite.ToString());
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                if (rdr.HasRows) party = rdr.GetString("party");
                rdr.Close();
                List<String> partymem = party.Split(' ').ToList();
                partymem.Remove(player.AccountId);
                party = String.Join(" ", partymem.Cast<String>());
                cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE characters SET party=@party WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@party", party);
                cmd.Parameters.AddWithValue("@accId", player.currentinvite.ToString());
                cmd.ExecuteNonQuery();
                cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM accounts WHERE id=@accId LIMIT 1;";
                cmd.Parameters.AddWithValue("@accId", player.currentinvite.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    String name = rdr.GetString("name");
                    Player p = player.Manager.FindPlayer(name);
                    rdr.Close();
                    if (p != null)
                    {
                        p.SendInfo(player.Name + " has left the party!");
                    }
                }
                else rdr.Close();
                player.inParty = false;
                foreach (String i in player.party.Split(' '))
                {
                    int id = int.Parse(i);
                    if (id != int.Parse(player.AccountId))
                    {


                        cmd = db.CreateQuery();
                        cmd.CommandText = "SELECT * FROM accounts  WHERE id=@accId LIMIT 1;";
                        cmd.Parameters.AddWithValue("@accId", id);
                        rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            rdr.Close();
                            String name = rdr.GetString("name");
                            cmd.CommandText = "UPDATE characters SET party=@party WHERE id=@accId;";
                            cmd.Parameters.AddWithValue("accId", id);
                            cmd.Parameters.AddWithValue("@party", party);
                            cmd.ExecuteNonQuery();

                            Player p = player.Manager.FindPlayer(name);

                            if (p != null)
                            {
                                p.party = party;
                                p.SendInfo(player.Name + " has left the party!");
                            }
                        }
                        else rdr.Close();



                    }
                }

                var cmd2 = db.CreateQuery();
                cmd2.CommandText = "UPDATE characters SET inParty=0,party=\"\",currInv=-1 WHERE accId=@accId";
                cmd2.Parameters.AddWithValue("@accId", int.Parse(player.AccountId));
                cmd2.ExecuteNonQuery();


                player.inParty = false;
                player.party = "";
                player.currentinvite = -1;
                player.SendInfo("You have left the party!");

            });
            return true;


        }
    }
    class KickParty : Command
    {
        public KickParty()
                    : base("pkick")
        { }
        public string Command { get { return "pkick"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            List<String> names = player.party.Split(' ').ToList();
            if (!player.inParty)
            {
                player.SendInfo("You're not in a party!");
                return false;
            }
            if (player.currentinvite != int.Parse(player.AccountId))
            {
                player.SendInfo("You're not the leader of this party!");
                return false;
            }
            if (args[0] == null || args[0] == "")
            {
                player.SendInfo("Invalid player!");
                return false;
            }

            player.Manager.Database.DoActionAsync(db =>
            {
                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM accounts WHERE name=@name LIMIT 1;";
                cmd.Parameters.AddWithValue("@name", args[0]);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    int id = rdr.GetInt32("id");
                    rdr.Close();
                    names.Remove(id.ToString());
                    String party = String.Join(" ", names);
                    player.party = party;

                    names = party.Split(' ').ToList();
                    foreach (String n in names)
                    {

                        cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE characters SET party=@party WHERE accId=@accId;";
                        cmd.Parameters.AddWithValue("@accId", n);
                        cmd.Parameters.AddWithValue("@party", party);
                        cmd.ExecuteNonQuery();
                        cmd = db.CreateQuery();
                        cmd.CommandText = "SELECT * FROM accounts WHERE id=@accId LIMIT 1;";
                        cmd.Parameters.AddWithValue("@accId", n);
                        rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            String name = rdr.GetString("name");
                            rdr.Close();
                            Player p = player.Manager.FindPlayer(name);
                            if (p != null)
                            {
                                p.SendInfo(args[0] + " has left your party!");
                                p.party = party;
                            }
                        }
                        else rdr.Close();

                    }
                    player.SendInfo("You have kicked " + args[0] + " from the party!");
                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET inParty=0, Party=\"\", currInv=-1 WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", id);
                    cmd.ExecuteNonQuery();
                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE characters SET Party=@party WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", player.AccountId);
                    cmd.Parameters.AddWithValue("@party", party);
                    cmd.ExecuteNonQuery();
                    Player target = player.Manager.FindPlayer(args[0]);
                    if (target != null)
                    {
                        target.SendInfo("You have been kicked from the party!");
                        target.party = "";
                        target.currentinvite = -1;
                        target.inParty = false;

                    }

                }
                else rdr.Close();
            });
            return true;
        }
    }
    class SummonParty : Command
    {
        public SummonParty()
                    : base("psumm")
        { }
        public string Command { get { return "psumm"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.inParty)
            {
                player.SendInfo("You're not in a party!");
                return false;
            }
            if (player.currentinvite != int.Parse(player.AccountId))
            {
                player.SendInfo("You're not the leader of the party!");
                return false;
            }
            if (!player.Owner.IsDungeon() || player.Owner == player.Manager.PlayerVault(player.Client) || player.Owner == player.Guild.GuildHall)
            {
                player.SendInfo("You cannot summon outside a dungeon!");
                return false;
            }
            if (player.Owner.PortalKeyExpired)
            {
                player.SendInfo("This dungeon is already closed!");
                return false;
            }
            foreach (String id in player.party.Split(' '))
            {
                player.Manager.Database.DoActionAsync(db =>
                {
                    MySqlCommand cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT * FROM accounts where id=@accId;";
                    cmd.Parameters.AddWithValue("@accId", id);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        String name = rdr.GetString("name");
                        rdr.Close();
                        Player p = player.Manager.FindPlayer(name);
                        if (p != null)
                        {
                            if (p.Owner == player.Owner)
                            {
                                p.Move(player.X, player.Y);
                            }
                            else
                            {
                                Packet pkt = new ReconnectPacket
                                {
                                    GameId = player.Owner.Id,
                                    Host = "",
                                    IsFromArena = false,
                                    Key = player.Owner.PortalKey,
                                    KeyTime = -1,
                                    Name = player.Owner.Name,
                                    Port = -1
                                };
                                p.SendInfo("Your party leader has summoned you!");
                                p.Client.SendPacket(pkt);
                            }


                        }
                    }
                });

            }
            player.SendInfo("Success!");
            return true;


        }
    }
    class CloseParty : Command
    {
        public CloseParty()
                    : base("pclose")
        { }
        public string Command { get { return "pclose"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.inParty)
            {
                player.SendInfo("You're not in a party!");
                return false;
            }
            if (player.currentinvite.ToString() != player.AccountId)
            {
                player.SendInfo("You're not the leader of that party!");
                return false;
            }
            foreach (String member in player.party.Split(' '))
            {
                if (member != player.AccountId)
                {
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        MySqlCommand cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE characters SET inParty=0,Party=\"\",currInv=-1 WHERE accId = @accId";
                        cmd.Parameters.AddWithValue("@accId", member);
                        cmd.ExecuteNonQuery();
                        cmd = db.CreateQuery();
                        cmd.CommandText = "SELECT * FROM accounts WHERE id=@accId LIMIT 1";
                        cmd.Parameters.AddWithValue("@accId", member);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        rdr.Read();
                        String name = rdr.GetString("name");
                        rdr.Close();
                        Player p = player.Manager.FindPlayer(name);
                        if (p != null)
                        {
                            p.SendInfo("Your party has been closed!");
                            p.inParty = false;
                            p.party = "";
                            p.currentinvite = -1;
                        }
                    });
                }


            }
            player.Manager.Database.DoActionAsync(db =>
            {
                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE characters SET inParty=0,Party=\"\", currInv=-1 WHERE accId = @accId";
                cmd.Parameters.AddWithValue("@accId", player.AccountId);
                cmd.ExecuteNonQuery();
            });
            player.SendInfo("Your party has been closed!");
            player.inParty = false;
            player.party = "";
            player.currentinvite = -1;
            return true;
        }
    }
    class CreateLink : Command
    {
        public CreateLink()
                   : base("linkcreate", 3)
        { }
        public string Command { get { return "linkcreate"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.Owner.IsDungeon() || player.Owner == player.Manager.PlayerVault(player.Client) || player.Owner == player.Guild.GuildHall)
            {
                player.SendInfo("You're not in a dungeon!");
                return false;
            }
            Packet pkt = new ReconnectPacket
            {
                GameId = player.Owner.Id,
                Host = "",
                IsFromArena = false,
                Key = player.Owner.PortalKey,
                KeyTime = -1,
                Name = player.Owner.Name,
                Port = -1
            };
            foreach (Client c in player.Manager.Clients.Values)
            {
                c.Pkt = pkt;
                if (c != player.Client) c.Player.SendInfo("Link created to: " + player.Owner.Name + ". Type /link to accept!");
            }
            player.SendInfo("Link created!");
            return true;

        }
    }
    class AcceptLink : Command
    {
        public AcceptLink() : base("link") { }
        public string Command { get { return "link"; } }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Client.Pkt != null)
            {
                player.Client.SendPacket(player.Client.Pkt);
                return true;
            }
            player.SendInfo("You have no link to accept!");
            return false;
        }
    }
}