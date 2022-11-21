using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class SharkAttacker : MarkerManager<AttackMarker>, IEnemyAttacker
    {


        [field: SerializeField]
        SharkAttackInfo SharkAttackInfo { get; set; }
        public EnemyAttackInfo EnemyAttackInfo => SharkAttackInfo;

        [field: SerializeField, ReadOnly]
        public AttackTransition Transition { get; set; }

        EnemyMover enemyMover;
        EnemyCore enemyDamageable;

        [SerializeField, ReadOnly]
        Vector2Int attackTargetPos;

        private void Awake()
        {
            TryGetComponent(out enemyMover);
            TryGetComponent(out enemyDamageable);
        }

        void IEnemyAttacker.OnStartAiming()
        {
            enemyDamageable.BeginWeekMarker();

            for (int i = 0; i < SharkAttackInfo.Range; i++)
            {
                var worldTilePos = enemyMover.TilePos + enemyMover.Dir * (i + 1);
                var tile=DungeonManager.Instance.GetGroundTile(worldTilePos);

                if (tile != null && (tile.tileType == TileType.Foundation || tile.tileType == TileType.Plant || tile.tileType == TileType.Sand))
                {
                    SetMarker(worldTilePos, null);
                    attackTargetPos = worldTilePos;
                    continue;
                }
                break;
            }
        }
        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            enemyDamageable.EndWeekMarker();

            while (true)
            {
                enemyMover.UpdateDir(attackTargetPos, false);
                if (enemyMover.Move(SharkAttackInfo.Speed,false))
                {
                    break;
                }
                yield return null;
            }
            ResetAllMarker();
        }
        private void OnDestroy()
        {
            ResetAllMarker();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Transition != AttackTransition.Damageing)
            {
                return;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
                {
                    player.Damaged(SharkAttackInfo.ATK, (Vector2)enemyMover.Dir * SharkAttackInfo.KnockBackPower);
                }
            }
            if (collision.gameObject.CompareTag("Plant"))
            {
                if (collision.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    plant.Damaged(SharkAttackInfo.ATK, SharkAttackInfo.DamageType);
                }
            }
        }
    }
}