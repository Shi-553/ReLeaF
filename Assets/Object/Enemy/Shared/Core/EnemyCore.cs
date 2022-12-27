using System;
using System.Collections;
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
        public bool IsDeath => HP <= 0;

        EnemyMover enemyMover;

        [SerializeField]
        GameObject specialPowerPrefab;
        [SerializeField]
        MarkerManager weakMarkerManager;
        [SerializeField]
        AudioInfo seEnemyDeath;

        [SerializeField]
        AudioInfo seEnemyDamaged;


        int greeningCount = 0;

        public event Action OnDeath;

        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyDamageableInfo.HPMax;
        }
        private IEnumerator Death()
        {
            if (TryGetComponent(out EnemyAttacker attacker))
            {
                attacker.Stop();
            }

            for (int x = 0; x < enemyMover.TileSize.x; x++)
            {
                for (int y = 0; y < enemyMover.TileSize.y; y++)
                {
                    DungeonManager.Singleton.SowSeed(new Vector2Int(enemyMover.TilePos.x + x, enemyMover.TilePos.y + y), true);
                }
            }
            ResetWeekMarker();

            OnDeath?.Invoke();

            foreach (var collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            if (TryGetComponent<EnemyAnimationBase>(out var animationBase))
            {
                yield return animationBase.DeathAnimation();
            }
            Destroy(gameObject);

            if (specialPowerPrefab != null)
                Instantiate(specialPowerPrefab, transform.position, Quaternion.identity, transform.parent);
        }

        public void SetWeekMarker()
        {
            greeningCount = 0;

            var localPoss = enemyDamageableInfo.WeakLocalTilePos.GetLocalTilePosList(enemyMover.DirNotZero);
            foreach (var localPos in localPoss)
            {
                var worldTilePos = enemyMover.TilePos + localPos;

                if (DungeonManager.Singleton.TryGetTile(worldTilePos, out var tile) && tile.CanOrAleeadyGreening(true))
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
                greeningCount++;
                Damaged(atk * greeningCount);
                return;
            }
        }
        public void Damaged(float atk)
        {
            if (HP == 0 || DamageValueEffectManager.Singleton == null)
                return;

            DamageValueEffectManager.Singleton.SetDamageValueEffect((int)atk, enemyMover.WorldCenter);

            if (HP - atk <= 0)
            {
                HP = 0;
                SEManager.Singleton.Play(seEnemyDeath, transform.position);
                StartCoroutine(Death());
                return;
            }
            HP -= atk;
            SEManager.Singleton.Play(seEnemyDamaged, transform.position);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                if (collision.TryGetComponent<Plant>(out var plant) && plant.IsInvincible)
                {
                    StartCoroutine(Death());
                }
            }
        }
    }
}
