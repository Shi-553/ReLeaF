using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("カニのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/CrabSpecialPower")]
    public class CrabSpecialPowerInfo : ScriptableObject, ISowSeedSpecialPowerInfo
    {
        [SerializeField, Rename("種をまくマス")]
        LocalTilePos seedLocalTilePos;

        public Vector2Int[] GetSeedLocalTilePos(Vector2Int dir) => seedLocalTilePos.GetLocalTilePosList(dir);


        [SerializeField, Rename("緑化開始地点にいくスピード(nマス/秒)")]
        float beforeSpeed = 10;
        public float BeforeSpeed => beforeSpeed;

        [SerializeField, Rename("緑化時のスピード(nマス/秒)")]
        float speed = 10;
        public float Speed => speed;

    }
}
