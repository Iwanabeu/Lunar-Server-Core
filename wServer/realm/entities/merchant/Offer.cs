using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.realm.entities.merchant
{
    public enum SellType
    {
        Weapon,
        Ability,
        Armor,
        Ring,
        Consumable
    }
    public class Offer
    {
        public int Price {get; private set; }
        public int Id { get; private set; }
        public int Item { get; private set; }
        public int Seller { get; private set; }
        public SellType Type { get; private set; }
        public Offer(int price, int id, int item, int seller, int type)
        {
            this.Price = price;
            this.Id = id;
            this.Item = item;
            this.Seller = seller;
            switch (type) {
                case 0:
                    this.Type = SellType.Weapon;
                    break;
                case 1:
                    this.Type = SellType.Ability;
                    break;
                case 2:
                    this.Type = SellType.Armor;
                    break;
                case 3:
                    this.Type = SellType.Ring;
                    break;
                default:
                    this.Type = SellType.Consumable;
                    break;

            }
        }
    }
}
