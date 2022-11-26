using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void SetEnemyDamageable(IEnemyDamageable damageable)
        {
            enemyDamageable = damageable;
            if (DungeonManager.Singleton.TryGetTile(tilePos, out var tile) && tile.TileType == TileType.Plant)
            {
                DungeonManager.Singleton.ToSand(tilePos);
            }
        }
        public override void TileChanged(DungeonManager.TileChangedInfo info)
        {
            if (info.tilePos == tilePos && info.afterTile.TileType == TileType.Plant)
            {
                enemyDamageable.DamagedGreening(tilePos, 1);
            }
        }
    }
}
