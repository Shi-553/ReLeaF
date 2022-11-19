using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SharkSpecialPower :  ItemBase
    {
        [SerializeField]
        SharkSpecialPowerInfo sharkSpecialPowerInfo;

        public override bool Use(Vector2Int tilePos, Vector2Int dir)
        {
            foreach (var weakLocalTilePos in sharkSpecialPowerInfo.WeakLocalTilePos)
            {
                var pos= tilePos + MathExtension.GetRotatedLocalPos(dir, weakLocalTilePos);
                DungeonManager.Instance.SowSeed(pos, PlantType.Foundation);

            }
            return true;
        }
    }
}
