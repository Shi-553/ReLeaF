using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SharkSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        SharkSpecialPowerInfo info;
        protected override SowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => info;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
            if (!IsUsing)
            {
                base.PreviewRange(tilePos, dir, returns);
                return;
            }
            returns.Clear();

            tilePos = DungeonManager.Singleton.WorldToTilePos(RobotMover.Singleton.transform.position);
            foreach (var weakLocalTilePos in info.ThrustingList)
            {
                var pos = tilePos + weakLocalTilePos;
                if (!DungeonManager.Singleton.TryGetTile(pos, out var tile) || !tile.CanOrAleeadyGreening(true))
                {
                    continue;
                }
                returns.Add(pos);
            }
        }
        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            GlobalCoroutine.Singleton.StartCoroutine(WaitRunning());

            GlobalCoroutine.Singleton.StartCoroutine(MovePlayer(dir));


            var localRanges = SowSeedSpecialPowerInfo.SeedLocalTilePos.GetLocalTilePosList(dir).ToList();

            var (minLocalTilePos, maxLocalTilePos) = localRanges.Aggregate(
                (localRanges[0], localRanges[0]),
                   (accumulator, o) =>
                   (Vector2Int.Min(o, accumulator.Item1), Vector2Int.Max(o, accumulator.Item2)));

            if (dir.y != 0)
            {
                var centerX = (minLocalTilePos.x + maxLocalTilePos.x) / 2;
                minLocalTilePos.x = centerX;
                maxLocalTilePos.x = centerX;
            }
            else
            {
                var centerY = (minLocalTilePos.y + maxLocalTilePos.y) / 2;
                minLocalTilePos.y = centerY;
                maxLocalTilePos.y = centerY;
            }

            if (dir.y < 0 || dir.x < 0)
            {
                (minLocalTilePos, maxLocalTilePos) = (maxLocalTilePos, minLocalTilePos);
            }

            var minPos = DungeonManager.Singleton.TilePosToWorld(minLocalTilePos + tilePos);
            var maxPos = DungeonManager.Singleton.TilePosToWorld(maxLocalTilePos + tilePos);

            var mover = RobotMover.Singleton;

            mover.transform.position = minPos;

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
                        if (local.y + tilePos.y == currentTilePos.y)
                        {
                            DungeonManager.Singleton.SowSeed(local + tilePos, true);
                            return true;
                        }
                        return false;
                    });
                }
                else
                {
                    localRanges.RemoveAll(local =>
                    {
                        if (local.x + tilePos.x == currentTilePos.x)
                        {
                            DungeonManager.Singleton.SowSeed(local + tilePos, true);
                            return true;
                        }
                        return false;
                    });

                }
                mover.UpdateManualOperation(mover.transform.position + (Vector3)(Vector2)dir, info.Speed, false);

                if (!isRunning)
                    break;

                yield return null;
            }

            mover.IsStop = true;
            tilePos = DungeonManager.Singleton.WorldToTilePos(mover.transform.position);
            GlobalCoroutine.Singleton.StartCoroutine(mover.GetComponent<RobotAnimation>().Thrust());
            yield return new WaitForSeconds(0.5f);

            foreach (var localPos in info.ThrustingList)
            {
                DungeonManager.Singleton.SowSeed(tilePos + localPos, true);
            }
            mover.IsStop = false;
        }

        bool isRunning = false;
        IEnumerator WaitRunning()
        {
            isRunning = true;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;

                if (UseCount > 1 || time > info.DashDuration)
                    break;

                yield return null;
            }
            isRunning = false;
        }
        IEnumerator MovePlayer(Vector2Int dir)
        {
            PlayerMover.Singleton.StartSpecialMove(dir, info.Speed);
            var invincible = PlayerCore.Singleton.IsInvincible;
            PlayerCore.Singleton.IsInvincible = true;

            yield return new WaitWhile(() => isRunning);


            PlayerMover.Singleton.FinishSpecialMove();

            PlayerCore.Singleton.IsInvincible = invincible;

        }
    }
}
