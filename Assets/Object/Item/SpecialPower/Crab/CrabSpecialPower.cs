using UnityEngine;

namespace ReLeaf
{
    public class CrabSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        SowSeedSpecialPowerInfo sowSeedSpecialPowerInfo;
        protected override SowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => sowSeedSpecialPowerInfo;

    }
}
