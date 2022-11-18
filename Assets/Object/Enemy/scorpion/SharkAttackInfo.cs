using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/Shark/AttackInfo")]
    public class SharkAttackInfo : EnemyAttackInfo
    {
        public DamageType DamageType => DamageType.Direct;

        [SerializeField]
        float atk;
        public float ATK => atk;
        [SerializeField]
        float speed = 4.0f;
        public float Speed => speed;
        [SerializeField]
        int range = 5;
        public int Range => range;

        [SerializeField]
        float knockBackPower = 4.0f;
        public float KnockBackPower => knockBackPower;

    }
}
