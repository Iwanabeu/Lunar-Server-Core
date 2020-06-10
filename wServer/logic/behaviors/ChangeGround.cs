using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.data;
using wServer.realm;

namespace wServer.logic.behaviors
{
    class ChangeGround : Behavior
    {
        private readonly int dist;
        private readonly string[] groundToChange;
        private readonly string[] targetType;
        public ChangeGround(string[] GroundToChange, string[] ChangeTo, int dist)
        {
            groundToChange = GroundToChange;
            targetType = ChangeTo;
            this.dist = dist;
        }
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            var dat = host.Manager.GameData;
            var w = host.Owner ;
            var pos = new IntPoint((int)host.X - (dist / 2), (int)host.Y - (dist / 2));
            if (w == null) return;
            for (int x = 0; x < dist; x++)
            {
                for (int y = 0; y < dist; y++)
                {
                    WmapTile tile = w.Map[x + pos.X, y + pos.Y].Clone();
                    if (groundToChange != null)
                    {
                        foreach (string type in groundToChange)
                        {
                            int r = Random.Next(targetType.Length);
                            if (tile.TileId == dat.IdToTileType[type])
                            {
                                tile.TileId = dat.IdToTileType[targetType[r]];
                                w.Map[x + pos.X, y + pos.Y] = tile;
                            }
                        }
                    }
                    else
                    {
                        int r = Random.Next(targetType.Length);
                        tile.TileId = dat.IdToTileType[targetType[r]];
                        w.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
            }
            return;
        }
    }
}
