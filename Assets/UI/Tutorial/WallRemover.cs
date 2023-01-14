using UnityEngine;

namespace ReLeaf
{
    public class WallRemover : MonoBehaviour
    {
        [SerializeField]
        Vector2Int size = new(1, 1);
        public Vector2 Size => (Vector2)size / 2.0f;
        public void RemoveWall()
        {
            var tilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    DungeonManager.Singleton.ToSand(new(tilePos.x + x, tilePos.y + y));
                }
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var tilePos = DungeonManager.Singleton.Tilemap.WorldToCell(transform.position);
            var worldPos = DungeonManager.Singleton.Tilemap.CellToWorld(tilePos) + (Vector3)Size / 2;

            Gizmos.matrix = Matrix4x4.TRS(worldPos, Quaternion.identity, Size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
    }
}
