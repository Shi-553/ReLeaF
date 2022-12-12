using UnityEngine;

namespace ReLeaf
{
    public class CircleVision : Vision
    {
        [SerializeField]
        float radius = 15.0f;

        protected override Collider2D[] GetOverLapAll() => Physics2D.OverlapCircleAll(transform.position, radius / 2);
    }
}