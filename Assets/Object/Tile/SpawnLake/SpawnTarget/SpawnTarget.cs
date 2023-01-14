using System.Collections;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SpawnTarget : MonoBehaviour
    {

        [SerializeField, Rename("何秒毎に湧かせるか")]
        float spwanInterval = 10.0f;
        public float SpwanInterval => spwanInterval;

        [SerializeField]
        SpawnTargetInfo info;

        WaitForSeconds wait;
        public SpawnLakeGroup Group { get; set; }

        public Vector2Int TilePos { get; private set; }
        public Vector3 WorldPos { get; private set; }
        private void Awake()
        {
            TilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);
            WorldPos = DungeonManager.Singleton.TilePosToWorld(TilePos);

            wait = new WaitForSeconds(SpwanInterval);
        }


        public EnemyMover Spawn()
        {
            var (element, index) = Group.SpawnLakeDic.Values
                .Where(lake => !lake.IsGreening)
                .MinBy(lake => (lake.TilePos - TilePos).sqrMagnitude);

            var currentWorldPos = DungeonManager.Singleton.TilePosToWorld(element.TilePos);

            var enemy = Instantiate(info.EnemyPrefab,
                currentWorldPos,
                Quaternion.identity,
                transform.parent);

            enemy.Init(TilePos);

            var enemyAnimation = enemy.GetComponent<EnemyAnimationBase>();
            enemyAnimation.Init();
            var co = enemyAnimation.SpawnAnimation(currentWorldPos, WorldPos, info.SpwanInitAnimationTime);

            StartCoroutine(co);

            return enemy;
        }

        Coroutine coroutine;
        public void StopSpawnInterval()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        public void StartSpawnInterval()
        {
            StopSpawnInterval();
            coroutine = StartCoroutine(StartSpawnIntervalImpl());
        }
        IEnumerator StartSpawnIntervalImpl()
        {
            while (true)
            {
                yield return wait;

                if (Group.IsGreening || !Group.CanSpawn)
                    break;

                var enemy = Spawn();
                yield return null;

                var initTilePos = enemy.TilePos;
                yield return new WaitUntil(() => enemy.TilePos != initTilePos);
            }
            coroutine = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var tilePos = DungeonManager.Singleton.Tilemap.WorldToCell(transform.position);
            var size = (Vector3)(Vector2)info.EnemyPrefab.TileSize * DungeonManager.CELL_SIZE;
            var worldPos = DungeonManager.Singleton.Tilemap.CellToWorld(tilePos) + size / 2;

            Gizmos.matrix = Matrix4x4.TRS(worldPos, Quaternion.identity, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
    }
}
