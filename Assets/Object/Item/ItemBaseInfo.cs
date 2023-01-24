using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum ItemType
    {
        AddGreeningRange,
        AutoGreening,
        EnemyStan,
        HPRecover,
        CrabSpecialPower,
        SharkSpecialPower,
        SeaUrchineSpecialPower,
        SpeedUp,
        Max
    }
    [ClassSummary("{asset.dirname}のパラメータ")]
    [CreateAssetMenu(menuName = "Item/ItemBaseInfo")]
    public class ItemBaseInfo : ScriptableObject
    {
        [SerializeField]
        ItemType itemType;
        public ItemType ItemType => itemType;

        [SerializeField, Rename("使用時のSE")]
        protected AudioInfo useSe;
        public AudioInfo UseSe => useSe;

        [SerializeField, Rename("取得時にすぐ効果を発動するか")]
        protected bool isImmediate = false;
        public bool IsImmediate => isImmediate;

        [SerializeField, Rename("スポーン時のアニメーション")]
        protected AnimationClip initAnimation;
        public AnimationClip InitAnimation => initAnimation;

        [SerializeField, Rename("選択時の説明")]
        protected string description;
        public virtual string Description => description;
    }
}
