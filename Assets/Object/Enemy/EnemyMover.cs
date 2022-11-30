using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class EnemyMover : MonoBehaviour
    {
        Rigidbody2DMover mover;


        [field: SerializeField, ReadOnly]
        public Vector2Int TilePos { get; private set; }
        [field: SerializeField, ReadOnly]
        public Vector2Int OldTilePos { get; private set; }
        public bool WasChangedTilePosPrevFrame => TilePos != OldTilePos;


        [field: SerializeField, ReadOnly]
        public Vector2Int Target { get; private set; }
        [field: SerializeField, ReadOnly]
        public Vector2Int OldTarget { get; private set; }
        public bool WasChangedTargetThisFrame => Target != OldTarget;


        [field: SerializeField, ReadOnly]
        public Vector2Int Dir { get; protected set; }

        [SerializeField]
        EnemyMoverInfo enemyMoverInfo;
        public Vector2Int TileSize => enemyMoverInfo.TileSize;


        void Start()
        {
            TryGetComponent(out mover);
            TilePos = DungeonManager.Singleton.WorldToTilePos(mover.Position);
            Dir = Vector2Int.down;
        }

        public bool Move(bool useRouting)
        {
            return Move(enemyMoverInfo.Speed, useRouting);
        }

        public bool Move(float speedOverride, bool useRouting)
        {
            OldTilePos = TilePos;
            GetCheckPoss(TilePos, Dir, buffer);

            var nextTilePos = TilePos + Dir;

            foreach (var nextPos in buffer)
            {
                if (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove)
                    return true;

            }

            if (Target == TilePos)
            {
                return true;
            }

            Vector2 worldNextTargetPos = DungeonManager.Singleton.TilePosToWorld(nextTilePos);

            var distance = worldNextTargetPos - mover.Position;
            var worldDir = distance.normalized;


            if (Vector2.Dot(worldDir, (Vector2)Dir) < 0 || (distance.sqrMagnitude < 0.001f))
            {
                var isFinish = Target == nextTilePos;
                if (isFinish)
                {
                    mover.Position = worldNextTargetPos;
                }

                TilePos = nextTilePos;

                return Target == nextTilePos;
            }

            mover.MoveDelta(DungeonManager.CELL_SIZE * speedOverride * worldDir);

            return false;
        }

        public bool UpdateTarget(Vector2Int targetTilePos)
        {
            OldTarget = Target;
            Target = targetTilePos;
            var ret = UpdateDirRouting();
            Target = routing.First();
            return ret;
        }
        public void UpdateTargetDir(Vector2Int targetTilePos, Vector2Int dir)
        {
            OldTarget = Target;
            Target = targetTilePos;
            Dir = dir;
        }



        // ここから経路探索

        public enum Direction
        {
            NONE, UP, DOWN, LEFT, RIGHT
        }
        Direction ToDirection(Vector2Int dir)
        {
            if (dir.y > 0)
                return Direction.UP;
            if (dir.y < 0)
                return Direction.DOWN;
            if (dir.x < 0)
                return Direction.LEFT;
            if (dir.x > 0)
                return Direction.RIGHT;

            return Direction.NONE;
        }
        struct Label
        {
            public Direction dir;
            public int count;

            public Label(Direction dir, int count)
            {
                this.dir = dir;
                this.count = count;
            }
        }

        // そのマスに到達したとき、来た方向を記録
        Dictionary<Vector2Int, Label> routingBuffer = new();

        Queue<Vector2Int> tempMapQueue = new Queue<Vector2Int>();

        // ゴールの手前のマスからスタートしたマスまで
        List<Vector2Int> routing = new List<Vector2Int>();
        List<Vector2Int> buffer = new();
        public IReadOnlyList<Vector2Int> Routing => routing;

        Vector2Int tempTarget;
        Vector2Int tempQueue;
        Label tempDic;

        bool UpdateDirRouting()
        {
            // ターゲットが同じタイル とりあえず↑
            if (Target == TilePos)
            {
                Dir = Vector2Int.up;
                return false;
            }

            // 移動中でターゲットも変更なし とりあえず変更なし
            if (!WasChangedTilePosPrevFrame && !WasChangedTargetThisFrame)
            {
                return true;
            }


            routingBuffer.Clear();
            tempMapQueue.Clear();

            if (!FindShortestPath())
            {
                // 到達不可能 とりあえず直線移動させる
                return false;
            }
            routing.Clear();


            // ターゲットから戻って経路を確認する
            var currnet = tempTarget;
            Direction prevDir = Direction.NONE;
            while (true)
            {
                var max = new Label(Direction.NONE, 0);
                for (int i = 0; i < TileSize.x; i++)
                {
                    for (int j = 0; j < TileSize.y; j++)
                    {
                        var c = routingBuffer[new Vector2Int(currnet.x + i, currnet.y + j)];
                        if (c.count > max.count)
                            max = c;
                    }
                }
                if (max.dir == Direction.NONE)
                {
                    Dir = prevDir.GetVector2Int();
                    return true;
                }
                var dir = max.dir.GetVector2Int();

                // 一つ戻る
                currnet -= dir;

                routing.Add(currnet);

                prevDir = max.dir;
            }
        }


        /// <summary>
        /// 最短経路を探す
        /// </summary>
        /// <returns>到達可能か</returns>
        bool FindShortestPath()
        {
            tempMapQueue.Enqueue(TilePos);

            for (int i = 0; i < TileSize.x; i++)
            {
                for (int j = 0; j < TileSize.y; j++)
                {
                    routingBuffer[new Vector2Int(TilePos.x + i, TilePos.y + j)] = new Label(Direction.NONE, 0);
                }
            }
            while (tempMapQueue.Count != 0)
            {
                tempQueue = tempMapQueue.Dequeue();
                tempDic = routingBuffer[tempQueue];

                if (tempDic.dir != Direction.DOWN && TryEnqueueAndCheckTarget(Direction.UP))
                {
                    return true;
                }
                if (tempDic.dir != Direction.UP && TryEnqueueAndCheckTarget(Direction.DOWN))
                {
                    return true;
                }
                if (tempDic.dir != Direction.RIGHT && TryEnqueueAndCheckTarget(Direction.LEFT))
                {
                    return true;
                }
                if (tempDic.dir != Direction.LEFT && TryEnqueueAndCheckTarget(Direction.RIGHT))
                {
                    return true;
                }
            }
            return false;
        }

        public void GetCheckPoss(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> checkPoss)
        {
            GetCheckPoss(tilePos, ToDirection(dir), checkPoss);
        }
        public void GetCheckPoss(Vector2Int tilePos, Direction dir, List<Vector2Int> checkPoss)
        {
            checkPoss.Clear();
            int size;
            Vector2Int checkDir;

            switch (dir)
            {
                case Direction.UP:
                    tilePos.y += TileSize.y;
                    checkDir = Vector2Int.right;
                    size = TileSize.x;
                    break;
                case Direction.DOWN:
                    tilePos.y -= 1;
                    checkDir = Vector2Int.right;
                    size = TileSize.x;
                    break;
                case Direction.LEFT:
                    tilePos.x -= 1;
                    checkDir = Vector2Int.up;
                    size = TileSize.y;
                    break;
                case Direction.RIGHT:
                    tilePos.x += TileSize.y;
                    checkDir = Vector2Int.up;
                    size = TileSize.y;
                    break;
                default:
                    checkDir = Vector2Int.zero;
                    size = 0;
                    break;
            }
            for (int i = 0; i < size; i++)
            {
                checkPoss.Add(tilePos + checkDir * i);
            }
        }

        /// <summary>
        /// 通行可能で既に通ってない場合キューに入れる
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>そこがターゲットかどうか</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            GetCheckPoss(tempQueue, dir, buffer);
            bool includeTarget = false;

            foreach (var nextPos in buffer)
            {
                // 既に通った
                if (routingBuffer.ContainsKey(nextPos))
                {
                    return false;
                }

                var isTarget = nextPos == Target;
                if (isTarget)
                    includeTarget = true;

                if (!isTarget && (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove))
                {
                    return false;
                }
            }
            tempMapQueue.Enqueue(tempQueue + dir.GetVector2Int());

            var nextBuffet = new Label(dir, tempDic.count + 1);
            foreach (var nextPos in buffer)
            {
                routingBuffer[nextPos] = nextBuffet;
            }

            if (includeTarget)
            {
                tempTarget = tempQueue + dir.GetVector2Int();
            }

            return includeTarget;
        }
    }
}
