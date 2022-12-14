using Pickle;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵が湧く湖の情報")]
    [CreateAssetMenu(menuName = "Tile/SpawnLake/SpawnLakeEnemyInfo")]
    public class SpawnLakeEnemyInfo : TileObjectInfo, IMultipleVisual
    {
        public enum SpawnLakeType
        {
            Shark,
            Crab,
            Max
        }

        [SerializeField, Rename("湧かせる敵のタイプ")]
        SpawnLakeType type;
        public SpawnLakeType Type => type;

        [SerializeField, Rename("湧かせる敵のプレハブ"), Pickle]
        EnemyMover enemyPrefab;
        public EnemyMover EnemyPrefab => enemyPrefab;


        [SerializeField, Rename("何秒毎に湧かせるか")]
        float spwanInterval = 10.0f;
        public float SpwanInterval => spwanInterval;

        [SerializeField, Rename("スポーンアニメーションの時間")]
        float spwanInitAnimationTime = 1.0f;
        public float SpwanInitAnimationTime => spwanInitAnimationTime;

        public int VisualType => Type.ToInt32();
        public int VisualMax => SpawnLakeType.Max.ToInt32();
    }
}
