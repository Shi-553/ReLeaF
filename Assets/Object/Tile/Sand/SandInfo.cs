using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("Sandタイルオブジェクトの情報")]
    [CreateAssetMenu(menuName = "Tile/SandInfo")]
    public class SandInfo : TileObjectInfo
    {
        [SerializeField]
        Material roomMat;
        public Material RoomMat => roomMat;

        [SerializeField]
        Material noRoomMat;
        public Material NoRoomMat=> noRoomMat;
    }
}
