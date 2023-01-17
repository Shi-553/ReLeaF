using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public abstract class SowSeedSpecialPowerBase : ItemBase
    {
        abstract protected ISowSeedSpecialPowerInfo SowSeedSpecialPowerInfo { get; }

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {

            foreach (var weakLocalTilePos in SowSeedSpecialPowerInfo.GetSeedLocalTilePos(dir))
            {
                var pos = tilePos + weakLocalTilePos;
                if (!DungeonManager.Singleton.TryGetTile(pos, out var tile) || !tile.CanOrAleadyGreening(true))
                {
                    continue;
                }
                returns.Add(pos);
            }
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            foreach (var weakLocalTilePos in SowSeedSpecialPowerInfo.GetSeedLocalTilePos(dir))
            {
                var pos = tilePos + weakLocalTilePos;
                DungeonManager.Singleton.SowSeed(pos, true);

            }
            yield break;
        }
    }
}
