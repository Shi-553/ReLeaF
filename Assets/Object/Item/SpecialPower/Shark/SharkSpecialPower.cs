using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SharkSpecialPower : SowSeedSpecialPowerBase
    {
        SharkSpecialPowerInfo Info => ItemBaseInfo as SharkSpecialPowerInfo;
        protected override ISowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => Info;

        Vector2Int previewdTilePos;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
            if (!IsUsing)
            {
                previewdTilePos = tilePos;
                base.PreviewRange(tilePos, dir, returns);
                return;
            }

            previewdTilePos = DungeonManager.Singleton.WorldToTilePos(RobotMover.Singleton.transform.position);
            foreach (var weakLocalTilePos in Info.ThrustingList)
            {
                var pos = previewdTilePos + weakLocalTilePos;
                if (!DungeonManager.Singleton.TryGetTile(pos, out var tile) || !tile.CanOrAleadyGreening(true))
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

            using var _ = RobotGreening.Singleton.StartGreening();


            var localRanges = SowSeedSpecialPowerInfo.GetSeedLocalTilePos(dir).ToList();

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

            var minPos = PlayerCore.Singleton.transform.position;
            var maxPos = DungeonManager.Singleton.TilePosToWorld(maxLocalTilePos + tilePos);

            var mover = RobotMover.Singleton;

            mover.GetComponentInChildren<SpriteRenderer>().sortingOrder++;
            mover.transform.position = minPos;

            SEManager.Singleton.Play(Info.SeSharkSpecialMove);

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

                mover.UpdateManualOperation(PlayerMover.Singleton.transform.position + (Vector3)(Vector2)dir * 2.0f, Info.Speed, false);

                if (!isRunning)
                    break;

                yield return null;
            }

            mover.IsStop = true;
            mover.GetComponent<RobotAnimation>().Thrust();
            yield return new WaitForSeconds(0.5f);
            SEManager.Singleton.Play(Info.SeSharkSpecial);
            foreach (var localPos in Info.ThrustingList)
            {
                DungeonManager.Singleton.SowSeed(previewdTilePos + localPos, true);
            }

            mover.GetComponentInChildren<SpriteRenderer>().sortingOrder--;

            mover.IsStop = false;
        }

        bool isRunning = false;
        IEnumerator WaitRunning()
        {
            isRunning = true;
            float time = 0;

            var playerRigidbody = PlayerMover.Singleton.GetComponent<Rigidbody2D>();
            var beforePlayerPos = playerRigidbody.position;
            var checkPlayerPos = false;

            while (true)
            {
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;


                var playerPos = playerRigidbody.position;

                if (UseCount > 1 && time > 0.05f)
                    break;
                if (time > Info.DashDuration)
                    break;

                checkPlayerPos = !checkPlayerPos;

                if (checkPlayerPos)
                {
                    if ((beforePlayerPos - playerPos).sqrMagnitude < 0.001f)
                        break;
                    beforePlayerPos = playerPos;
                }
            }
            isRunning = false;
        }
        IEnumerator MovePlayer(Vector2Int dir)
        {
            PlayerMover.Singleton.StartSpecialMove(dir, Info.Speed);
            PlayerCore.Singleton.AddInvincible(true);
            var playerAnimation = PlayerCore.Singleton.GetComponent<PlayerAnimation>();
            playerAnimation.Grasp(true);

            yield return new WaitWhile(() => isRunning);

            PlayerMover.Singleton.FinishSpecialMove();

            playerAnimation.Grasp(false);
            PlayerCore.Singleton.RemoveInvincible();

        }
    }
}
