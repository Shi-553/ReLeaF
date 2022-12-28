using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("SeaUrchinのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SeaUrchinSpecialPower")]
    public class SeaUrchinSpecialPowerInfo : ScriptableObject, ISowSeedSpecialPowerInfo
    {
        [SerializeField, Rename("種をまくマス"), EditTilePos(Direction.NONE, true)]
        ArrayWrapper<Vector2Int> seedLocalTilePos;
        public Vector2Int[] GetSeedLocalTilePos(Vector2Int dir) => seedLocalTilePos.Value;

        [SerializeField, Rename("緑化開始地点までの距離(nマス)")]
        int distance = 5;
        public int Distance => distance;

        [SerializeField, Rename("緑化開始地点にいくスピード(nマス/秒)")]
        float speed = 10;
        public float Speed => speed;

        [SerializeField, Rename("これより近づいたらつつきモーションを始める、緑化開始地点までの距離(nマス)")]
        float startSowSeedDistance = 1.0f;
        public float StartSowSeedDistance => startSowSeedDistance;

    }
}
