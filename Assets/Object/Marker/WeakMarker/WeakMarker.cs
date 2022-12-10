namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void SetEnemyDamageable(IEnemyDamageable damageable)
        {
            enemyDamageable = damageable;
        }
        public override void TileChanged(DungeonManager.TileChangedInfo info)
        {
            if (info.tilePos == tilePos && info.afterTile.TileType == TileType.Foundation)
            {
                enemyDamageable.DamagedGreening(tilePos, 1);
            }
        }
    }
}
