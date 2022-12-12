using UnityEngine;

namespace ReLeaf
{
    public class BoxVision : Vision
    {
        [SerializeField]
        Vector2 size = Vector2.one;
        [SerializeField]
        float angle = 0;

        protected override Collider2D[] GetOverLapAll() => Physics2D.OverlapBoxAll(transform.position, size / 2, angle);
    }
}