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

        public Transform EnemyRoot { get; private set; }

        public void Awake()
        {
            var tilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            maxTile = tilePos;
            minTile = tilePos;

            AddRoomTile(tilePos);

            foreach (var setRoom in GetComponentsInChildren<ISetRoom>())
            {
                setRoom.SetRoom(this);
            }


            var enemy = GetComponentInChildren<EnemyCore>();
            EnemyRoot = enemy != null ? enemy.transform.parent : transform;

        }
        private void Start()
        {
            if (ContainsRoom(PlayerMover.Singleton.TilePos))
            {
                PlayerMover.Singleton.Room = this;
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
            using var _ = RobotGreening.Singleton.StartGreening();

            RobotMover.Singleton.Sprite.sortingOrder++;

            RoomVirtualCamera.Singleton.BeginRoomBlast(
                DungeonManager.Singleton.TilePosToWorld(minTile),
                DungeonManager.Singleton.TilePosToWorld(maxTile));

            var blendTime = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time;
            yield return new WaitForSeconds(blendTime);

            List<IRoomBlastTarget> targets = new();

            GetComponentsInChildren(targets);

            targets = targets.Where(t => t.CanGreening()).ToList();

            targets.ForEach(g => g.BeginGreening());

            targets.Sort((a, b) =>
            (int)((a.Position - RobotMover.Singleton.transform.position).sqrMagnitude - (b.Position - RobotMover.Singleton.transform.position).sqrMagnitude));

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                yield return Move(target);
                yield return Attack();
                target.Greening();
            }

            RoomBlastRateUI.Singleton.Inactive();

            RoomVirtualCamera.Singleton.EndRoomBlast();
            RobotMover.Singleton.IsStop = false;

            RobotMover.Singleton.Sprite.sortingOrder--;
            RobotMover.Singleton.UpdateManualOperation(PlayerCore.Singleton.transform.position, 50, true, 1);

            co = null;
        }
        IEnumerator Attack()
        {
            RobotMover.Singleton.IsStop = true;
            RobotMover.Singleton.GetComponent<RobotAnimation>().Thrust();
            yield return new WaitForSeconds(0.5f);

        }
        IEnumerator Move(IRoomBlastTarget target)
        {
            RobotMover.Singleton.IsStop = false;
            while (true)
            {
                var targetPos = target.Position;

                RobotMover.Singleton.UpdateManualOperation(targetPos, 50, true, 1);

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
            if (tile.CurrentTileObject.TileType == TileType.SpwanLake)
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

        private void OnDrawGizmos()
        {
            roomTilePoss.Clear();
            AddRoomTileInEdior((Vector2Int)DungeonManager.Singleton.Tilemap.WorldToCell(transform.position));
            foreach (var pos in roomTilePoss)
            {
                Gizmos.DrawSphere(DungeonManager.Singleton.TilePosToWorld(pos), 0.1f);
            }
            Gizmos.DrawSphere(transform.position, 0.15f);
        }

#endif
    }
}
