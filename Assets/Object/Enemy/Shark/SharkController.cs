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

        Transform target;

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

            var nextTile = DungeonManager.Instance.GetGroundTile(mover.TilePos + mover.Dir);

            // ¡‚Ì•ûŒü‚Éi‚ñ‚¾ê‡AŽŸ‚ÌˆÊ’u‚ªFoundation‚È‚ç
            if (nextTile != null && nextTile.tileType == TileType.Foundation && mover.Dir != Vector2Int.zero)
            {
                StartCoroutine(attacker.Attack());
                target = null;
                return;
            }

            if (target == null)
                target = searchVision.Targets.MinBy(t => (t.position - transform.position).sqrMagnitude);

            var targetTilePos = DungeonManager.Instance.WorldToTilePos(target.position);

            mover.UpdateDir(targetTilePos, true);

            if (mover.Move(enemyMoverInfo.Speed, true))
            {
                StartCoroutine(attacker.Attack());
                target = null;
            }
        }
    }
}
