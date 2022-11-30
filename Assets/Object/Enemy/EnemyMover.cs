using DebugLogExtension;
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
        public bool WasChangedTilePosPrevMove => TilePos != OldTilePos;


        [field: SerializeField, ReadOnly]
        public Vector2Int MoveTarget { get; private set; }


        public Vector2Int Dir { get; private set; }
        void UpdateDir(bool isNext = false)
        {
            if (isNext)
            {
                routing.TryPop(out var _);
            }

            if (routing.TryPeek(out var peekResult))
            {
                if (MathExtension.DuringExists(peekResult, TilePos, TilePos + TileSize))
                {
                    Dir = (peekResult - TilePos).ClampOneMagnitude();
                    MoveTarget = peekResult;
                    return;
                }
                var nearest = GetNearest(peekResult);
                Dir = (peekResult - nearest).ClampOneMagnitude();
                MoveTarget = peekResult + TilePos - nearest;
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

            var nextTilePos = TilePos + Dir;

            foreach (var nextPos in buffer)
            {
                if (!DungeonManager.Singleton.TryGetTile(nextPos, out var tile) || !tile.CanEnemyMove)
                    return MoveResult.Error;
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
                var isFinish = routing.Count == 0;
                if (isFinish)
                    mover.Position = worldNextTargetPos;


                return isFinish ? MoveResult.Finish : MoveResult.Moveing;
            }

            mover.MoveDelta(DungeonManager.CELL_SIZE * speedOverride * worldDir);

            return MoveResult.Moveing;
        }

        Vector2Int target = Vector2Int.zero;
        // ��������(���R�Ȉʒu���w��)
        public bool UpdateTarget(Vector2Int targetTilePos)
        {
            target = targetTilePos;
            var ret = UpdateDirRouting();
            return ret;
        }

        // �}�j���A������(������̒����ʒu���w��)
        public void UpdateMoveTargetAndDir(Vector2Int targetTilePos)
        {
            routing.Clear();
            routing.Push(targetTilePos);
            UpdateDir();
        }


        int GetNearest(int target, int min, int max)
        {
            if (min <= target && target <= max)
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

        // ��������o�H�T��

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

        // ���̃}�X�ɓ��B�����Ƃ��A�����������L�^
        Dictionary<Vector2Int, Label> routingMapBuffer = new();

        Queue<Vector2Int> tempMapQueue = new Queue<Vector2Int>();

        // �X�^�[�g�����}�X����S�[���̎�O�̃}�X�܂�
        Stack<Vector2Int> routing = new();
        public Stack<Vector2Int> Routing => routing;

        List<Vector2Int> buffer = new();
        public IReadOnlyList<Vector2Int> Targets => buffer;

        Vector2Int tempTarget;
        Vector2Int tempQueue;
        Label tempDic;

        bool UpdateDirRouting()
        {
            // �^�[�Q�b�g�������^�C�� �Ƃ肠������
            if (MathExtension.DuringExists(target, TilePos, TilePos + TileSize))
            {
                UpdateMoveTargetAndDir(target);
                return true;
            }



            routingMapBuffer.Clear();
            tempMapQueue.Clear();

            if (!FindShortestPath())
            {
                // ���B�s�\
                return false;
            }
            routing.Clear();


            // �^�[�Q�b�g����߂��Čo�H���m�F����
            var currnet = tempTarget;
            while (true)
            {
                var max = new Label(Direction.NONE, 0);
                for (int i = 0; i < TileSize.x; i++)
                {
                    for (int j = 0; j < TileSize.y; j++)
                    {
                        var c = routingMapBuffer[new Vector2Int(currnet.x + i, currnet.y + j)];
                        if (c.count > max.count)
                            max = c;
                    }
                }
                if (max.dir == Direction.NONE)
                {
                    routing.DebugLogCollection();
                    UpdateDir();

                    return true;
                }
                var dir = max.dir.GetVector2Int();

                // ��߂�
                currnet -= dir;

                routing.Push(currnet);
            }
        }


        /// <summary>
        /// �ŒZ�o�H��T��
        /// </summary>
        /// <returns>���B�\��</returns>
        bool FindShortestPath()
        {
            tempMapQueue.Enqueue(TilePos);

            for (int i = 0; i < TileSize.x; i++)
            {
                for (int j = 0; j < TileSize.y; j++)
                {
                    routingMapBuffer[new Vector2Int(TilePos.x + i, TilePos.y + j)] = new Label(Direction.NONE, 0);
                }
            }
            while (tempMapQueue.Count != 0)
            {
                tempQueue = tempMapQueue.Dequeue();
                tempDic = routingMapBuffer[tempQueue];

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
        /// �ʍs�\�Ŋ��ɒʂ��ĂȂ��ꍇ�L���[�ɓ����
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>�������^�[�Q�b�g���ǂ���</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            GetCheckPoss(tempQueue, dir, buffer);
            bool includeTarget = false;

            foreach (var nextPos in buffer)
            {
                // ���ɒʂ���
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

            var nextBuffet = new Label(dir, tempDic.count + 1);
            foreach (var nextPos in buffer)
            {
                routingMapBuffer[nextPos] = nextBuffet;
            }

            if (includeTarget)
            {
                tempTarget = tempQueue + dir.GetVector2Int();
            }

            return includeTarget;
        }
    }
}
