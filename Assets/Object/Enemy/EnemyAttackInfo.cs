using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵({asset.dirname})の攻撃パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/EnemyAttackInfo")]
    public class EnemyAttackInfo : ScriptableObject
    {
        [SerializeField,Rename("攻撃した後の待機時間(秒)")]
        float coolTime = 1.0f;
        public float CoolTime => coolTime;
        [SerializeField, Rename("攻撃前の準備時間(秒)")]
        float aimTime = 1.0f;
        public float AimTime => aimTime;
    }
}
