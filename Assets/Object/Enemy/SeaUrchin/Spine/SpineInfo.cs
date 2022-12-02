using UnityEngine;
using Utility;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/SeaUrchin/SpineInfo")]
    class SpineInfo : ScriptableObject
    {

        [SerializeField, Rename("スポーン時のアニメーションまでのディレイ(秒)")]
        float initAnimationDelay = 0.1f;
        public float InitAnimationDelay => initAnimationDelay;

        [SerializeField, Rename("スポーン時のアニメーションの長さ(秒)")]
        float initAnimationTime = 0.3f;
        public float InitAnimationTime => initAnimationTime;


        [SerializeField, Rename("スポーン時のアニメーションの移動速度(nマス/秒)")]
        float initAnimationSpeed = 0.3f;
        public float InitAnimationSpeed => initAnimationSpeed;

        [SerializeField, Rename("速度(nマス/秒)")]
        float speed = 1.0f;
        public float Speed => speed;

        [SerializeField, Rename("攻撃力")]
        float atk = 2;
        public float ATK => atk;

        [SerializeField, Rename("攻撃のノックバック力(nマス/秒)　減衰あり")]
        float knockBackPower = 4.0f;
        public float KnockBackPower => knockBackPower;
    }
}
