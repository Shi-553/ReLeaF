using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField]
        WeakMarker weakMarkerPrefab;

        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyBaseInfo.HPMax;
            weakMarkerManager.InitPool(weakMarkerPrefab);
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
                var tile = DungeonManager.Instance.GetGroundTile(worldTilePos);

                if (tile != null && (tile.tileType == TileType.Foundation || tile.tileType == TileType.Plant || tile.tileType == TileType.Sand))
                {
                    var marker = weakMarkerManager.SetMarker<WeakMarker>(worldTilePos);
                    if (marker != null)
                    {
                        marker.Init(this);
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
    }
}
