using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("塗り範囲拡大アイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/ExpandGreeningRangeItemInfo")]
    public class ExpandGreeningRangeItemInfo : ItemBaseInfo
    {
        [SerializeField, Rename("拡大範囲量(nマス/秒)")]
        float range = 2;
        public float Range => range;
        [SerializeField, Rename("効果時間(秒)")]
        float duration = 10;
        public float Duration => duration;

        public override string Description => description
            .Replace("{" + nameof(range) + "}", range.ToString("0.#"))
            .Replace("{" + nameof(duration) + "}", duration.ToString("0.#"));
    }
}
