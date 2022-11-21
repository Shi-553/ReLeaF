using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SharkController : MonoBehaviour
    {

        [SerializeField]
        Vision searchVision;

        [SerializeField]
        EnemyMoverInfo enemyMoverInfo;
        EnemyMover mover;
        IEnemyAttacker attacker;
        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out mover);
        }
        void Update()
        {
            if (!searchVision.ShouldFoundTarget)
            {
                return;
            }
            if (attacker.IsAttack)
            {
                return;
            }
            var target = searchVision.Targets.MinBy(t => (t.position - transform.position).sqrMagnitude);
            var targetTilePos = DungeonManager.Instance.WorldToTilePos(target.position);

            mover.UpdateDir(targetTilePos, true);

            if (mover.Move(enemyMoverInfo.Speed, true))
            {
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
