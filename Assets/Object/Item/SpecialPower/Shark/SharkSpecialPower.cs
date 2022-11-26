using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SharkSpecialPower : ItemBase
    {
        [SerializeField]
        SharkSpecialPowerInfo sharkSpecialPowerInfo;

        public override List<Vector2Int> PreviewRange(Vector2Int tilePos, Vector2Int dir)
        {
            var returns = new List<Vector2Int>(sharkSpecialPowerInfo.SeedLocalTilePos.Length);
            foreach (var weakLocalTilePos in sharkSpecialPowerInfo.SeedLocalTilePos)
            {
                var pos = tilePos + MathExtension.GetRotatedLocalPos(dir, weakLocalTilePos);
                if (!DungeonManager.Instance.CanSowSeed(pos, PlantType.Foundation))
                {
                    continue;
                }
                returns.Add( pos);
            }
            return returns;
        }

        public override void Use(Vector2Int tilePos, Vector2Int dir)
        {
            foreach (var weakLocalTilePos in sharkSpecialPowerInfo.SeedLocalTilePos)
            {
                var pos = tilePos + MathExtension.GetRotatedLocalPos(dir, weakLocalTilePos);
                DungeonManager.Instance.SowSeed(pos, PlantType.Foundation);

            }
        }
    }
}
