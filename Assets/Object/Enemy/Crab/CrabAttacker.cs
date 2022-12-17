using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class CrabAttacker : MonoBehaviour, IEnemyAttacker
    {
        public AttackTransition Transition { get; set; }
        Coroutine IEnemyAttacker.AttackCo { get; set; }
        public EnemyAttackInfo EnemyAttackInfo => crabAttackInfo;

        [SerializeField]
        CrabAttackInfo crabAttackInfo;

        EnemyMover mover;
        EnemyCore enemyCore;

        [SerializeField]
        MarkerManager targetMarkerManager;

        Vector2Int[] attackPoss;

        [SerializeField]
        AudioInfo seBeforeAttack;

        [SerializeField]
        AudioInfo seAttack;

        void IEnemyAttacker.StopImpl()
        {
            targetMarkerManager.ResetAllMarker();
        }
        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos)
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

        void IEnemyAttacker.OnStartAiming()
        {
            attackPoss = GetAttackRange(mover.TilePos, mover.DirNotZero, true).ToArray();
            foreach (var attackPos in attackPoss)
            {
                targetMarkerManager.SetMarker<TargetMarker>(attackPos);
            }
            enemyCore.SetWeekMarker();
            SEManager.Singleton.Play(seBeforeAttack, transform.position);
        }
        IEnumerator IEnemyAttacker.OnStartDamageing()
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
            targetMarkerManager.ResetAllMarker();
            SEManager.Singleton.Play(seAttack, transform.position);
        }


        void IEnemyAttacker.OnEndCoolTime()
        {
            enemyCore.ResetWeekMarker();
        }
        void Start()
        {
            TryGetComponent(out mover);
            TryGetComponent(out enemyCore);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Plant"))
            {
                if (collider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (MathExtension.DuringExists(plant.TilePos, mover.TilePos, mover.TilePos + mover.TileSize, false))
                    {
                        plant.Damaged(crabAttackInfo.ATK, DamageType.Direct);
                        return;
                    }
                }
            }
        }
    }
}
