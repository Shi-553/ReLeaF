using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("Sharkのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : ScriptableObject, ISowSeedSpecialPowerInfo
    {
        [SerializeField, Rename("種をまくマス")]
        LocalTilePos seedLocalTilePos;
        public Vector2Int[] GetSeedLocalTilePos(Vector2Int dir) => seedLocalTilePos.GetLocalTilePosList(dir);


        [SerializeField, Rename("突きで緑化するローカルポジション"), EditTilePos(Direction.NONE, true)]
        ArrayWrapper<Vector2Int> thrustingList;
        public Vector2Int[] ThrustingList => thrustingList.Value;

        [SerializeField, Rename("スペシャルダッシュスピード")]
        float speed = 10;
        public float Speed => speed;

        [SerializeField, Rename("スペシャルダッシュ最大時間")]
        float dashDuration = 1;
        public float DashDuration => dashDuration;
    }
}
