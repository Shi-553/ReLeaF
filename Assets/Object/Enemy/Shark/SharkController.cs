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

            var nextTile = DungeonManager.Instance.GetGroundTile(mover.TilePos + mover.Dir);

            // 今の方向に進んだ場合、次の位置がFoundationなら
            if (nextTile != null && nextTile.tileType == TileType.Foundation && mover.Dir != Vector2Int.zero)
            {
                StartCoroutine(attacker.Attack());
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
