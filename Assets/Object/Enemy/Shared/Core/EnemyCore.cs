using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class EnemyCore : MonoBehaviour, IEnemyDamageable
    {
        [SerializeField]
        EnemyDamageableInfo enemyDamageableInfo;

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
            HP = enemyDamageableInfo.HPMax;
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

        public void SetWeekMarker()
        {
            foreach (var localPos in enemyDamageableInfo.WeakLocalTilePos.GetLocalTilePosList(enemyMover.Dir))
            {
                var worldTilePos = enemyMover.TilePos + localPos;

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
        public void ResetWeekMarker()
        {
            weakMarkerManager.ResetAllMarker();
        }
        public void DamagedGreening(Vector2Int tilePos, float atk)
        {
            if (HP == 0)
                return;
            if (weakMarkerManager.ResetMarker(tilePos))
            {
                Damaged(atk * (enemyDamageableInfo.WeakLocalTilePos.GetLocalTilePosList(enemyMover.Dir).Length - (weakMarkerManager.Markers.Count)));
                return;
            }
        }
        public void Damaged(float atk)
        {
            if (HP == 0)
                return;
            Debug.Log(atk + "É_ÉÅÅ[ÉW!");

            DamageValueEffectManager.Singleton.SetDamageValueEffect((int)atk, enemyMover.WorldCenter);

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
