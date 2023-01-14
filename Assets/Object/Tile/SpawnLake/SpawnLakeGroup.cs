using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SpawnLakeGroup : MonoBehaviour, IRoomBlastTarget, ISetRoom
    {
        readonly Dictionary<Vector2Int, SpawnLake> spawnLakeDic = new();

        public IReadOnlyDictionary<Vector2Int, SpawnLake> SpawnLakeDic => spawnLakeDic;

        List<SpawnTarget> targets = new();
        public IReadOnlyList<SpawnTarget> Targets => targets;



        public bool CanSpawn { get; set; } = true;
        public bool IsGreening => spawnLakeDic.All(t => t.Value.IsGreening);


        void Awake()
        {
            PlayerMover.Singleton.OnChangeRoom += OnChangeRoom;
            GameRuleManager.Singleton.OnChangeState += OnChangeState;

            transform.GetComponentsInChildren(targets);
            targets.ForEach(t => t.Group = this);

            AddTarget(DungeonManager.Singleton.WorldToTilePos(transform.position));

            if (spawnLakeDic.Count == 0)
                Debug.LogWarning("湖がない", gameObject);
        }

        void AddTarget(Vector2Int pos)
        {
            if (DungeonManager.Singleton.TryGetTile<SpawnLake>(pos, out var lake) && spawnLakeDic.TryAdd(pos, lake))
            {
                AddTarget(pos + Vector2Int.up);
                AddTarget(pos + Vector2Int.down);
                AddTarget(pos + Vector2Int.left);
                AddTarget(pos + Vector2Int.right);
            }
        }


        private void OnChangeRoom(Room room)
        {
            if (Room != room)
            {
                Stop();
                return;
            }
            foreach (var target in targets)
            {
                target.StartSpawnInterval();
            }
        }

        public void Stop()
        {
            targets.ForEach(t => t.StopSpawnInterval());
        }

        private void OnChangeState(GameRuleState obj)
        {
            if (obj != GameRuleState.Playing || !CanSpawn)
            {
                Stop();
                return;
            }
        }

        public List<EnemyMover> SpawnAllNow() => targets.Select(t => t.Spawn()).ToList();


        public Vector3 Position
        {
            get
            {
                Vector2 center = Vector2.zero;
                if (SpawnLakeDic.Count > 0)
                {
                    SpawnLakeDic.Values.ForEach(lake => center += lake.TilePos);
                    center /= SpawnLakeDic.Count;
                }
                return DungeonManager.Singleton.TilePosToWorld(center);
            }
        }
        void IRoomBlastTarget.BeginGreening()
        {
            CanSpawn = false;
            Stop();
        }
        public void Greening()
        {
            SpawnLakeDic.Values.ForEach(lake => DungeonManager.Singleton.SowSeed(lake.TilePos, true));
        }

        public Room Room { get; private set; }
        public void SetRoom(Room room) => Room = room;


#if UNITY_EDITOR
        void AddRoomTileInEdior(Vector2Int pos)
        {
            var obj = DungeonManager.Singleton.Tilemap.GetInstantiatedObject((Vector3Int)pos);
            if (obj == null)
                return;
            var tile = obj.GetComponent<SpawnLake>();
            if (tile == null) return;

            if (!spawnLakeDic.TryAdd(pos, tile))
                return;

            AddRoomTileInEdior(pos + Vector2Int.up);
            AddRoomTileInEdior(pos + Vector2Int.down);
            AddRoomTileInEdior(pos + Vector2Int.left);
            AddRoomTileInEdior(pos + Vector2Int.right);
        }
        private void OnDrawGizmosSelected()
        {
            spawnLakeDic.Clear();
            AddRoomTileInEdior((Vector2Int)DungeonManager.Singleton.Tilemap.WorldToCell(transform.position));
            Gizmos.color = Color.red;
            foreach (var pos in spawnLakeDic.Keys)
            {
                Vector3 world = DungeonManager.Singleton.TilePosToWorld(pos);
                world.z -= 0.1f;
                Gizmos.DrawSphere(world, 0.1f);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.15f);
        }
#endif
    }
}
