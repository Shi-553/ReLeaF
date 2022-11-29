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
            var nextTilePos = TilePos + Dir;

            Vector2 worldNextTargetPos = DungeonManager.Singleton.TilePosToWorld(nextTilePos);

            var distance = worldNextTargetPos - mover.Position;
            var worldDir = distance.normalized;

            var isFinish = (isStopInNear && (Target - nextTilePos).sqrMagnitude <= 1) || Target == nextTilePos;


            if (isStopInNear && (Target - TilePos).sqrMagnitude <= 1)
            {
                Dir = Target - TilePos;
                return true;
            }

            if (Vector2.Dot(worldDir, (Vector2)Dir) < 0 || (distance.sqrMagnitude < 0.001f))
            {
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

        public void UpdateDir(Vector2Int targetTilePos, bool isRouting)
        {
            OldTarget = Target;
            Target = targetTilePos;
            if (isRouting)
                UpdateDirRouting();
            else
                UpdateDirStraight();
        }


        // ��������܂�����
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


        // ��������o�H�T��

        public enum Direction
        {
            NONE, UP, DOWN, LEFT, RIGHT
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
        Dictionary<Vector2Int, Label> routingBuffer = new();

        Queue<Vector2Int> tempMapQueue = new Queue<Vector2Int>();

        // �S�[���̎�O�̃}�X����X�^�[�g�����}�X�܂�
        List<Vector2Int> routing = new List<Vector2Int>();
        public IReadOnlyList<Vector2Int> Routing => routing;

        Vector2Int tempTarget;
        Vector2Int tempQueue;
        Label tempDic;

        void UpdateDirRouting()
        {
            // �^�[�Q�b�g�������^�C�� �Ƃ肠������
            if (Target == TilePos)
            {
                Dir = Vector2Int.up;
                return;
            }

            // �ړ����Ń^�[�Q�b�g���ύX�Ȃ� �Ƃ肠�����ύX�Ȃ�
            if (!WasChangedTilePosPrevFrame && !WasChangedTargetThisFrame)
            {
                return;
            }


            routingBuffer.Clear();
            tempMapQueue.Clear();

            if (!FindShortestPath())
            {
                // ���B�s�\ �Ƃ肠���������ړ�������
                UpdateDirStraight();
                "���B�s�\".DebugLog();
                return;
            }
            routing.Clear();


            // �^�[�Q�b�g����߂��Čo�H���m�F����
            var currnet = tempTarget;
            Direction prevDir = Direction.NONE;
            while (true)
            {
                var max = new Label(Direction.NONE, 0);
                for (int i = 0; i < enemyMoverInfo.TileSize.x; i++)
                {
                    for (int j = 0; j < enemyMoverInfo.TileSize.y; j++)
                    {
                        var c = routingBuffer[new Vector2Int(currnet.x + i, currnet.y + j)];
                        if (c.count > max.count)
                            max = c;
                    }
                }
                if (max.dir == Direction.NONE)
                {
                    Dir = prevDir.GetVector2Int();
                    return;
                }
                var dir = max.dir.GetVector2Int();

                // ��߂�
                currnet -= dir;

                routing.Add(currnet);

                prevDir = max.dir;
            }
        }

        [SerializeField]
        TestTest prefab;

        /// <summary>
        /// �ŒZ�o�H��T��
        /// </summary>
        /// <returns>���B�\��</returns>
        bool FindShortestPath()
        {
            tempMapQueue.Enqueue(TilePos);

            for (int i = 0; i < enemyMoverInfo.TileSize.x; i++)
            {
                for (int j = 0; j < enemyMoverInfo.TileSize.y; j++)
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

        /// <summary>
        /// �ʍs�\�Ŋ��ɒʂ��ĂȂ��ꍇ�L���[�ɓ����
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>�������^�[�Q�b�g���ǂ���</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            var nextBasePos = tempQueue;
            Vector2Int djacentDir;
            int size;
            switch (dir)
            {
                case Direction.UP:
                    nextBasePos.y += enemyMoverInfo.TileSize.y;
                    size = enemyMoverInfo.TileSize.x;
                    djacentDir = Vector2Int.right;
                    break;
                case Direction.DOWN:
                    nextBasePos.y -= 1;
                    size = enemyMoverInfo.TileSize.x;
                    djacentDir = Vector2Int.right;
                    break;
                case Direction.LEFT:
                    nextBasePos.x -= 1;
                    size = enemyMoverInfo.TileSize.y;
                    djacentDir = Vector2Int.up;
                    break;
                case Direction.RIGHT:
                    nextBasePos.x += enemyMoverInfo.TileSize.y;
                    size = enemyMoverInfo.TileSize.y;
                    djacentDir = Vector2Int.up;
                    break;
                default:
                    size = 0;
                    djacentDir = Vector2Int.zero;
                    break;
            }
            bool includeTarget = false;

            for (int i = 0; i < size; i++)
            {
                var nextPos = nextBasePos + djacentDir * i;
                // ���ɒʂ���
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
                tempTarget = nextBasePos;
            }

            return includeTarget;
        }
    }
}
