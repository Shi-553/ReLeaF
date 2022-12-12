using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("円形で当たり判定をチェックする")]
    public class CircleVision : Vision
    {
        [SerializeField, Rename("検知範囲(n/マス)")]
        float radius = 15.0f;

        protected override Collider2D[] GetOverLapAll() => Physics2D.OverlapCircleAll(transform.position, radius / 2);
    }
}