using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("円形で当たり判定をチェックする")]
    public class CircleVision : Vision
    {
        [SerializeField, Rename("検知範囲(n/マス)")]
        float radius = 15.0f;

        protected override int GetInitCapacity() => (int)(radius / 2 * radius / 2 * 3.14f);

        protected override Collider2D[] GetOverLapAll() =>
            Physics2D.OverlapCircleAll(transform.position, radius / 2, mask);
    }
}