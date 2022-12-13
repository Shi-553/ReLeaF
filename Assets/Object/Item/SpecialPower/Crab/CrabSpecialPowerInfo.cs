using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("カニのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/Crab/CrabSpecialPower")]
    public class CrabSpecialPowerInfo : SowSeedSpecialPowerInfo
    {
        [SerializeField, Rename("緑化開始地点にいくスピード")]
        float beforeSpeed = 10;
        public float BeforeSpeed => beforeSpeed;

        [SerializeField, Rename("緑化時のスピード")]
        float speed = 10;
        public float Speed => speed;

    }
}
