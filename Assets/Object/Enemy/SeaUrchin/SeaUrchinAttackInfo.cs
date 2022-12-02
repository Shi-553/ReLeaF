using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("SeaUrchinの攻撃パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/SeaUrchin/SeaUrchinAttackInfo")]
    class SeaUrchinAttackInfo : EnemyAttackInfo
    {
        [SerializeField, Rename("発射するトゲ")]
        Spine spinePrefab;
        public Spine SpinePrefab => spinePrefab;
        [SerializeField, Rename("攻撃時間(秒)")]
        float attackTime = 1.0f;
        public float AttackTime => attackTime;
    }
}
