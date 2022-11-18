using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ReLeaf
{
    public class EnemyCore : MarkerManager<WeakMarker>, IEnemyDamageable
    {
        [SerializeField]
        EnemyDamageableInfo enemyBaseInfo;

        [field:SerializeField,ReadOnly]
        public float HP { get; private set; }

        EnemyMover enemyMover;
        private void Start()
        {
            TryGetComponent(out enemyMover);
            Init(true);
            HP = enemyBaseInfo.HPMax;
        }
        private void OnDestroy()
        {
            Uninit();
        }

        public void BeginWeekMarker()
        {
            foreach (var defaultLocalPos in enemyBaseInfo.WeakLocalTilePos)
            {
                var worldTilePos = enemyMover.TilePos + enemyMover.GetRotatedLocalPos(defaultLocalPos);
                var tile = DungeonManager.Instance.GetGroundTile(worldTilePos);

                if (tile != null && (tile.tileType == TileType.Foundation || tile.tileType == TileType.Plant || tile.tileType == TileType.Sand))
                {
                    var marker = SetMarker(worldTilePos, transform);
                    if (marker != null)
                    {
                        marker.Init(this);
                    }
                }

            }
        }
        public void EndWeekMarker()
        {
            ResetAllMarker();
        }
        public void DamagedGreening(Vector2Int tilePos, float atk)
        {
            if (HP == 0)
                return;
            if (markers.Remove(tilePos))
            {
                Damaged(atk * (enemyBaseInfo.WeakLocalTilePos.Length - (markers.Count )));
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
                Destroy(gameObject);
                return;
            }
            HP -= atk;
        }
    }
}
