using System.Linq;

namespace ReLeaf
{
    public class SpawnTarget : TileObject
    {
        public SpawnLakeEnemyInfo EnemyInfo => Info as SpawnLakeEnemyInfo;

        TileObject instancingTarget = null;
        public override TileObject InstancingTarget => instancingTarget;

        protected override void FasterInitOnlyOnceImpl()
        {
            instancingTarget = transform.GetComponentsInChildren<TileObject>(true).Last();
        }
    }
}
