using UnityEngine;

namespace ReLeaf
{
    public class SharkSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        SharkSpecialPowerInfo info;
        protected override SowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => info;
    }
}
