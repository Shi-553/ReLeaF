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
        EnemyMover movable;
        IEnemyAttacker attacker;
        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out movable);
        }
        void Update()
        {
            if (!searchVision.ShouldFoundTarget)
            {
                return;
            }
            if (attacker.IsAttack) { 
                return;
            }
            var target = searchVision.Targets.MinBy(t => (t.position - transform.position).sqrMagnitude);
            var targetTilePos = (Vector2Int)DungeonManager.Instance.WorldToTilePos(target.position);


            if (movable.MoveTo(targetTilePos, enemyMoverInfo.Speed,true)) 
            {
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
