using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SharkAttacker : EnemyAttacker
    {

        [field: SerializeField]
        SharkAttackInfo SharkAttackInfo { get; set; }
        public override EnemyAttackInfo EnemyAttackInfo => SharkAttackInfo;



        [SerializeField, ReadOnly]
        Vector2Int attackStartPos;
        [SerializeField, ReadOnly]
        Vector2Int attackTargetPos;

        List<Vector2Int> buffer = new();

        [SerializeField]
        AudioInfo seBeforeAttack;
        [SerializeField]
        AudioInfo seAttack;

        EnemyDirectAttackController enemyDirectAttackController;

        protected override void Init()
        {
            TryGetComponent(out enemyDirectAttackController);
        }

        protected override void OnStartAiming()
        {
            enemyCore.SetWeekMarker();

            attackStartPos = enemyMover.TilePos;

            enemyMover.GetCheckPoss(enemyMover.TilePos, enemyMover.DirNotZero, buffer);
            attackTargetPos = buffer.Last() + (enemyMover.DirNotZero * (SharkAttackInfo.Range));
            var ranges = GetAttackRange(enemyMover.TilePos, enemyMover.DirNotZero, true).ToArray();
            foreach (var target in ranges)
            {
                attackMarkerManager.SetMarker<TargetMarker>(target, enemyMover.DirNotZero.GetRotation());
            }
            SEManager.Singleton.Play(seBeforeAttack, enemyMover.WorldCenter);

            if (ranges.Length <= 4)
                enemyDirectAttackController.AddImpossibleLastTargets();
        }
        protected override IEnumerator OnStartDamageing()
        {
            enemyCore.ResetWeekMarker();
            SEManager.Singleton.Play(seAttack, enemyMover.WorldCenter);

            enemyMover.UpdateTargetStraight(attackTargetPos);

            while (true)
            {
                if (enemyMover.Move(SharkAttackInfo.Speed, true) != EnemyMover.MoveResult.Moveing)
                {
                    break;
                }
                yield return null;

                if (this == null)
                    yield break;
            }
        }
        protected override void OnStartCoolTime()
        {
            attackMarkerManager.ResetAllMarker();
        }
        public override IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos)
        {
            List<Vector2Int> returns = new(2);
            List<Vector2Int> buffer = new(2);

            // É_ÉÅÇ»èÍçá falseÇï‘Ç∑
            bool CheckAndAdd(Vector2Int pos)
            {
                if (returns.Contains(pos))
                    return true;

                if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                {
                    return false;
                }
                if (tile.CanEnemyMoveAttack(includeMoveabePos) && tile.InstancedParent.CanEnemyMoveAttack(includeMoveabePos))
                {
                    buffer.Add(pos);
                    return true;
                }

                return false;
            }
            var range = SharkAttackInfo.Range + 1;

            for (int i = 0; i < range; i++)
            {
                var worldTilePosBase = pos + dir * i;

                // ècÇ©â°Ç©Ç≈xÇ∆yÇÃÉãÅ[ÉvèáÇïœÇ¶ÇƒÇÈ
                if (dir.x != 0)
                {
                    for (int x = 0; x < enemyMover.TileSize.x; x++)
                    {
                        for (int y = 0; y < enemyMover.TileSize.y; y++)
                        {
                            if (!CheckAndAdd(new Vector2Int(worldTilePosBase.x + x, worldTilePosBase.y + y)))
                                return returns;
                        }
                        returns.AddRange(buffer);
                        buffer.Clear();
                    }
                }
                else
                {
                    for (int y = 0; y < enemyMover.TileSize.y; y++)
                    {
                        for (int x = 0; x < enemyMover.TileSize.x; x++)
                        {
                            if (!CheckAndAdd(new Vector2Int(worldTilePosBase.x + x, worldTilePosBase.y + y)))
                                return returns;
                        }
                        returns.AddRange(buffer);
                        buffer.Clear();
                    }

                }
            }
            return returns;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Transition != AttackTransition.Damageing)
            {
                return;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent<PlayerCore>(out var player))
                {
                    player.Damaged(SharkAttackInfo.ATK, (Vector2)enemyMover.DirNotZero * SharkAttackInfo.KnockBackPower);
                }
            }
        }
        protected override void OnTriggerStay2D(Collider2D collider)
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
                    else
                    {
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
}