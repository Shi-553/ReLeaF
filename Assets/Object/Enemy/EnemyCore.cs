using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class EnemyCore : MonoBehaviour, IEnemyDamageable
    {
        [SerializeField]
        EnemyDamageableInfo enemyBaseInfo;

        [field: SerializeField, ReadOnly]
        public float HP { get; private set; }

        EnemyMover enemyMover;

        [SerializeField]
        GameObject specialPowerPrefab;
        [SerializeField]
        MarkerManager weakMarkerManager;
        [SerializeField]
        AudioInfo seEnemyDeath;

        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyBaseInfo.HPMax;
        }
        private void Death()
        {
            Instantiate(specialPowerPrefab, transform.position, Quaternion.identity, transform.parent);
            for (int x = 0; x < enemyMover.TileSize.x; x++)
            {
                for (int y = 0; y < enemyMover.TileSize.y; y++)
                {
                    DungeonManager.Singleton.ToEnemyPlant(new Vector2Int(enemyMover.TilePos.x + x, enemyMover.TilePos.y + y));
                }
            }
            Destroy(gameObject);
        }

        public void BeginWeekMarker()
        {
            foreach (var defaultLocalPos in enemyBaseInfo.WeakLocalTilePos)
            {
                var worldTilePos = enemyMover.TilePos + defaultLocalPos.GetRotatedLocalPos(enemyMover.Dir, enemyMover.TileSize);

                if (DungeonManager.Singleton.TryGetTile(worldTilePos, out var tile) && tile.CanEnemyMove)
                {
                    var marker = weakMarkerManager.SetMarker<WeakMarker>(worldTilePos);
                    if (marker != null)
                    {
                        marker.SetEnemyDamageable(this);
                    }
                }

            }
        }
        public void EndWeekMarker()
        {
            weakMarkerManager.ResetAllMarker();
        }
        public void DamagedGreening(Vector2Int tilePos, float atk)
        {
            if (HP == 0)
                return;
            if (weakMarkerManager.ResetMarker(tilePos))
            {
                Damaged(atk * (enemyBaseInfo.WeakLocalTilePos.Length - (weakMarkerManager.Markers.Count)));
                return;
            }
        }
        public void Damaged(float atk)
        {
            if (HP == 0)
                return;
            Debug.Log(atk + "É_ÉÅÅ[ÉW!");

            if (HP - atk <= 0)
            {
                HP = 0;
                SEManager.Singleton.Play(seEnemyDeath, transform.position);
                Death();
                return;
            }
            HP -= atk;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                if (collision.TryGetComponent<Plant>(out var plant) && plant.IsInvincible)
                {
                    Death();
                }
            }
        }
    }
}
