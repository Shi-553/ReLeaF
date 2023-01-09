using TMPro;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("ダメージ値エフェクトの情報")]
    [CreateAssetMenu(menuName = "UI/DamageValueEffectInfo")]
    class DamageValueEffectInfo : ScriptableObject
    {
        [SerializeField, Rename("アニメーション")]
        AnimationClip initAnimation;
        public AnimationClip InitAnimation => initAnimation;

        [SerializeField, Rename("普通のダメージのスプライトアセット")]
        TMP_SpriteAsset normalDamageSpriteAsset;
        public TMP_SpriteAsset NormalDamageSpriteAsset => normalDamageSpriteAsset;

        [SerializeField, Rename("大きなダメージのスプライトアセット")]
        TMP_SpriteAsset highDamageSpriteAsset;
        public TMP_SpriteAsset HighDamageSpriteAsset => highDamageSpriteAsset;

        [SerializeField, Rename("大きなダメージとなる閾値")]
        int highDamageThreshold;
        public int HighDamageThreshold => highDamageThreshold;

        [SerializeField, Rename("サイズ変化で考慮する最大ダメージ")]
        float maxDamage = 10;
        public float MaxDamage => maxDamage;

        [SerializeField, Rename("最小サイズ")]
        float minSize = 30;
        public float MinSize => minSize;

        [SerializeField, Rename("最大サイズ")]
        float maxSize = 70;
        public float MaxSize => maxSize;

        [SerializeField, Rename("表示座標のオフセット")]
        Vector3 offset;
        public Vector3 Offset => offset;
        [SerializeField, Rename("表示座標のランダムオフセットの最大")]
        Vector3 randomOffsetMax;
        public Vector3 RandomOffsetMax => randomOffsetMax;
    }
}
