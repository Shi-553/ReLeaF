using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class CrabAttacker : EnemyAttacker
    {
        public override EnemyAttackInfo EnemyAttackInfo => crabAttackInfo;

        [SerializeField]
        CrabAttackInfo crabAttackInfo;

        Vector2Int[] attackPoss;

        [SerializeField]
        AudioInfo seBeforeAttack;

        [SerializeField]
        AudioInfo seAttack;

        public override IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos)
        {
            foreach (var local in crabAttackInfo.AttackLocalTilePos.GetLocalTilePosList(dir))
            {
                var attackPos = pos + local;

                if (!DungeonManager.Singleton.TryGetTile(attackPos, out var tile))
                    continue;

                if (tile.CanEnemyAttack(includeMoveabePos))
                {
                    yield return attackPos;
                }
            }
        }

        protected override void OnStartAiming()
        {
            attackPoss = GetAttackRange(enemyMover.TilePos, enemyMover.DirNotZero, true).ToArray();
            foreach (var attackPos in attackPoss)
            {
                attackMarkerManager.SetMarker<TargetMarker>(attackPos);
            }
            enemyCore.SetWeekMarker();
            SEManager.Singleton.Play(seBeforeAttack, transform.position);
        }
        protected override IEnumerator OnStartDamageing()
        {
            yield return new WaitForSeconds(crabAttackInfo.AttackBeforeDamageTime);
            if (this == null)
                yield break;

            var player = PlayerCore.Singleton;
            foreach (var attackPos in attackPoss)
            {
                if (player.Mover.TilePos == attackPos)
                {
                    player.Damaged(crabAttackInfo.ATK, (player.transform.position - transform.position).normalized * crabAttackInfo.KnockBackPower);
                }
                if (!DungeonManager.Singleton.TryGetTile<Plant>(attackPos, out var plant))
                    continue;

                plant.Damaged(crabAttackInfo.ATK, DamageType.Direct);

            }
            attackMarkerManager.ResetAllMarker();
            SEManager.Singleton.Play(seAttack, transform.position);
        }


        protected override void OnEndCoolTime()
        {
            enemyCore.ResetWeekMarker();
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Plant"))
            {
                if (collider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (MathExtension.DuringExists(plant.TilePos, enemyMover.TilePos, enemyMover.TilePos + enemyMover.TileSize, false))
                    {
                        plant.Damaged(crabAttackInfo.ATK, DamageType.Direct);
                        return;
                    }
                }
            }
        }
    }
}
