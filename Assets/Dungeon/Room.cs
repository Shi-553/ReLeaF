using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public interface IRoomBlastTarget
    {
        public Vector3 Position { get; }

        public void BeginGreening();

        public bool CanGreening() => true;
        public void Greening();
    }
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

            roomTilePoss.Clear();
            AddRoomTile(tilePos);

            foreach (var setRoom in GetComponentsInChildren<ISetRoom>())
            {
                setRoom.SetRoom(this);
            }

        }
        private void Start()
        {
            if (ContainsRoom(PlayerMover.Singleton.TilePos))
            {
                PlayerMover.Singleton.Room = this;
            }
        }

        public void InitRoomTileEarly()
        {
            roomTilePoss.Clear();
            AddRoomTileEarly(DungeonManager.Singleton.WorldToTilePos(transform.position));
        }
        void AddRoomTileEarly(Vector2Int pos)
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

            AddRoomTileEarly(pos + Vector2Int.up);
            AddRoomTileEarly(pos + Vector2Int.down);
            AddRoomTileEarly(pos + Vector2Int.left);
            AddRoomTileEarly(pos + Vector2Int.right);
        }

        void AddRoomTile(Vector2Int pos)
        {
            if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                return;

            if (tile.TileType == TileType.Wall)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            if (tile is ISetRoom roomTile)
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

        Coroutine co;
        public bool IsRoomBlastNow => co != null;
        public void GreeningRoom()
        {
            if (GameRuleManager.Singleton.IsPlaying)
                co = StartCoroutine(RoomBlast());
        }

        IEnumerator RoomBlast()
        {
            List<IRoomBlastTarget> targets = new();
            GetComponentsInChildren(targets);

            targets = targets.Where(t => t.CanGreening()).ToList();
            if (targets.Count == 0)
                yield break;

            PostProccessManager.Singleton.ToDarkMode();

            targets.ForEach(g => g.BeginGreening());

            yield return TileCulling.Singleton.StopCulling();

            RoomVirtualCamera.Singleton.BeginRoomBlast(
                DungeonManager.Singleton.TilePosToWorld(minTile),
                DungeonManager.Singleton.TilePosToWorld(maxTile));

            var blendTime = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time;
            yield return new WaitForSeconds(blendTime + 0.1f);

            var co = NotificationUI.Singleton.Notice(NotificationUI.NotificationType.Blast, 1);
            yield return new WaitForSeconds(0.1f);
            targets.ForEach(t => t.Greening());

            yield return co;

            RoomBlastRateUI.Singleton.Inactive();
            RoomVirtualCamera.Singleton.EndRoomBlast();


            PostProccessManager.Singleton.ToLightMode();
            if (GameRuleManager.Singleton.IsFinished)
                yield break;

            yield return new WaitForSeconds(1);
            yield return TileCulling.Singleton.RestartCulling();

            co = null;
        }


#if UNITY_EDITOR

        void AddRoomTileInEditor(Vector2Int pos)
        {
            var tile = DungeonManager.Singleton.Tilemap.GetTile<TerrainTile>((Vector3Int)pos);
            if (tile == null)
                return;

            if (tile.CurrentTileObject.TileType == TileType.Wall)
                return;
            if (tile.CurrentTileObject.TileType == TileType.SpwanLake)
                return;

            if (!roomTilePoss.Add(pos))
                return;

            if (tile.CurrentTileObject.TileType == TileType.Entrance)
                return;

            AddRoomTileInEditor(pos + Vector2Int.up);
            AddRoomTileInEditor(pos + Vector2Int.down);
            AddRoomTileInEditor(pos + Vector2Int.left);
            AddRoomTileInEditor(pos + Vector2Int.right);
        }
        private void OnDrawGizmos()
        {
            roomTilePoss.Clear();
            AddRoomTileInEditor((Vector2Int)DungeonManager.Singleton.Tilemap.WorldToCell(transform.position));
            foreach (var pos in roomTilePoss)
            {
                Gizmos.DrawSphere(DungeonManager.Singleton.TilePosToWorld(pos), 0.1f);
            }
            Gizmos.DrawSphere(transform.position, 0.15f);
        }

#endif
    }
}
