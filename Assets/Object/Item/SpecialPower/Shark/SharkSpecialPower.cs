using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SharkSpecialPower : ItemBase
    {
        [SerializeField]
        SharkSpecialPowerInfo sharkSpecialPowerInfo;

        public override List<Vector2Int> PreviewRange(Vector2Int tilePos, Vector2Int dir)
        {
            var localList = sharkSpecialPowerInfo.SeedLocalTilePos.GetLocalTilePosList(dir);

            var returns = new List<Vector2Int>(localList.Length);
            foreach (var weakLocalTilePos in localList)
            {
                var pos = tilePos + weakLocalTilePos;
                if (!DungeonManager.Singleton.CanSowSeed(pos, PlantType.Foundation, true))
                {
                    continue;
                }
                returns.Add(pos);
            }
            return returns;
        }

        public override void Use(Vector2Int tilePos, Vector2Int dir)
        {
            foreach (var weakLocalTilePos in sharkSpecialPowerInfo.SeedLocalTilePos.GetLocalTilePosList(dir))
            {
                var pos = tilePos + weakLocalTilePos;
                DungeonManager.Singleton.SowSeed(pos, PlantType.Foundation, true);

            }
        }
    }
}
