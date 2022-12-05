using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("ダメージ値エフェクトの情報")]
    [CreateAssetMenu(menuName = "UI/DamageValueEffectInfo")]
    class DamageValueEffectInfo : ScriptableObject
    {
        [SerializeField, Rename("サイズ変化で考慮する最大ダメージ")]
        float maxDamage = 10;
        public float MaxDamage => maxDamage;

        [SerializeField, Rename("最小サイズ")]
        float minSize = 30;
        public float MinSize => minSize;

        [SerializeField, Rename("最大サイズ")]
        float maxSize = 70;
        public float MaxSize => maxSize;

    }
}
