using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("緑化範囲拡大アイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/AddGreeningRangeItemInfo")]
    public class AddGreeningRangeItemInfo : ItemBaseInfo
    {
        [SerializeField, Rename("緑化範囲拡大量(nマス/秒)")]
        float addRange = 2;
        public float AddRange => addRange;
        [SerializeField, Rename("効果時間(秒)")]
        float duration = 10;
        public float Duration => duration;

        public override string Description => description
            .Replace("{" + nameof(addRange) + "}", addRange.ToString("0.#"))
            .Replace("{" + nameof(duration) + "}", duration.ToString("0.#"));
    }
}
