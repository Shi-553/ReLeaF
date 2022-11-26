using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace ReLeaf
{
    public class EnemyCore : MonoBehaviour, IEnemyDamageable
    {
        [SerializeField]
        EnemyDamageableInfo enemyBaseInfo;

        [field:SerializeField,ReadOnly]
        public float HP { get; private set; }

        EnemyMover enemyMover;

        [SerializeField]
        GameObject specialPowerPrefab;
        [SerializeField]
        MarkerManager weakMarkerManager;


        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyBaseInfo.HPMax;
        }
        private void Death()
        {
            Instantiate(specialPowerPrefab, transform.position, Quaternion.identity, transform.parent);
            Destroy(gameObject);
        }

        public void BeginWeekMarker()
        {
            foreach (var defaultLocalPos in enemyBaseInfo.WeakLocalTilePos)
            {
                var worldTilePos = enemyMover.TilePos + MathExtension.GetRotatedLocalPos(enemyMover.Dir,defaultLocalPos);

                if (DungeonManager.Singleton.TryGetTile(worldTilePos,out var tile) && tile.CanEnemyMove)
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
                Damaged(atk * (enemyBaseInfo.WeakLocalTilePos.Length - (weakMarkerManager.Markers.Count )));
                return;
            }
        }
        public void Damaged(float atk)
        {
            if (HP == 0)
                return;
            Debug.Log(atk+"É_ÉÅÅ[ÉW!");

            if (HP - atk <= 0)
            {
                HP = 0;
                Death();
                return;
            }
            HP -= atk;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                if (collision.TryGetComponent<Plant>(out var plant)&& plant.IsInvincible)
                {
                    Death();
                }
            }
        }
    }
}
