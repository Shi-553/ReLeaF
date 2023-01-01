using System;
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
        /// �ړ��Ɏg���^�[�Q�b�g�ʒu�A�����
        /// </summary>
        [field: SerializeField, ReadOnly]
        public Vector2Int MoveTarget { get; private set; }

        public event Action OnMove;

        public bool IsMove => mover.IsMove;
        public bool IsLeftNow => Dir.x < 0;
        public bool IsLeftIfMove { get; private set; }
        public Vector2 WorldCenter => DungeonManager.Singleton.TilePosToWorld((Vector2)TilePos + ((Vector2)TileSize - Vector2.one) / 2);

        Vector2Int dir;
        Vector2Int Dir
        {
            get => dir;
            set
            {
                dir = value;
                dirNotZero = dir == Vector2Int.zero ? Vector2Int.down : dir;
            }
        }
        Vector2Int dirNotZero;
        public Vector2Int DirNotZero
        {
            get => dirNotZero;
            set => Dir = value;
        }

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
            Error,
        }
        public MoveResult Move()
        {
            return Move(enemyMoverInfo.Speed);
        }

        public MoveResult Move(float speedOverride, bool isAttackMove = false)
        {
            OnMove?.Invoke();

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
                if (routes.Count == 1 && nextPos == target)
                    return MoveResult.Finish;
            }

            foreach (var nextPos in buffer)
            {
                if (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove(isAttackMove) || !tile.InstancedParent.CanEnemyMove(isAttackMove))
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

        // ��������(���R�Ȉʒu���w�肵�Čo�H�T��)
        public bool UpdateTargetAutoRouting(Vector2Int targetTilePos)
        {
            target = targetTilePos;
            var ret = UpdateDirRouting();
            return ret;
        }

        // �}�j���A������(�����ʒu���w��)
        public void UpdateTargetStraight(Vector2Int targetTilePos)
        {
            target = targetTilePos;
            routes.Clear();
            routes.Push(targetTilePos);
            UpdateDir();
        }


        /// <summary>
        /// min <= target < max �Ȃ�target��Ԃ�
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

        // ���̃}�X�ɓ��B�����Ƃ��A�����������L�^
        Dictionary<Vector2Int, Direction> routingMapBuffer = new();

        Queue<Vector2Int> tempMapQueue = new();

        // �X�^�[�g�����}�X����S�[���̎�O�̃}�X�܂�
        Stack<Vector2Int> routes = new();
        public Stack<Vector2Int> Routing => routes;

        // �Ō�ɂǂ̌����Ń^�[�Q�b�g�Ɍ�������
        public Vector2Int ToTargetDir { get; private set; }

        List<Vector2Int> buffer = new();
        public IReadOnlyList<Vector2Int> Targets => buffer;

        Vector2Int tempTarget;
        Vector2Int tempQueue;

        bool UpdateDirRouting()
        {
            // �^�[�Q�b�g�������^�C��
            if (MathExtension.DuringExists(target, TilePos, TilePos + TileSize))
            {
                UpdateTargetStraight(target);
                return true;
            }

            routingMapBuffer.Clear();
            tempMapQueue.Clear();

            if (!FindShortestPath())
            {
                // ���B�s�\
                return false;
            }
            routes.Clear();


            // �^�[�Q�b�g����߂��Čo�H���m�F����

            var currnet = tempTarget;

            // �ŏ��̓^�[�Q�b�g�̈ʒu���݂�
            Direction currentDir = routingMapBuffer[tempTarget];

            ToTargetDir = currentDir.GetVector2Int();

            while (true)
            {
                if (currentDir == Direction.NONE)
                {
                    UpdateDir();
                    return true;
                }

                var dir = currentDir.GetVector2Int();

                // ��߂�
                currnet -= dir;

                routes.Push(currnet);

                currentDir = routingMapBuffer[currnet];
            }
        }


        /// <summary>
        /// �ŒZ�o�H��T��
        /// </summary>
        /// <returns>���B�\��</returns>
        bool FindShortestPath()
        {
            tempMapQueue.Enqueue(TilePos);

            routingMapBuffer[TilePos] = Direction.NONE;

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
        /// �������tilepos����dir�Ɉړ��������Ƃ��A��Q���Ȃǂ𔻒肵�Ȃ��Ƃ����Ȃ��ʒu��Ԃ�
        /// </summary>
        public void GetCheckPoss(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> checkPoss)
        {
            GetCheckPoss(tilePos, dir.ToDirection(), checkPoss);
        }
        /// <summary>
        /// �������tilepos����dir�Ɉړ��������Ƃ��A��Q���Ȃǂ𔻒肵�Ȃ��Ƃ����Ȃ��ʒu��Ԃ�
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
        /// �ʍs�\�Ŋ��ɒʂ��ĂȂ��ꍇ�L���[�ɓ����
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>�������^�[�Q�b�g���ǂ���</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            GetCheckPoss(tempQueue, dir, buffer);
            bool includeTarget = false;

            var nextPivotPos = tempQueue + dir.GetVector2Int();

            // ���ɒʂ���
            if (routingMapBuffer.ContainsKey(nextPivotPos))
            {
                return false;
            }

            foreach (var nextPos in buffer)
            {
                var isTarget = nextPos == target;
                if (isTarget)
                    includeTarget = true;

                if (!includeTarget && (!DungeonManager.Singleton.TryGetTile<TileObject>(nextPos, out var tile) || !tile.CanEnemyMove(true) || (tile != tile.InstancedParent && !tile.InstancedParent.CanEnemyMove(true))))
                {
                    return false;
                }
            }

            tempMapQueue.Enqueue(nextPivotPos);

            routingMapBuffer[nextPivotPos] = dir;

            if (includeTarget)
            {
                tempTarget = tempQueue + dir.GetVector2Int();
            }

            return includeTarget;
        }
    }
}
