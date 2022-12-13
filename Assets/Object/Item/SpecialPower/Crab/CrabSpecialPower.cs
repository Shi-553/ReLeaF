using System.Collections;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class CrabSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        CrabSpecialPowerInfo sowSeedSpecialPowerInfo;
        protected override SowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => sowSeedSpecialPowerInfo;

        public override IEnumerator Use(Vector2Int tilePos, Vector2Int dir)
        {
            var localRanges = SowSeedSpecialPowerInfo.SeedLocalTilePos.GetLocalTilePosList(dir).ToList();

            var (minLocalTilePos, maxLocalTilePos) = localRanges.Aggregate(
                (localRanges[0], localRanges[0]),
                   (accumulator, o) =>
                   (Vector2Int.Min(o, accumulator.Item1), Vector2Int.Max(o, accumulator.Item2)));

            if (dir.y != 0)
            {
                var centerY = (minLocalTilePos.y + maxLocalTilePos.y) / 2;
                minLocalTilePos.y = centerY;
                maxLocalTilePos.y = centerY;
            }
            else
            {
                var centerX = (minLocalTilePos.x + maxLocalTilePos.x) / 2;
                minLocalTilePos.x = centerX;
                maxLocalTilePos.x = centerX;
            }

            var minPos = DungeonManager.Singleton.TilePosToWorld(minLocalTilePos + tilePos);
            var maxPos = DungeonManager.Singleton.TilePosToWorld(maxLocalTilePos + tilePos);

            var mover = RobotMover.Singleton;

            mover.UpdateManualOperation(minPos, sowSeedSpecialPowerInfo.BeforeSpeed, true);

            yield return new WaitUntil(() => !mover.UseManualOperation);


            mover.UpdateManualOperation(maxPos, sowSeedSpecialPowerInfo.Speed, false);


            Vector2Int currentTilePos = tilePos + Vector2Int.one;
            while (true)
            {
                var temp = DungeonManager.Singleton.WorldToTilePos(mover.transform.position);

                if (temp == currentTilePos)
                    yield return null;

                currentTilePos = temp;

                if (dir.y != 0)
                {
                    localRanges.RemoveAll(local =>
                    {
                        if (local.x + tilePos.x == currentTilePos.x)
                        {
                            DungeonManager.Singleton.SowSeed(local + tilePos);
                            return true;
                        }
                        return false;
                    });
                }
                else
                {
                    localRanges.RemoveAll(local =>
                    {
                        if (local.y + tilePos.y == currentTilePos.y)
                        {
                            DungeonManager.Singleton.SowSeed(local + tilePos);
                            return true;
                        }
                        return false;
                    });

                }
                if (!mover.UseManualOperation)
                {
                    yield break;
                }

                yield return null;
            }
        }
    }
}
