using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("円形で当たり判定をチェックする")]
    public class CircleVision : Vision
    {
        [SerializeField, Rename("検知範囲(n/マス半径)")]
        float radius = 15.0f;
        float Radius => radius * DungeonManager.CELL_SIZE;

        protected override int GetInitCapacity() => (int)(Radius * Radius * 3.14f);

        protected override Collider2D[] GetOverLapAll() =>
            Physics2D.OverlapCircleAll(transform.position, Radius, mask);

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
#endif
    }
}