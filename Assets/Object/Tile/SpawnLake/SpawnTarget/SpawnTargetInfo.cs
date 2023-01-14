using Pickle;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵が湧く湖の情報")]
    [CreateAssetMenu(menuName = "Tile/SpawnLake/SpawnLakeEnemyInfo")]
    public class SpawnTargetInfo : ScriptableObject
    {

        [SerializeField, Rename("湧かせる敵のプレハブ"), Pickle]
        EnemyMover enemyPrefab;
        public EnemyMover EnemyPrefab => enemyPrefab;


        [SerializeField, Rename("スポーンアニメーションの時間")]
        float spwanInitAnimationTime = 1.0f;
        public float SpwanInitAnimationTime => spwanInitAnimationTime;

    }
}
