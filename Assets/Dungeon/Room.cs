using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class Room : MonoBehaviour
    {

        HashSet<Vector2Int> roomTilePoss = new();

        public IEnumerable<Vector2Int> RoomTilePoss => roomTilePoss;

        public bool ContainsRoom(Vector2Int pos) => roomTilePoss.Contains(pos);

        Vector2Int maxTile;
        Vector2Int minTile;

        public Transform EnemyRoot { get; private set; }

        public void Awake()
        {
            var tilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            maxTile = tilePos;
            minTile = tilePos;

            AddRoomTile(tilePos);

            var enemy = GetComponentInChildren<EnemyCore>();
            EnemyRoot = enemy != null ? enemy.transform.parent : transform;

        }
        private void Start()
        {
            if (ContainsRoom(PlayerMover.Singleton.TilePos))
            {
                PlayerMover.Singleton.LastRoom = this;
            }
        }

        void AddRoomTile(Vector2Int pos)
        {
            if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                return;

            if (tile.TileType == TileType.Wall)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            if (tile is ISetRoomTile roomTile)
                roomTile.SetRoom(this);

            if (maxTile.y < pos.y) maxTile.y = pos.y;
            if (maxTile.x < pos.x) maxTile.x = pos.x;

            if (minTile.y > pos.y) minTile.y = pos.y;
            if (minTile.x > pos.x) minTile.x = pos.x;


            if (tile.TileType == TileType.Entrance)
            {
                return;
            }

            AddRoomTile(pos + Vector2Int.up);
            AddRoomTile(pos + Vector2Int.down);
            AddRoomTile(pos + Vector2Int.left);
            AddRoomTile(pos + Vector2Int.right);
        }

        public void GreeningRoom()
        {
            if (GameRuleManager.Singleton.IsPlaying)
                StartCoroutine(RoomBlast());
        }


        IEnumerator RoomBlast()
        {
            using var _ = RobotGreening.Singleton.StartGreening();

            RobotMover.Singleton.GetComponentInChildren<SpriteRenderer>().sortingOrder++;

            yield return null;

            HashSet<SpawnLakeGroup> groups = new();
            foreach (var pos in roomTilePoss)
            {
                if (DungeonManager.Singleton.TryGetTile<SpawnLake>(pos, out var lake))
                {
                    groups.Add(lake.Group);
                }
            }
            groups.ForEach(g => g.CanSpawn = false);


            var wait = new WaitForSeconds(1);



            foreach (var group in groups)
            {
                Vector2 center = Vector2.zero;
                group.Dic.Values.ForEach(lake => center += lake.TilePos);
                center /= group.Dic.Count;

                yield return Move(DungeonManager.Singleton.TilePosToWorld(center));
                yield return Attack();
                group.Dic.Values.ForEach(lake => DungeonManager.Singleton.SowSeed(lake.TilePos, true));
            }


            var enemys = GetComponentsInChildren<EnemyCore>();

            foreach (var enemy in enemys)
            {
                yield return Move(enemy.transform.GetChild(0));
                yield return Attack();
                enemy.Damaged(999);
            }


            RobotMover.Singleton.IsStop = false;

            RobotMover.Singleton.GetComponentInChildren<SpriteRenderer>().sortingOrder--;
        }
        IEnumerator Attack()
        {
            RobotMover.Singleton.IsStop = true;
            RobotMover.Singleton.GetComponent<RobotAnimation>().Thrust();
            yield return new WaitForSeconds(0.5f);

        }
        IEnumerator Move(Vector3 target)
        {

            RobotMover.Singleton.IsStop = false;
            while (true)
            {
                RobotMover.Singleton.UpdateManualOperation(target, 30, true, 1);

                yield return null;
                if (!RobotMover.Singleton.UseManualOperation)
                    yield break;
            }
        }
        IEnumerator Move(Transform target)
        {

            RobotMover.Singleton.IsStop = false;
            while (true)
            {
                RobotMover.Singleton.UpdateManualOperation(target.position, 30, true, 1);

                yield return null;
                if (!RobotMover.Singleton.UseManualOperation)
                    yield break;
            }
        }


#if UNITY_EDITOR
        void AddRoomTileInEdior(Vector2Int pos)
        {
            var tile = DungeonManager.Singleton.Tilemap.GetTile<TerrainTile>((Vector3Int)pos);
            if (tile == null)
                return;

            if (tile.CurrentTileObject.TileType == TileType.Wall)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            if (tile.CurrentTileObject.TileType == TileType.Entrance)
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
