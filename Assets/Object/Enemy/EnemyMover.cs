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
        public Vector2Int Dir { get; private set; }

        void Start()
        {
            TryGetComponent(out mover);
            TilePos = (Vector2Int)DungeonManager.Instance.WorldToTilePos(mover.Position);
            Dir = Vector2Int.down;
        }
        public Vector2Int GetDir(Vector2Int targetTilePos)
        {
            var dir = targetTilePos - TilePos;

            dir.x = Mathf.Clamp(dir.x, -1, 1);
            dir.y = Mathf.Clamp(dir.y, -1, 1);
            if (dir.x != 0)
            {
                dir.y = 0;
            }
            return dir;
        }
        public bool MoveTo(Vector2Int targetTilePos, float speed, bool isStopInNear)
        {

            Dir = GetDir(targetTilePos);
            var nextTilePos = TilePos + Dir;

            var adjustNextTargetPos = DungeonManager.Instance.TilePosToWorld(nextTilePos);

            if (isStopInNear && (targetTilePos - TilePos).sqrMagnitude <= 1)
            {
                Dir = targetTilePos - TilePos;
                return true;
            }
            if ((adjustNextTargetPos - mover.Position).sqrMagnitude < 0.001f)
            {
                TilePos = nextTilePos;

                return targetTilePos == nextTilePos;
            }
            mover.MoveTowards(adjustNextTargetPos, speed);

            return false;
        }

        // 下向きをデフォルトとするローカル座標を向きに応じて回転
        public Vector3 GetRotatedLocalPos(Vector3 defaultLocal)
        {
            return Quaternion.Euler(Dir.x, Dir.y + 1, 0) * defaultLocal;
        }
        public Vector2Int GetRotatedLocalPos(Vector2Int defaultLocal)
        {
            if (Dir == Vector2Int.down)
            {
                return defaultLocal;
            }
            if (Dir == Vector2Int.up)
            {
                return -defaultLocal;
            }
            if (Dir == Vector2Int.left)
            {
                return new Vector2Int(defaultLocal.y, defaultLocal.x);
            }
            if (Dir == Vector2Int.right)
            {
                return -new Vector2Int(defaultLocal.y, defaultLocal.x);
            }

            return defaultLocal;
        }
    }
}
