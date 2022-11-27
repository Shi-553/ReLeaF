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


        void Start()
        {
            TryGetComponent(out mover);
            TilePos = DungeonManager.Singleton.WorldToTilePos(mover.Position);
            Dir = Vector2Int.down;
        }


        public bool Move(float speed, bool isStopInNear)
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

            mover.MoveDelta(DungeonManager.CELL_SIZE * worldDir * speed);

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

        // ���̃}�X�ɓ��B�����Ƃ��A�����������L�^
        Dictionary<Vector2Int, Direction> routingBuffer = new Dictionary<Vector2Int, Direction>();

        Queue<Vector2Int> tempMapQueue = new Queue<Vector2Int>();

        // �S�[���̎�O�̃}�X����X�^�[�g�����}�X�܂�
        List<Vector2Int> routing = new List<Vector2Int>();
        public IReadOnlyList<Vector2Int> Routing => routing;

        Vector2Int temp;

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
                return;
            }
            routing.Clear();

            // �^�[�Q�b�g����߂��Čo�H���m�F����
            var currnet = Target;

            while (true)
            {
                var dir = routingBuffer[currnet].ToVector2Int();

                // ��߂�
                currnet -= dir;

                routing.Add(currnet);
                if (currnet == TilePos)
                {
                    Dir = dir;
                    return;
                }
            }
        }


        /// <summary>
        /// �ŒZ�o�H��T��
        /// </summary>
        /// <returns>���B�\��</returns>
        bool FindShortestPath()
        {
            tempMapQueue.Enqueue(TilePos);

            routingBuffer[TilePos] = Direction.NONE;

            while (tempMapQueue.Count != 0)
            {
                temp = tempMapQueue.Dequeue();

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
        /// �ʍs�\�Ŋ��ɒʂ��ĂȂ��ꍇ�L���[�ɓ����
        /// </summary>
        /// <param name="nextPos"></param>
        /// <returns>�������^�[�Q�b�g���ǂ���</returns>
        bool TryEnqueueAndCheckTarget(Direction dir)
        {
            var nextPos = temp + dir.ToVector2Int();

            // ���ɒʂ���
            if (routingBuffer.ContainsKey(nextPos))
            {
                return false;
            }

            if ((DungeonManager.Singleton.TryGetTile(nextPos, out var tile) && tile.CanEnemyMove) || nextPos == Target)
            {
                tempMapQueue.Enqueue(nextPos);

                routingBuffer[nextPos] = dir;
            }

            return nextPos == Target;
        }
    }
}
