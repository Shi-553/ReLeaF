using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class AddGreeningRangeItem : ItemBase
    {
        AddGreeningRangeItemInfo Info => ItemBaseInfo as AddGreeningRangeItemInfo;
       

        Queue<TimePos> lateGreenings = new();

        Coroutine lateGreeningCo;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            PlayerMover.Singleton.OnGreening += OnGreening;
            lateGreeningCo = PlayerMover.Singleton.StartCoroutine(LateGreening());
            IsFinishUse = true;
            StatusChangeManager.Singleton.AddStatus(new(Info.Duration, Icon));

            yield return new WaitForSeconds(Info.Duration);

            PlayerMover.Singleton.StopCoroutine(lateGreeningCo);
        }

        private void OnGreening(Vector2Int tilePos)
        {
            lateGreenings.Enqueue(new(Time.time, tilePos));
        }

        IEnumerator LateGreening()
        {
            while (true)
            {
                var now = Time.time;

                while (true)
                {
                    if (!lateGreenings.TryPeek(out var timePos))
                    {
                        break;
                    }
                    if (now - timePos.time < 0.5f)
                    {
                        break;
                    }

                    lateGreenings.Dequeue();

                    if (DungeonManager.Singleton.TryGetTile(timePos.tilePos, out var tile) && tile.IsAlreadyGreening)
                    {
                        DungeonManager.Singleton.SowSeed(timePos.tilePos + Vector2Int.up, false, false, false);
                        DungeonManager.Singleton.SowSeed(timePos.tilePos + Vector2Int.down, false, false, false);
                        DungeonManager.Singleton.SowSeed(timePos.tilePos + Vector2Int.left, false, false, false);
                        DungeonManager.Singleton.SowSeed(timePos.tilePos + Vector2Int.right, false, false, false);
                    }
                }
                yield return null;
            }
        }

        struct TimePos : IEquatable<TimePos>
        {
            public float time;
            public Vector2Int tilePos;

            public TimePos(float time, Vector2Int tilePos)
            {
                this.time = time;
                this.tilePos = tilePos;
            }

            public override bool Equals(object obj)
            {
                return obj is TimePos pos && Equals(pos);
            }

            public bool Equals(TimePos other)
            {
                return tilePos.Equals(other.tilePos);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(tilePos);
            }

            public static bool operator ==(TimePos left, TimePos right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TimePos left, TimePos right)
            {
                return !(left == right);
            }
        }
    }
}
