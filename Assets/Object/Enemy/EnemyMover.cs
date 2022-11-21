using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


        void Start()
        {
            TryGetComponent(out mover);
            TilePos = DungeonManager.Instance.WorldToTilePos(mover.Position);
            Dir = Vector2Int.down;
        }


        public bool Move(float speed, bool isStopInNear)
        {
            var nextTilePos = TilePos + Dir;

            Vector2 worldNextTargetPos = DungeonManager.Instance.TilePosToWorld(nextTilePos);

            var distance = worldNextTargetPos - mover.Position;
            var worldDir = distance.normalized;

            var isFinish = (isStopInNear && (Target - nextTilePos).sqrMagnitude <= 1) || Target == nextTilePos;


            if (isStopInNear && (Target - TilePos).sqrMagnitude <= 1)
            {
                Dir = Target - TilePos;
                return true;
            }

            if (Vector2.Dot(worldDir, (Vector2)Dir) < 0|| ( distance.sqrMagnitude<0.001f))
            {
                if (isFinish)
                {
                    mover.Position = worldNextTargetPos;
                }

                OldTilePos = TilePos;
                TilePos = nextTilePos;

                return Target == nextTilePos;
            }

            mover.Move(worldDir * speed);

            return false;
        }

        public void UpdateDir(Vector2Int targetTilePos, bool isRouting)
        {
            OldTarget = Target;
            Target = targetTilePos;
            if (isRouting)
                UpdateDirRouting();
            else
                UpdateDirStraight();
        }


        // ここからまっすぐ
        void UpdateDirStraight()
        {
            if (Target == TilePos)
            {
                Dir = Vector2Int.up;
                return;
            }
            var dir = Target - TilePos;

            dir.x = Mathf.Clamp(dir.x, -1, 1);
            dir.y = Mathf.Clamp(dir.y, -1, 1);
            if (dir.x != 0)
            {
                dir.y = 0;
            }
            Dir = dir;
        }


        // ここから経路探索

        public enum Direction
        {
            NONE, UP, DOWN, LEFT, RIGHT
        }

        // そのマスに到達したとき、来た方向を記録
        Dictionary<Vector2Int, Direction> routingBuffer = new Dictionary<Vector2Int, Direction>();

        Queue<Vector2Int> mapQueue = new Queue<Vector2Int>();

        Vector2Int temp;

        void UpdateDirRouting()
        {
            // ターゲットが同じタイル とりあえず↑
            if (Target == TilePos)
            {
                Dir = Vector2Int.up;
                return;
            }

            // 移動中でターゲットも変更なし とりあえず変更なし
            if (!WasChangedTilePosPrevFrame && !WasChangedTargetThisFrame)
            {
                return;
            }


            routingBuffer.Clear();
            mapQueue.Clear();

            if (!FindShortestPath())
            {
                // 到達不可能 とりあえず直線移動させる
                UpdateDirStraight();
                return;
            }

            // ターゲットから戻って経路を確認する
            var currnet = Target;

            while (true)
            {
                var dir = routingBuffer[currnet].ToVector2Int();

                // 一つ戻る
                currnet -= dir;

                if (currnet == TilePos)
                {
                    Dir = dir;
                    return;
                }
            }
        }


        /// <summary>
        /// 最短経路を探す
        /// </summary>
        /// <returns>到達可能か</returns>
        bool FindShortestPath()
        {
            mapQueue.Enqueue(TilePos);

            routingBuffer[TilePos] = Direction.NONE;

            while (mapQueue.Count != 0)
            {
                temp = mapQueue.Dequeue();

                if (TryEnqueueAndCheckTarget(Direction.UP))
                {
                    return true;
                }
                if (TryEnqueueAndCheckTarget(Direction.DOWN))
                {
                    return true;
                }
                if (TryEnqueueAndCheckTarget(Direction.LEFT))
                {
                    return true;
                }
                if (TryEnqueueAndCheckTarget(Direction.RIGHT))
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 通行可能で既に通ってない場合キューに入れる
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>そこがターゲットかどうか</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            var nextPos = temp + dir.ToVector2Int();

            // 既に通った
            if (routingBuffer.ContainsKey(nextPos))
            {
                return false;
            }

            var tile = DungeonManager.Instance.GetGroundTile(nextPos);

            // 通れるか
            if ((tile != null && tile.tileType == TileType.Sand) || nextPos == Target)
            {
                mapQueue.Enqueue(nextPos);

                routingBuffer[nextPos] = dir;
            }

            return nextPos == Target;
        }
    }
}
