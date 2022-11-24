using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class SharkAttacker : MonoBehaviour, IEnemyAttacker
    {


        [field: SerializeField]
        SharkAttackInfo SharkAttackInfo { get; set; }
        public EnemyAttackInfo EnemyAttackInfo => SharkAttackInfo;

        [field: SerializeField, ReadOnly]
        public AttackTransition Transition { get; set; }

        EnemyMover enemyMover;
        EnemyCore enemyDamageable;

        [SerializeField, ReadOnly]
        Vector2Int attackStartPos;
        [SerializeField, ReadOnly]
        Vector2Int attackTargetPos;


        private void Awake()
        {
            TryGetComponent(out enemyMover);
            TryGetComponent(out enemyDamageable);

        }

        void IEnemyAttacker.OnStartAiming()
        {
            attackStartPos = enemyMover.TilePos;
            enemyDamageable.BeginWeekMarker();

            attackTargetPos = GetAttackRange(enemyMover.TilePos,enemyMover.Dir,true).Last();
        }
        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            enemyDamageable.EndWeekMarker();

            while (true)
            {
                enemyMover.UpdateDir(attackTargetPos, false);
                if (enemyMover.Move(SharkAttackInfo.Speed, false))
                {
                    break;
                }
                yield return null;
            }
        }

        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool isDamagableOnly)
        {
            for (int i = 0; i < SharkAttackInfo.Range; i++)
            {
                var worldTilePos = enemyMover.TilePos + enemyMover.Dir * (i + 1);
                if (!DungeonManager.Instance.TryGetTile(worldTilePos, out var tile) || !tile.CanEnemyMove)
                {
                    yield break;
                }
                if (tile.CanEnemyAttack(isDamagableOnly))
                {
                    yield return worldTilePos;
                }
                
            }
        }
        public int GetAttackRangeCount(Vector2Int pos, Vector2Int dir, bool isDamagableOnly)
        {
            int count = 0;
            for (int i = 0; i < SharkAttackInfo.Range; i++)
            {
                var worldTilePos = enemyMover.TilePos + enemyMover.Dir * (i + 1);

                if (!DungeonManager.Instance.TryGetTile(worldTilePos, out var tile)|| !tile.CanEnemyMove)
                {
                    return count;
                }

                if (tile.CanEnemyAttack(isDamagableOnly))
                {
                    count++;
                }
            }
            return count;
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
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (Transition != AttackTransition.Damageing)
            {
                return;
            }
            if (collider.gameObject.CompareTag("Plant"))
            {
                if (collider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (MathExtension.DuringExists(plant.TilePos, attackStartPos, attackTargetPos))
                    {
                        plant.Damaged(SharkAttackInfo.ATK, DamageType.Direct);
                    }
                }
            }
        }
    }
}