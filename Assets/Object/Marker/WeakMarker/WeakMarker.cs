namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void SetEnemyDamageable(IEnemyDamageable damageable)
        {
            enemyDamageable = damageable;

            if (DungeonManager.Singleton.TryGetTile<Plant>(tilePos, out var plant))
            {
                plant.IsSetWeakMarker = true;
            }
        }
        protected override void UninitImpl()
        {
            if (DungeonManager.Singleton != null && DungeonManager.Singleton.TryGetTile<Plant>(tilePos, out var plant))
            {
                plant.IsSetWeakMarker = false;
            }
        }
        public override void OnGreening(DungeonManager.GreeningInfo info)
        {
            if (info.tilePos == tilePos)
            {
                enemyDamageable.DamagedGreening(tilePos, 1);
            }
        }
    }
}
