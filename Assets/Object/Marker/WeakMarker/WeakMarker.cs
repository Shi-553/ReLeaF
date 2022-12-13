namespace ReLeaf
{

    public class WeakMarker : MarkerBase
    {
        IEnemyDamageable enemyDamageable;
        public void SetEnemyDamageable(IEnemyDamageable damageable)
        {
            enemyDamageable = damageable;
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
