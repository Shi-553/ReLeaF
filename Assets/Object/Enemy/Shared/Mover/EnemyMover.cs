using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public partial class EnemyMover : MonoBehaviour
    {
        Rigidbody2DMover mover;


        [field: SerializeField, ReadOnly]
        public Vector2Int TilePos { get; private set; }
        [field: SerializeField, ReadOnly]
        public Vector2Int OldTilePos { get; private set; }
        public bool WasChangedTilePosPrevMove => TilePos != OldTilePos;


        /// <summary>
        /// 移動に使うターゲット位置、左下基準
        /// </summary>
        [field: SerializeField, ReadOnly]
        public Vector2Int MoveTarget { get; private set; }

        public bool IsMove => mover.IsMove;
        public bool IsLeftNow => Dir.x < 0;
        public bool IsLeftIfMove { get; private set; }
        public Vector2 WorldCenter => DungeonManager.Singleton.TilePosToWorld((Vector2)TilePos + ((Vector2)TileSize - Vector2.one) / 2);

        public Vector2Int Dir { get; set; }

        void UpdateDir(bool isNext = false)
        {
            if (isNext)
            {
                routes.TryPop(out var _);
            }

            if (routes.TryPeek(out var peekResult))
            {
                if (MathExtension.DuringExists(peekResult, TilePos, TilePos + TileSize))
                {
                    Dir = (peekResult - TilePos).ClampOneMagnitude();
                    MoveTarget = peekResult;
                    return;
                }
                var nearest = GetNearest(peekResult);
                var nearToResult = peekResult - nearest;
                Dir = nearToResult.ClampOneMagnitude();

                MoveTarget = TilePos + nearToResult;

            }
        }

        [SerializeField]
        EnemyMoverInfo enemyMoverInfo;
        public Vector2Int TileSize => enemyMoverInfo.TileSize;


        void Start()
        {
            TryGetComponent(out mover);
            TilePos = DungeonManager.Singleton.WorldToTilePos(mover.Position);
        }

        public enum MoveResult
        {
            Moveing,
            Finish,
            Error
        }
        public MoveResult Move()
        {
            return Move(enemyMoverInfo.Speed);
        }

        public MoveResult Move(float speedOverride)
        {
            OldTilePos = TilePos;
            GetCheckPoss(TilePos, Dir, buffer);

            if (Dir.x != 0)
                IsLeftIfMove = Dir.x < 0;

            var nextTilePos = TilePos + Dir;

            if (routes.Count == 1 && MoveTarget == target && MathExtension.DuringExists(MoveTarget, TilePos, TilePos + TileSize))
            {
                return MoveResult.Finish;
            }
            foreach (var nextPos in buffer)
            {
                if (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove)
                {
                    return MoveResult.Error;
                }
            }


            Vector2 worldNextTargetPos = DungeonManager.Singleton.TilePosToWorld(nextTilePos);

            var distance = worldNextTargetPos - mover.Position;
            var worldDir = distance.normalized;


            if (Vector2.Dot(worldDir, (Vector2)Dir) < 0 || (distance.sqrMagnitude < 0.001f))
            {
                TilePos = nextTilePos;
                if (MoveTarget == TilePos)
                {
                    UpdateDir(true);
                }
                var isFinish = routes.Count == 0;
                if (isFinish)
                    mover.Position = worldNextTargetPos;


                return isFinish ? MoveResult.Finish : MoveResult.Moveing;
            }

            mover.MoveDelta(DungeonManager.CELL_SIZE * speedOverride * worldDir);

            return MoveResult.Moveing;
        }

        Vector2Int target = Vector2Int.zero;

        // 自動操作(自由な位置を指定して経路探索)
        public bool UpdateTargetAutoRouting(Vector2Int targetTilePos)
        {
            target = targetTilePos;
            var ret = UpdateDirRouting();
            return ret;
        }

        // マニュアル操作(直線位置を指定)
        public void UpdateTargetStraight(Vector2Int targetTilePos)
        {
            target = targetTilePos;
            routes.Clear();
            routes.Push(targetTilePos);
            UpdateDir();
        }


        /// <summary>
        /// min <= target < max ならtargetを返す
        /// </summary>
        int GetNearest(int target, int min, int max)
        {
            if (min <= target && target < max)
                return target;
            else if (target < min)
                return min;
            else
                return max;
        }
        public Vector2Int GetNearest(Vector2Int target)
        {
            return new Vector2Int(GetNearest(target.x, TilePos.x, TilePos.x + TileSize.x - 1),
                GetNearest(target.y, TilePos.y, TilePos.y + TileSize.y - 1));
        }

        // そのマスに到達したとき、来た方向を記録
        Dictionary<Vector2Int, Direction> routingMapBuffer = new();

        Queue<Vector2Int> tempMapQueue = new();

        // スタートしたマスからゴールの手前のマスまで
        Stack<Vector2Int> routes = new();
        public Stack<Vector2Int> Routing => routes;

        // 最後にどの向きでターゲットに向かうか
        public Vector2Int ToTargetDir { get; private set; }

        List<Vector2Int> buffer = new();
        public IReadOnlyList<Vector2Int> Targets => buffer;

        Vector2Int tempTarget;
        Vector2Int tempQueue;

        bool UpdateDirRouting()
        {
            // ターゲットが同じタイル
            if (MathExtension.DuringExists(target, TilePos, TilePos + TileSize))
            {
                UpdateTargetStraight(target);
                return true;
            }

            routingMapBuffer.Clear();
            tempMapQueue.Clear();

            if (!FindShortestPath())
            {
                // 到達不可能
                return false;
            }
            routes.Clear();


            // ターゲットから戻って経路を確認する

            var currnet = tempTarget;
            // 四隅を入れる
            Direction[] cornerDirs = new Direction[4];

            // 最初はターゲットの位置をみる
            Direction currentDir = routingMapBuffer[target];

            ToTargetDir = currentDir.GetVector2Int();

            bool isSizeOne = TileSize == Vector2Int.one;

            while (true)
            {
                if (currentDir == Direction.NONE)
                {
                    UpdateDir();
                    return true;
                }

                var dir = currentDir.GetVector2Int();

                // 一つ戻る
                currnet -= dir;

                routes.Push(currnet);


                if (isSizeOne)
                {
                    currentDir = routingMapBuffer[currnet];
                    continue;
                }


                // 例えば、一つ前が左(Direction.UP)だった場合、戻るために上に移動する。
                // すると、右下＆左下の組み合わせはありえないので右下＆右上、右上＆左上、左上＆左下の三組を調べて次の方向を出す

                // 反時計回りに格納 右下->右上->左上->左下
                cornerDirs[0] = routingMapBuffer[new Vector2Int(currnet.x + TileSize.x - 1, currnet.y)];
                cornerDirs[1] = routingMapBuffer[new Vector2Int(currnet.x + TileSize.x - 1, currnet.y + TileSize.y - 1)];
                cornerDirs[2] = routingMapBuffer[new Vector2Int(currnet.x, currnet.y + TileSize.y - 1)];
                cornerDirs[3] = routingMapBuffer[currnet];

                // 0~2の3回
                for (int i = 0; i < 3; i++)
                {
                    Direction corner1 = cornerDirs[((int)currentDir + i) % cornerDirs.Length];
                    Direction corner2 = cornerDirs[((int)currentDir + i + 1) % cornerDirs.Length];

                    if (corner1 == corner2)
                    {
                        currentDir = corner1;
                        break;
                    }
                }
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
                    routingMapBuffer[new Vector2Int(TilePos.x + i, TilePos.y + j)] = Direction.NONE;
                }
            }
            while (tempMapQueue.Count != 0)
            {
                tempQueue = tempMapQueue.Dequeue();
                var beforeDir = routingMapBuffer[tempQueue];

                if (beforeDir != Direction.DOWN && TryEnqueueAndCheckTarget(Direction.UP))
                {
                    return true;
                }
                if (beforeDir != Direction.UP && TryEnqueueAndCheckTarget(Direction.DOWN))
                {
                    return true;
                }
                if (beforeDir != Direction.RIGHT && TryEnqueueAndCheckTarget(Direction.LEFT))
                {
                    return true;
                }
                if (beforeDir != Direction.LEFT && TryEnqueueAndCheckTarget(Direction.RIGHT))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 左下基準のtileposからdirに移動したいとき、障害物などを判定しないといけない位置を返す
        /// </summary>
        public void GetCheckPoss(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> checkPoss)
        {
            GetCheckPoss(tilePos, dir.ToDirection(), checkPoss);
        }
        /// <summary>
        /// 左下基準のtileposからdirに移動したいとき、障害物などを判定しないといけない位置を返す
        /// </summary>
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
                    tilePos.x += TileSize.x;
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
                if (routingMapBuffer.ContainsKey(nextPos))
                {
                    return false;
                }

                var isTarget = nextPos == target;
                if (isTarget)
                    includeTarget = true;

                if (!isTarget && (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove))
                {
                    return false;
                }
            }
            tempMapQueue.Enqueue(tempQueue + dir.GetVector2Int());

            foreach (var nextPos in buffer)
            {
                routingMapBuffer[nextPos] = dir;
            }

            if (includeTarget)
            {
                tempTarget = tempQueue + dir.GetVector2Int();
            }

            return includeTarget;
        }
    }
}
