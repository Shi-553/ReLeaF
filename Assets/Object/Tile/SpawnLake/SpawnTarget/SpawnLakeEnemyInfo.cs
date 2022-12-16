using Pickle;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("“G‚ª—N‚­ŒÎ‚Ìî•ñ")]
    [CreateAssetMenu(menuName = "Tile/SpawnLake/SpawnLakeEnemyInfo")]
    public class SpawnLakeEnemyInfo : TileObjectInfo, IMultipleVisual
    {
        public enum SpawnLakeType
        {
            Shark,
            Crab,
            Max
        }

        [SerializeField, Rename("—N‚©‚¹‚é“G‚Ìƒ^ƒCƒv")]
        SpawnLakeType type;
        public SpawnLakeType Type => type;

        [SerializeField, Rename("—N‚©‚¹‚é“G‚ÌƒvƒŒƒnƒu"), Pickle]
        EnemyMover enemyPrefab;
        public EnemyMover EnemyPrefab => enemyPrefab;


        [SerializeField, Rename("‰½•b–ˆ‚É—N‚©‚¹‚é‚©")]
        float spwanInterval = 10.0f;
        public float SpwanInterval => spwanInterval;

        [SerializeField, Rename("ƒXƒ|[ƒ“ƒAƒjƒ[ƒVƒ‡ƒ“‚ÌŽžŠÔ")]
        float spwanInitAnimationTime = 1.0f;
        public float SpwanInitAnimationTime => spwanInitAnimationTime;

        public int VisualType => Type.ToInt32();
        public int VisualMax => SpawnLakeType.Max.ToInt32();
    }
}
