using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReLeaf
{
    public class Room : MonoBehaviour
    {

        HashSet<Vector2Int> roomTilePoss = new();

        public IEnumerable<Vector2Int> RoomTilePoss => roomTilePoss;

        public bool ContainsRoom(Vector2Int pos) => roomTilePoss.Contains(pos);

        Vector2Int maxTile;
        Vector2Int minTile;

        public void Awake()
        {
            var tilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            maxTile = tilePos;
            minTile = tilePos;

            AddRoomTile(tilePos);
        }

        void AddRoomTile(Vector2Int pos)
        {
            if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                return;

            if (tile.TileType == TileType.Wall || tile.TileType == TileType.Entrance)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            if (maxTile.y < pos.y) maxTile.y = pos.y;
            if (maxTile.x < pos.x) maxTile.x = pos.x;

            if (minTile.y > pos.y) minTile.y = pos.y;
            if (minTile.x > pos.x) minTile.x = pos.x;


            AddRoomTile(pos + Vector2Int.up);
            AddRoomTile(pos + Vector2Int.down);
            AddRoomTile(pos + Vector2Int.left);
            AddRoomTile(pos + Vector2Int.right);
        }

        public void GreeningRoom()
        {
            var enemys = GetComponentsInChildren<EnemyCore>();


            foreach (var enemy in enemys)
            {
                enemy.Damaged(999);
            }
        }

#if UNITY_EDITOR
        void AddRoomTileInEdior(Vector2Int pos)
        {
            var tile = DungeonManager.Singleton.Tilemap.GetTile<TerrainTile>((Vector3Int)pos);
            if (tile == null)
                return;

            if (tile.CurrentTileObject.TileType == TileType.Wall || tile.CurrentTileObject.TileType == TileType.Entrance)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            AddRoomTileInEdior(pos + Vector2Int.up);
            AddRoomTileInEdior(pos + Vector2Int.down);
            AddRoomTileInEdior(pos + Vector2Int.left);
            AddRoomTileInEdior(pos + Vector2Int.right);
        }
        void ResetGizmo()
        {
            roomTilePoss.Clear();
            AddRoomTileInEdior((Vector2Int)DungeonManager.Singleton.Tilemap.WorldToCell(transform.position));
        }
        private void OnDrawGizmosSelected()
        {
            foreach (var pos in roomTilePoss)
            {
                Gizmos.DrawSphere(DungeonManager.Singleton.TilePosToWorld(pos), 0.1f);
            }
        }
        [CustomEditor(typeof(Room))]
        public class RoomEditor : Editor
        {
            private void OnEnable()
            {
                var room = target as Room;

                room.ResetGizmo();
            }

            public override void OnInspectorGUI()
            {
                var room = target as Room;
                base.OnInspectorGUI();

                if (GUILayout.Button("Reset Gizmo"))
                {
                    room.ResetGizmo();
                }
            }
        }
#endif
    }
}
