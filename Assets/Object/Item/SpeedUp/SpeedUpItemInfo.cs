using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("スピードアップアイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpeedUpItemInfo")]
    public class SpeedUpItemInfo : DurationItem
    {
        [SerializeField, Rename("スピードアップ量(nマス/秒)")]
        float speedUp = 2;
        public float SpeedUp => speedUp;

    }
}
