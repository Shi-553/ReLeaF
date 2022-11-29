using System.Collections.Generic;
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

        public bool Move(bool isStopInNear)
        {
            return Move(enemyMoverInfo.Speed, isStopInNear);
        }

        public bool Move(float speedOverride, bool isStopInNear)
        {

            (var nextBasePos, Vector2Int djacentDir, int size) = GetCheckPoss(TilePos, ToDirection(Dir));

            var nextTilePos = TilePos + Dir;

            for (int i = 0; i < size; i++)
            {
                var nextPos = nextBasePos + djacentDir * i;
                if (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove)
                    return true;

                if (isStopInNear && Target == nextPos)
                {
                    Dir = Target - TilePos;
                    return true;
                }
                if (!isStopInNear && Target == nextPos - Dir)
                    return true;
            }


            Vector2 worldNextTargetPos = DungeonManager.Singleton.TilePosToWorld(nextTilePos);

            var distance = worldNextTargetPos - mover.Position;
            var worldDir = distance.normalized;



            if (isStopInNear && (Target - TilePos).sqrMagnitude <= 1)
            {
                Dir = Target - TilePos;
                return true;
            }

            if (Vector2.Dot(worldDir, (Vector2)Dir) < 0 || (distance.sqrMagnitude < 0.001f))
            {
                var isFinish = (isStopInNear && (Target - nextTilePos).sqrMagnitude <= 1) || Target == nextTilePos;
                if (isFinish)
                {
                    mover.Position = worldNextTargetPos;
                }

                OldTilePos = TilePos;
                TilePos = nextTilePos;

                return Target == nextTilePos;
            }

            mover.MoveDelta(DungeonManager.CELL_SIZE * speedOverride * worldDir);

            return false;
        }

        public bool UpdateDir(Vector2Int targetTilePos)
        {
            OldTarget = Target;
            Target = targetTilePos;
            return UpdateDirRouting();
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

        [SerializeField]
        TestTest prefab;

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
        (Vector2Int pos, Vector2Int adacentDir, int size) GetCheckPoss(Vector2Int tilePos, Direction dir)
        {
            switch (dir)
            {
                case Direction.UP:
                    tilePos.y += TileSize.y;
                    return (tilePos, Vector2Int.right, TileSize.x);
                case Direction.DOWN:
                    tilePos.y -= 1;
                    return (tilePos, Vector2Int.right, TileSize.x);
                case Direction.LEFT:
                    tilePos.x -= 1;
                    return (tilePos, Vector2Int.up, TileSize.y);
                case Direction.RIGHT:
                    tilePos.x += TileSize.y;
                    return (tilePos, Vector2Int.up, TileSize.y);
                default:
                    return (tilePos, Vector2Int.zero, 0);
            }
        }
        /// <summary>
        /// 通行可能で既に通ってない場合キューに入れる
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>そこがターゲットかどうか</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            (var nextBasePos, Vector2Int djacentDir, int size) = GetCheckPoss(tempQueue, dir);
            bool includeTarget = false;

            for (int i = 0; i < size; i++)
            {
                var nextPos = nextBasePos + djacentDir * i;
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
            for (int i = 0; i < size; i++)
            {
                var nextPos = nextBasePos + djacentDir * i;
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
