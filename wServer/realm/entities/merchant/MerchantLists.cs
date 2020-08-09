#region

using System;
using System.Collections.Generic;
using System.Linq;
using db.data;
using log4net;

#endregion

namespace wServer.realm.entities
{
    internal class MerchantLists
    {
        public static int[] AccessoryClothList;
        public static int[] AccessoryDyeList;
        public static int[] ClothingClothList;
        public static int[] ClothingDyeList;

        public static Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>
        {
            {0xb41, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},
            {0xbab, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},
            {0xbad, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},

            //WEAPONS
            {0xaf6, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa87, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa86, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa85, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa07, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xb02, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa8d, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa8c, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa8b, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa1e, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xb08, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xaa2, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xaa1, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xaa0, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa9f, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xb0b, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa47, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa84, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa83, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa82, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xaff, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa8a, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa89, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa88, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa19, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xc50, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xc4f, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xc4e, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xc4d, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xc4c, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},

            //Rings
            {0xabf, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac0, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xac1, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac2, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac3, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac4, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac5, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac6, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xac7, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xac8, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xac9, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xaca, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xacb, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xacc, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xacd, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xace, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            //ARMORS
            {0xb05, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa96, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa95, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa94, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa60, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xafc, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa93, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa92, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa91, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xa13, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xaf9, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa90, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xa8f, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},
            {0xa8e, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xad3, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)},

            //ABILITIES
            {0xb25, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa5b, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb22, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa0c, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb24, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa30, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb26, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa55, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb27, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xae1, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb28, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa65, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb29, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa6b, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb2a, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xaa8, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb2b, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xaaf, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb2c, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xab6, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb2d, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xa46, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb23, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xb20, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xb33, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xb32, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xc59, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xc58, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},

            //PET FOOD
            {0xccc, new Tuple<int, CurrencyType>(5, CurrencyType.Gold)},
            {0xccb, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xcca, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xcc9, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xcc8, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)},
            {0xcc7, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)},
            {0xcc6, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xcc5, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},
            {0xcc4, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)},

            //EGGS
            {0xc86, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon feline egg
            {0xc87, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare feline egg
            {0xc8a, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon canine egg
            {0xc8b, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare canine egg
            {0xc8e, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon avian egg
            {0xc8f, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare avian egg
            {0xc92, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon exotic egg
            {0xc93, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare exotic egg
            {0xc96, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon farm egg
            {0xc97, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare farm egg
            {0xc9a, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon woodland egg
            {0xc9b, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare woodland egg
            {0xc9e, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon reptile egg
            {0xc9f, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare reptile egg
            {0xca2, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon insect egg
            {0xca3, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare insect egg
            {0xca6, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon pinguin egg
            {0xca7, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare pinguin egg
            {0xcaa, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon aquatic egg
            {0xcab, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare aquatic egg
            {0xcae, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon spooky egg
            {0xcaf, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare spooky egg
            {0xcb2, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon humanoid egg
            {0xcb3, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare humanoid egg
            {0xcb6, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon ???? egg
            {0xcb7, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare ???? egg
            {0xcba, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon automaton egg
            {0xcbb, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare automaton egg
            {0xcbe, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //uncommon mystery egg
            {0xcbf, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //rare mystery egg

            //KEYS
            {0x2290, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)}, //Bella's Key - just temponary for testing

            {0x701, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)}, //Undead lair key
            {0x705, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)}, //Pirate cave key
            {0x70a, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //Abyss of demons key
            {0x70b, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)}, //Snake pit key
            {0x710, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //Tomb of the ancients key
            {0x71f, new Tuple<int, CurrencyType>(1000, CurrencyType.Gold)}, //Sprite World Key
            {0xc11, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //Ocean Trench Key
            {0xc19, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)}, //Totem Key
            {0xc23, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //Manor Key
            {0xc2e, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //Daby's Key
            {0xc2f, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //Lab Key
            {0xcce, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //Deadwater docks key
            {0xccf, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //Woodland Labyrinth Key
            {0xcda, new Tuple<int, CurrencyType>(4000, CurrencyType.Gold)}, //The crawling depths key
            {0xcdd, new Tuple<int, CurrencyType>(3000, CurrencyType.Gold)}, //Shatters key
        };

        public static int[] store10List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store11List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store12List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store13List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store14List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store15List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store16List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store17List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store18List = {0xb41, 0xbab, 0xbad, 0xbac};
        public static int[] store19List = {0xb41, 0xbab, 0xbad, 0xbac};

        public static int[] store1List =
        {
            0xcdd, 0xcda, 0xccf, 0xcce, 0xc2f, 0xc2e, 0xc23, 0xc19, 0xc11, 0x71f, 0x710,
            0x70b, 0x70a, 0x705, 0x701, 0x2290
        };

        public static int[] store20List = {0xb41, 0xbab, 0xbad, 0xbac};

        //keys need to add etcetc
        public static int[] store2List =
        {
            0xcbf, 0xcbe, 0xcbb, 0xcba, 0xcb7, 0xcb6, 0xcb2, 0xcb3, 0xcae, 0xcaf, 0xcab,
            0xcaa, 0xca7, 0xca6, 0xca3, 0xca2, 0xc9f, 0xc9e, 0xc9b, 0xc9a, 0xc97, 0xc96, 0xc93, 0xc92, 0xc8f, 0xc8e,
            0xc8b, 0xc8a, 0xc87, 0xc86
        };

        //pet eggs
        public static int[] store3List = {0xccc, 0xccb, 0xcca, 0xcc9, 0xcc8, 0xcc7, 0xcc6, 0xcc5, 0xcc4};

        //pet food
        public static int[] store4List =
        {
            0xb25, 0xa5b, 0xb22, 0xa0c, 0xb24, 0xa30, 0xb26, 0xa55, 0xb27, 0xae1, 0xb28,
            0xa65, 0xb29, 0xa6b, 0xb2a, 0xaa8, 0xb2b, 0xaaf, 0xb2c, 0xab6, 0xb2d, 0xa46, 0xb23, 0xb20, 0xb33, 0xb32,
            0xc59, 0xc58
        };

        //abilities
        public static int[] store5List =
        {
            0xb05, 0xa96, 0xa95, 0xa94, 0xa60, 0xafc, 0xa93, 0xa92, 0xa91, 0xa13, 0xaf9,
            0xa90, 0xa8f, 0xa8e, 0xad3
        };

        //armors
        public static int[] store6List =
        {
            0xaf6, 0xa87, 0xa86, 0xa85, 0xa07, 0xb02, 0xa8d, 0xa8c, 0xa8b, 0xa1e, 0xb08,
            0xaa2, 0xaa1, 0xaa0, 0xa9f
        };

        //Wands&staves&bows
        public static int[] store7List =
        {
            0xb0b, 0xa47, 0xa84, 0xa83, 0xa82, 0xaff, 0xa8a, 0xa89, 0xa88, 0xa19, 0xc50,
            0xc4f, 0xc4e, 0xc4d, 0xc4c
        };

        //Swords&daggers&samurai shit
        public static int[] store8List =
        {
            0x909e,0x909f,0x90a0,0x90a1,0x90a2,0x90a3,0x90a4,0x90a5,0x90a6,0x90a7,0x90a8,0x90a9,0x90aa,0x90ab,0x90ac,0x90ad,0x90ae,0x90af,0x90b0,0x90b1,0x90b2,0x90b3,0x90b4, 0x90b5
        };

        // rings
        public static int[] store9List = {0xb41, 0xbab, 0xbad, 0xbac};

        private static readonly ILog log = LogManager.GetLogger(typeof (MerchantLists));

        public static void InitMerchatLists(XmlData data)
        {
            log.Info("Loading merchant lists...");
            List<int> accessoryDyeList = new List<int>();
            List<int> clothingDyeList = new List<int>();
            List<int> accessoryClothList = new List<int>();
            List<int> clothingClothList = new List<int>();

            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => noShopCloths.All(i => i != _.Value.ObjectId)))
            {
                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Clothing") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(51, CurrencyType.Fame));
                    clothingDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Accessory") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(51, CurrencyType.Fame));
                    accessoryDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Large"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(160, CurrencyType.Fame));
                    clothingClothList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Small"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(160, CurrencyType.Fame));
                    accessoryClothList.Add(item.Value.ObjectType);
                }
            }

            ClothingDyeList = clothingDyeList.ToArray();
            ClothingClothList = clothingClothList.ToArray();
            AccessoryClothList = accessoryClothList.ToArray();
            AccessoryDyeList = accessoryDyeList.ToArray();
            log.Info("Merchat lists added.");
        }

        private static readonly string[] noShopCloths =
        {
            "Large Ivory Dragon Scale Cloth", "Small Ivory Dragon Scale Cloth",
            "Large Green Dragon Scale Cloth", "Small Green Dragon Scale Cloth",
            "Large Midnight Dragon Scale Cloth", "Small Midnight Dragon Scale Cloth",
            "Large Blue Dragon Scale Cloth", "Small Blue Dragon Scale Cloth",
            "Large Red Dragon Scale Cloth", "Small Red Dragon Scale Cloth",
            "Large Jester Argyle Cloth", "Small Jester Argyle Cloth",
            "Large Alchemist Cloth", "Small Alchemist Cloth",
            "Large Mosaic Cloth", "Small Mosaic Cloth",
            "Large Spooky Cloth", "Small Spooky Cloth",
            "Large Flame Cloth", "Small Flame Cloth",
            "Large Heavy Chainmail Cloth", "Small Heavy Chainmail Cloth",
        };
    }
}