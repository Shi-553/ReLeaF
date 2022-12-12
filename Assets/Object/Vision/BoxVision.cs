using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("矩形で当たり判定をチェックする")]
    public class BoxVision : Vision
    {
        [SerializeField, Rename("検知範囲(n/マス)")]
        Vector2 size = Vector2.one;
        [SerializeField, Rename("角度(度)")]
        float angle = 0;

        protected override int GetInitCapacity() => (int)(size.x * size.y);

        protected override Collider2D[] GetOverLapAll() =>
            Physics2D.OverlapBoxAll(transform.position, size / 2, angle, mask);
    }
}