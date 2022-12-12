using UnityEngine;

namespace ReLeaf
{
    public class CrabSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        CrabSpecialPowerInfo sowSeedSpecialPowerInfo;
        protected override SowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => sowSeedSpecialPowerInfo;

        //public override IEnumerator Use(Vector2Int tilePos, Vector2Int dir)
        //{
        //    StartCoroutine(base.Use(tilePos, dir));
        //    var localRanges = SowSeedSpecialPowerInfo.SeedLocalTilePos.GetLocalTilePosList(dir);


        //    yield break;
        //}
    }
}
