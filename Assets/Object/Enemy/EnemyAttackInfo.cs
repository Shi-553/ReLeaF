using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/EnemyAttackInfo")]
    public class EnemyAttackInfo : ScriptableObject
    {
        [SerializeField]
        float coolTime = 1.0f;
        public float CoolTime => coolTime;
        [SerializeField]
        float aimTime = 1.0f;
        public float AimTime => aimTime;
    }
}
