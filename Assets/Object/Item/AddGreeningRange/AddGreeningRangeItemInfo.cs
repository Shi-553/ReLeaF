using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("緑化範囲拡大アイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/AddGreeningRangeItemInfo")]
    public class AddGreeningRangeItemInfo : DurationItem
    {
        [SerializeField, Rename("緑化範囲拡大量(nマス/秒)")]
        float addRange = 2;
        public float AddRange => addRange;
    }
}
