using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("スピードアップアイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpeedUpItemInfo")]
    public class SpeedUpItemInfo : ScriptableObject
    {
        [SerializeField, Rename("スピードアップ量(nマス/秒)")]
        float speedUp = 2;
        public float SpeedUp => speedUp;
        [SerializeField, Rename("スピードアップ時間(秒)")]
        float duration = 10;
        public float Duration => duration;

    }
}
