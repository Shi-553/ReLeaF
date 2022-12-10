using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("ぐちゃぐちゃな地面のパラメータ")]
    [CreateAssetMenu(menuName = "Tile/MessyInfo")]
    public class MessyInfo : TileObjectInfo
    {
        [SerializeField, Rename("砂になるまでの時間")]
        float cureTime = 10;
        public float CureTime => cureTime;


        [SerializeField, Rename("ぐちゃぐちゃになったときの音")]
        AudioInfo changeSand;
        public AudioInfo ChangeSand => changeSand;
    }
}
