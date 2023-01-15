using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class AutoGreennerItem : ItemBase
    {
        AutoGreennerItemInfo Info => ItemBaseInfo as AutoGreennerItemInfo;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
            foreach (var range in Info.Ranges)
            {
                var pos = tilePos + range;
                if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                {
                    continue;
                }
                if (tile.CanOrAleeadyGreening(false))
                {
                    returns.Add(pos);
                }
            }
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            var worldPos = DungeonManager.Singleton.TilePosToWorld(tilePos);

            IsFinishUse = true;

            HashSet<Vector2Int> greenMap = new(Info.Ranges.Length);

            int count = (Mathf.FloorToInt(Mathf.Sqrt(Info.Ranges.Length)) + 1) / 2;

            var buffetLength = ((count * 2) - 1) * ((count * 2) - 1) - (((count - 1) * 2) - 1) * (((count - 1) * 2) - 1);

            List<Vector2Int> target = new(buffetLength);
            List<Vector2Int> buffer = new(buffetLength);

            target.Add(tilePos);

            int targetCount = target.Count;

            var greeningWait = new WaitForSeconds(Info.OneGreeningTime);

            for (int c = 0; c < count; c++)
            {
                int bufferIndex = 0;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];
                    // 緑化
                    DungeonManager.Singleton.SowSeed(pos, false);
                }

                if (targetCount > 0)
                    SEManager.Singleton.Play(Info.GreeningSe, worldPos);

                yield return greeningWait;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];

                    if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                    {
                        continue;
                    }
                    if (tile.TileType != TileType.Foundation || !tile.IsAlreadyGreening)
                    {
                        continue;
                    }

                    // 既に存在するときに上書きすることでClear()をしなくてよくなる最適化
                    void BufferAdd(Vector2Int nextPos)
                    {
                        if (!greenMap.Add(nextPos))
                        {
                            return;
                        }

                        if (buffer.Count == bufferIndex)
                            buffer.Add(nextPos);
                        else
                            buffer[bufferIndex] = nextPos;

                        bufferIndex++;
                    }

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            BufferAdd(new Vector2Int(pos.x + x, pos.y + y));
                        }
                    }
                }


                if (bufferIndex == 0)
                    yield break;
                targetCount = bufferIndex;

                // ターゲットとバッファをスワップしつつループすることでメモリ節約
                (buffer, target) = (target, buffer);

            }
        }
    }
}
