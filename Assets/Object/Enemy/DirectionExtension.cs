using UnityEngine;
using static ReLeaf.EnemyMover;

namespace ReLeaf
{
    public static class DirectionExtension
    {
        public static Vector2Int ToVector2Int(this Direction dir)
        {
            return dir switch
            {
                Direction.UP => Vector2Int.up,
                Direction.DOWN => Vector2Int.down,
                Direction.LEFT => Vector2Int.left,
                Direction.RIGHT => Vector2Int.right,
                _ => Vector2Int.zero,
            };
        }
        public static Direction Inverse(this Direction dir)
        {
            return dir switch
            {
                Direction.UP => Direction.DOWN,
                Direction.DOWN => Direction.UP,
                Direction.LEFT => Direction.RIGHT,
                Direction.RIGHT => Direction.LEFT,
                _ => dir,
            };
        }
    }
}
