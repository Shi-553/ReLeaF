using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("Crabの攻撃パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/Crab/CrabAttackInfo")]
    public class EnemyDontMoveAttackInfo : EnemyAttackInfo
    {

        [SerializeField, Rename("攻撃モーション開始から実際にダメージ発生までの時間(秒)")]
        float attackBeforeDamageTime = 0.1f;
        public float AttackBeforeDamageTime => attackBeforeDamageTime;
        [SerializeField, Rename("攻撃力")]
        float atk = 5;
        public float ATK => atk;

        [SerializeField, Rename("攻撃するマス", "(上向きが基準のローカルポジション)")]
        LocalTilePos attackLocalTilePos;
        public LocalTilePos AttackLocalTilePos => attackLocalTilePos;

        [SerializeField, Rename("攻撃のノックバック力(nマス/秒)　減衰あり")]
        float knockBackPower = 4.0f;
        public float KnockBackPower => knockBackPower;
    }
}
