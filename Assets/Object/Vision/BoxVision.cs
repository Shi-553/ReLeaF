using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("矩形で当たり判定をチェックする")]
    public class BoxVision : Vision
    {
        [SerializeField, Rename("検知範囲(n/マス)")]
        Vector2 size = Vector2.one;
        Vector2 Size => size * DungeonManager.CELL_SIZE;
        [SerializeField, Rename("角度(度)")]
        float angle = 0;

        protected override int GetInitCapacity() => (int)(Size.x * Size.y);

        protected override Collider2D[] GetOverLapAll() =>
            Physics2D.OverlapBoxAll(transform.position, Size, angle, mask);

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, angle), Size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
    }
}