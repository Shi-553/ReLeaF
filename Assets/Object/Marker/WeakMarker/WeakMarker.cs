using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void Init(IEnemyDamageable damageable)
        {
            enemyDamageable= damageable;
            var tile = DungeonManager.Instance.GetGroundTile(tilePos);
            if (tile != null && (tile.tileType == TileType.Plant || tile.tileType == TileType.Foundation))
            {
                DungeonManager.Instance.ToSand(tilePos);
            }
        }
        public override void TileChanged(DungeonManager.TileChangedInfo info)
        {
            if (info.tilePos==tilePos&&(info.afterTile.tileType == TileType.Plant || info.afterTile.tileType == TileType.Foundation))
            {
                enemyDamageable.DamagedGreening(tilePos, 1);
            }
        }
    }
}
