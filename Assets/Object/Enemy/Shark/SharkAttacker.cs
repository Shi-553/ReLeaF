using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

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

        [SerializeField]
        MarkerManager attackMarkerManager;

        List<Vector2Int> buffer = new();

        [SerializeField]
        AudioInfo seBeforeAttack;
        [SerializeField]
        AudioInfo seAttack;



        private void Awake()
        {
            TryGetComponent(out enemyMover);
            TryGetComponent(out enemyDamageable);

        }

        void IEnemyAttacker.OnStartAiming()
        {
            enemyDamageable.SetWeekMarker();

            attackStartPos = enemyMover.TilePos;
            var dir = enemyMover.Dir == Vector2Int.zero ? Vector2Int.down : enemyMover.Dir;
            enemyMover.GetCheckPoss(enemyMover.TilePos, dir, buffer);
            attackTargetPos = buffer.Last() + (dir * (SharkAttackInfo.Range - 1));

            foreach (var target in GetAttackRange(enemyMover.TilePos, enemyMover.Dir, false))
            {
                attackMarkerManager.SetMarker<TargetMarker>(target, enemyMover.Dir.GetRotation());
            }
            SEManager.Singleton.Play(seBeforeAttack, transform.position);
        }
        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            enemyDamageable.ResetWeekMarker();
            SEManager.Singleton.Play(seAttack, transform.position);

            enemyMover.UpdateTargetStraight(attackTargetPos);

            while (true)
            {
                if (enemyMover.Move(SharkAttackInfo.Speed) != EnemyMover.MoveResult.Moveing)
                {
                    break;
                }
                yield return null;

                if (this == null)
                yield break;

            }
        }
        void IEnemyAttacker.OnStartCoolTime()
        {
            attackMarkerManager.ResetAllMarker();
        }
        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool isDamagableOnly)
        {
            List<Vector2Int> returns = new(2);
            List<Vector2Int> buffer = new(2);

            for (int i = 0; i < SharkAttackInfo.Range + 1; i++)
            {
                var worldTilePosBase = pos + dir * i;
                for (int x = 0; x < enemyMover.TileSize.x; x++)
                {
                    for (int y = 0; y < enemyMover.TileSize.y; y++)
                    {
                        var worldTilePos = new Vector2Int(worldTilePosBase.x + x, worldTilePosBase.y + y);
                        if (returns.Contains(worldTilePos))
                            continue;
                        if (!DungeonManager.Singleton.TryGetTile(worldTilePos, out var tile) || !tile.CanEnemyMove)
                        {
                            return returns;
                        }
                        if (tile.CanEnemyAttack(isDamagableOnly))
                        {
                            buffer.Add(worldTilePos);
                        }
                    }
                }
                returns.AddRange(buffer);
                buffer.Clear();
            }
            return returns;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Transition != AttackTransition.Damageing)
            {
                return;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent<PlayerCore>(out var player))
                {
                    player.Damaged(SharkAttackInfo.ATK, (Vector2)enemyMover.Dir * SharkAttackInfo.KnockBackPower);
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Plant"))
            {
                if (collider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (Transition == AttackTransition.Damageing)
                    {
                        if (MathExtension.DuringExists(plant.TilePos, attackStartPos, attackTargetPos, true))
                        {
                            plant.Damaged(SharkAttackInfo.ATK, DamageType.Direct);
                            return;
                        }
                    }

                    if (MathExtension.DuringExists(plant.TilePos, enemyMover.TilePos, enemyMover.TilePos + enemyMover.TileSize, false))
                    {
                        plant.Damaged(SharkAttackInfo.ATK, DamageType.Direct);
                        return;
                    }
                }
            }
        }
    }
}