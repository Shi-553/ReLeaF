using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("Sharkのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : SowSeedSpecialPowerInfo
    {

        [SerializeField, Rename("突きで緑化するローカルポジション"), EditTilePos(Direction.UP)]
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
