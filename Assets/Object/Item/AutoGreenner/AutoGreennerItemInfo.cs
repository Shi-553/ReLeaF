using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("自動緑化アイテムのパラメータ")]
    [CreateAssetMenu(menuName = "Item/AutoGreennerItemInfo")]
    public class AutoGreennerItemInfo : ItemBaseInfo
    {
        [SerializeField, Rename("範囲"), EditTilePos(Direction.NONE, true)]
        ArrayWrapper<Vector2Int> ranges;

        public Vector2Int[] Ranges => ranges.Value;


        [SerializeField, Rename("1回の緑化にかかる時間")]
        float oneGreeningTime = 0.5f;
        public float OneGreeningTime => oneGreeningTime;


        [SerializeField, Rename("緑化時のSE")]
        protected AudioInfo greeningSe;
        public AudioInfo GreeningSe => greeningSe;
    }
}
