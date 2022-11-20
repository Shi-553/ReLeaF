using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("Sharkの攻撃パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/Shark/AttackInfo")]
    public class SharkAttackInfo : EnemyAttackInfo
    {
        public DamageType DamageType => DamageType.Direct;

        [SerializeField, Rename("攻撃力")]
        float atk;
        public float ATK => atk;
        [SerializeField, Rename("攻撃時の移動スピード(nマス/秒)")]
        float speed = 4.0f;
        public float Speed => speed;
        [SerializeField, Rename("攻撃範囲(前方nマス)")]
        int range = 5;
        public int Range => range;

        [SerializeField, Rename("攻撃のノックバック力(nマス/秒)　減衰あり")]
        float knockBackPower = 4.0f;
        public float KnockBackPower => knockBackPower;

    }
}
