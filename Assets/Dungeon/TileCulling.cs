using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class TileCulling : SingletonBase<TileCulling>
    {
        [SerializeField]
        int cullingDistance = 21;
        int cullingDistanceSq;

        PlayerMover mover;

        List<Vector2Int>[] quadrants = new List<Vector2Int>[4] { new(), new(), new(), new() };

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {

        }

        bool canCulling = true;
        public IEnumerator StopCulling()
        {
            canCulling = false;

            while (true)
            {
                int count = 0;
                foreach (var tile in DungeonManager.Singleton.TileDic.Values)
                {
                    if (!tile.EnableRenderer)
                    {
                        tile.EnableRenderer = true;
                        count++;
                        if (count > 10000)
                            break;
                    }
                }
                if (count == 0)
                    break;
                yield return null;
            }
        }

        public IEnumerator RestartCulling()
        {
            canCulling = true;

            while (true)
            {
                int count = 0;

                foreach (var (pos, tile) in DungeonManager.Singleton.TileDic)
                {
                    if (!tile.EnableRenderer)
                        continue;

                    var distanceSq = (pos - mover.TilePos).sqrMagnitude;

                    if (distanceSq > cullingDistanceSq)
                    {
                        tile.EnableRenderer = false;
                        count++;
                        if (count > 1000)
                            break;
                    }
                }
                if (count == 0)
                    break;
                yield return null;
            }
        }

        IEnumerator Start()
        {
            cullingDistanceSq = cullingDistance * cullingDistance;

            TryGetComponent(out mover);

            yield return null;

            foreach (var (pos, tile) in DungeonManager.Singleton.TileDic)
            {
                var distanceSq = (pos - mover.TilePos).sqrMagnitude;

                if (distanceSq > cullingDistanceSq)
                {
                    tile.EnableCollider = false;
                    tile.EnableRenderer = false;
                }
            }

            HashSet<Vector2Int> borderPoss = new();
            for (int x = -cullingDistance; x <= cullingDistance; x++)
            {
                for (int y = -cullingDistance; y <= cullingDistance; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var distanceSq = pos.sqrMagnitude;

                    if (distanceSq <= cullingDistanceSq)
                        borderPoss.Add(pos);
                }
            }


            var removes = borderPoss.Where(p =>
            borderPoss.Contains(p + Vector2Int.down) &&
            borderPoss.Contains(p + Vector2Int.up) &&
            borderPoss.Contains(p + Vector2Int.left) &&
            borderPoss.Contains(p + Vector2Int.right))
                .ToArray();

            borderPoss.ExceptWith(removes);

            foreach (var pos in borderPoss)
            {
                if (pos.x >= 0 && pos.y <= 0)
                {
                    quadrants[0].Add(pos);
                }
                if (pos.x >= 0 && pos.y >= 0)
                {
                    quadrants[1].Add(pos);
                }

                if (pos.x <= 0 && pos.y >= 0)
                {
                    quadrants[2].Add(pos);
                }
                if (pos.x <= 0 && pos.y <= 0)
                {
                    quadrants[3].Add(pos);
                }
            }

            mover.OnChangeTilePos += OnChangeTilePos;
        }

        private void OnChangeTilePos(Vector2Int playerPos)
        {
            var oldpos = mover.OldTilePos;
            while (true)
            {
                var dir = (playerPos - oldpos).ClampOneMagnitude();
                Move(oldpos, dir);
                oldpos += dir;

                if (oldpos == playerPos)
                    break;
            }
        }
        void Move(Vector2Int oldPos, Vector2Int dir)
        {
            var offset = (int)(Vector2.SignedAngle(Vector2.right, (Vector2)dir) / 90);

            foreach (var pos in quadrants[MathExtension.Mod(offset, 4)])
            {
                if (DungeonManager.Singleton.TryGetTile(oldPos + pos, out var tile))
                {
                    tile.EnableRenderer = true;
                    tile.EnableCollider = true;
                }
            }
            foreach (var pos in quadrants[MathExtension.Mod(offset + 1, 4)])
            {
                if (DungeonManager.Singleton.TryGetTile(oldPos + pos, out var tile))
                {
                    tile.EnableRenderer = true;
                    tile.EnableCollider = true;
                }
            }
            if (!canCulling)
                return;
            foreach (var pos in quadrants[MathExtension.Mod(offset + 2, 4)])
            {
                if (DungeonManager.Singleton.TryGetTile(oldPos + pos, out var tile))
                {
                    tile.EnableRenderer = false;
                    tile.EnableCollider = false;
                }
            }
            foreach (var pos in quadrants[MathExtension.Mod(offset + 3, 4)])
            {
                if (DungeonManager.Singleton.TryGetTile(oldPos + pos, out var tile))
                {
                    tile.EnableRenderer = false;
                    tile.EnableCollider = false;
                }
            }
        }

    }
}
