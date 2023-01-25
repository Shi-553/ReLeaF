namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void SetEnemyDamageable(IEnemyDamageable damageable)
        {
            enemyDamageable = damageable;

            if (DungeonManager.Singleton.TryGetTile<Plant>(TilePos, out var plant))
            {
                plant.IsSetWeakMarker = true;
            }
        }
        protected override void UninitImpl()
        {
            if (DungeonManager.Singleton != null && DungeonManager.Singleton.TryGetTile<Plant>(TilePos, out var plant))
            {
                plant.IsSetWeakMarker = false;
            }
        }
        public override bool OnGreening(DungeonManager.GreeningInfo info)
        {
            if (info.tilePos == TilePos)
            {
                enemyDamageable.DamagedGreening(TilePos, 1);
                return true;
            }
            return false;
        }
    }
}
