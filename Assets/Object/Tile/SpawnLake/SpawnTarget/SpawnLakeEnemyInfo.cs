using Pickle;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("�G���N���΂̏��")]
    [CreateAssetMenu(menuName = "Tile/SpawnLake/SpawnLakeEnemyInfo")]
    public class SpawnLakeEnemyInfo : TileObjectInfo, IMultipleVisual
    {
        public enum SpawnLakeType
        {
            Shark,
            Crab,
            Max
        }

        [SerializeField, Rename("�N������G�̃^�C�v")]
        SpawnLakeType type;
        public SpawnLakeType Type => type;

        [SerializeField, Rename("�N������G�̃v���n�u"), Pickle]
        EnemyMover enemyPrefab;
        public EnemyMover EnemyPrefab => enemyPrefab;


        [SerializeField, Rename("���b���ɗN�����邩")]
        float spwanInterval = 10.0f;
        public float SpwanInterval => spwanInterval;

        [SerializeField, Rename("�X�|�[���A�j���[�V�����̎���")]
        float spwanInitAnimationTime = 1.0f;
        public float SpwanInitAnimationTime => spwanInitAnimationTime;

        public int VisualType => Type.ToInt32();
        public int VisualMax => SpawnLakeType.Max.ToInt32();
    }
}
