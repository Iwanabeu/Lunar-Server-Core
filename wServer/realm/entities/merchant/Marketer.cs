#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using log4net;
using wServer.networking.svrPackets;
using wServer.realm.entities.player;
using wServer.realm.entities.merchant;
using MySql.Data.MySqlClient;
using wServer.networking;
#endregion

namespace wServer.realm.entities.marketer
{
    public class Marketer : SellableObject
    {
        private const int BUY_NO_GOLD = 3;
        private const int BUY_NO_FAME = 6;
        private const int BUY_NO_FORTUNETOKENS = 9;
        private const int MERCHANT_SIZE = 100;
        private static readonly ILog log = LogManager.GetLogger(typeof(Marketer));
        private Offer offer;
        private ConcurrentDictionary<int, Offer> list;

        private int tickcount;

        public static Random Random { get; private set; }

        public Marketer(RealmManager manager, ushort objType = 0xb000, World owner = null)
            : base(manager, objType)
        {
            SellId = -1;
            Size = MERCHANT_SIZE;
            if (owner != null)
                Owner = owner;

            if (Random == null) Random = new Random();
            if (owner != null)
            {
                ResolveSellId();
                refresh();
            }
        }


        public bool Custom { get; set; }
        public int SellType { get; set; }
        public int SellId { get; set; }
        public int MTime { get; set; }
        public int Discount { get; set; }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.Market_Type] = SellId;
            stats[StatsType.Market_Price] = Price;
            base.ExportStats(stats);
        }

        public override void Init(World owner)
        {
            base.Init(owner);
            ResolveSellId();
            refresh();
            UpdateCount++;
            //if (SellId == -1) Owner.LeaveWorld(this);
        }

        protected override bool TryDeduct(Player player)
        {
            var acc = player.Client.Account;

            return acc.Credits >= Price;
        }

        public override void Buy(Player player)
        {
            Manager.Database.DoActionAsync(db =>
            {
                if (ObjectType == 0xb000) //Marketer
                {
                    if (player.AccountId.Equals(offer.Seller.ToString()))
                    {
                        player.SendInfo("You cannot buy your own item!");
                    }
                    else if (TryDeduct(player))
                    {
                        for (var i = 4; i < player.Inventory.Length; i++)
                        {
                            //Buyer Side, check inventory and buy
                            if (player.Inventory[i] != null) continue;
                            player.Inventory[i] = Manager.GameData.Items[(ushort)SellId];

                            player.Credits =
                                player.Client.Account.Credits =
                                    db.UpdateCredit(player.Client.Account, -Price);
                            player.Client.SendPacket(new BuyResultPacket
                            {
                                Result = 0,
                                Message = "{\"key\":\"server.buy_success\"}"
                            });
                            player.UpdateCount++;
                            player.SaveToCharacter();
                            UpdateCount++;
                            //Buyer side complete
                            //Seller side, add gold and notify player
                            String sellerid = offer.Seller.ToString();
                            if (Manager.Clients.ContainsKey(sellerid))
                            {
                                Client c = Manager.Clients[sellerid];
                                c.Player.SendInfo("Your: " + Manager.GameData.ObjectTypeToId + " has sold for: " + Price + " gold!");
                                c.Player.Credits =
                                c.Account.Credits =
                                    db.UpdateCredit(c.Account, Price);
                            }
                            else
                            {
                                db.UpdateCredit(sellerid.ToString(), Price);
                            }
                            //Seller side complete

                            //Server side, remove offer and update display
                            db.removeOffer(offer.Id);
                            MySqlCommand cmd = db.CreateQuery();
                            cmd.CommandText = "SELECT offerId,Value,Item,Type,accId FROM offers WHERE Value=(SELECT MIN(Value) FROM offers AS o WHERE o.Item=@itemId);";
                            cmd.Parameters.AddWithValue("@itemId", this.SellId.ToString());
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            rdr.Read();
                            if (rdr.HasRows)
                            {

                                int offerId = rdr.GetInt32("offerId");
                                int itemId = rdr.GetInt32("Item");
                                int price = rdr.GetInt32("Value");
                                int type = rdr.GetInt32("Type");
                                int acc = rdr.GetInt32("accId");
                                Offer offer = new Offer(price: price, id: offerId, item: itemId, seller: acc, type: type);
                                list.AddOrUpdate(this.SellId, offer, (key, oldValue) => offer);
                                this.checkPrices();
                            }
                            else
                            {
                                Offer temp;
                                list.TryRemove(this.SellId, out temp);
                                temp = null;

                                this.refresh();
                            }
                            rdr.Close();
                            return;



                        }
                        player.Client.SendPacket(new BuyResultPacket
                        {
                            Result = 0,
                            Message = "{\"key\":\"server.inventory_full\"}"
                        });

                    }
                    else
                    {


                        player.Client.SendPacket(new BuyResultPacket
                        {
                            Result = BUY_NO_GOLD,
                            Message = "{\"key\":\"server.not_enough_gold\"}"
                        });
                    }
                }
            });
        }

        public override void Tick(RealmTime time)
        {
            try
            {
                if (Size == 0 && SellId != -1)
                {
                    Size = MERCHANT_SIZE;
                    UpdateCount++;
                }


                tickcount++;
                if (tickcount % (Manager?.TPS * 10) == 0) //once per minute after spawning
                {

                    refresh();
                    UpdateCount++;
                }
                else
                {
                    checkPrices();
                    UpdateCount++;
                }




                //if (SellId == -1) Owner?.LeaveWorld(this);

                base.Tick(time);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public void checkPrices()

        {

            if (this.Owner == null || this.offer == null || this.SellId == -1 || this.list == null || this.list.IsEmpty) return;
            switch (this.SellType)
            {
                case 0:
                    if (Manager.Marketplace_Weapons[this.SellId].Price < this.Price)
                    {
                        this.offer = Manager.Marketplace_Weapons[this.SellId];
                        this.Price = offer.Price;
                    }
                    break;
                case 1:
                    if (Manager.Marketplace_Abilities[this.SellId].Price < this.Price)
                    {
                        this.offer = Manager.Marketplace_Abilities[this.SellId];
                        this.Price = offer.Price;
                    }
                    break;
                case 2:
                    if (Manager.Marketplace_Armor[this.SellId].Price < this.Price)
                    {
                        this.offer = Manager.Marketplace_Armor[this.SellId];
                        this.Price = offer.Price;
                    }
                    break;
                case 3:
                    if (Manager.Marketplace_Rings[this.SellId].Price < this.Price)
                    {
                        this.offer = Manager.Marketplace_Rings[this.SellId];
                        this.Price = offer.Price;
                    }
                    break;
                case 4:
                    if (Manager.Marketplace_Consumables[this.SellId].Price < this.Price)
                    {
                        this.offer = Manager.Marketplace_Consumables[this.SellId];
                        this.Price = offer.Price;
                    }
                    break;

            }
        }

        public void refresh()
        {
            if (list == null || list.IsEmpty) { this.ResolveSellId(); this.SellId = 0xb000; return; }
            foreach (int demand in Manager.searched)
            {
                this.offer = list[demand];
                this.Price = offer.Price;
                this.SellId = demand;
                this.SellType = (int)offer.Type;
                Manager.searched.Remove(demand);
                Manager.InUse.Add(demand);
                return;

            }
            int chosen;
            do
            {
                chosen = list.RandomElement(Random).Key;
            } while (Manager.InUse.Contains(chosen) && !(Manager.InUse.Count >= list.Count));
            this.offer = list[chosen];
            this.Price = offer.Price;
            this.SellId = chosen;
            this.SellType = (int)offer.Type;
            Manager.InUse.Add(chosen);
            UpdateCount++;
            return;

        }
        public void ResolveSellId()
        {
            SellId = -1;
            if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Market_Weapons)
            {
                this.list = Manager.Marketplace_Weapons;
                this.SellType = 0;
            }
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Market_Abilities)
            {
                this.list = Manager.Marketplace_Abilities;
                this.SellType = 1;
            }
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Market_Armor)
            {
                this.list = Manager.Marketplace_Armor;
                this.SellType = 2;
            }
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Market_Rings)
            {
                this.list = Manager.Marketplace_Rings;
                this.SellType = 3;
            }
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Market_Consumables)
            {
                this.list = Manager.Marketplace_Consumables;
                this.SellType = 4;
            }
        }
    }
}