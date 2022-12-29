using System.Linq;

namespace ReLeaf
{
    public class SpawnTarget : TileObject
    {
        public SpawnLakeEnemyInfo EnemyInfo => Info as SpawnLakeEnemyInfo;

        public override TileObject InstancingTarget => transform.GetComponentsInChildren<TileObject>().Last();
    }
}
