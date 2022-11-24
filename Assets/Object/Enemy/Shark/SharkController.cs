using LinqExtension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


        [SerializeField]
        MarkerManager targetMarkerManager;

        Vector2Int? targetTilePos;

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
                if(attacker.Transition==AttackTransition.Damageing)
                    targetMarkerManager.ResetAllMarker();

                return;
            }


            if (mover.WasChangedTilePosPrevFrame || targetTilePos == null)
            {

                var targetTilePoss = searchVision.Targets.Select(p => DungeonManager.Instance.WorldToTilePos(p.position)).ToArray();
                // 一番近い直線距離
                var minDistanceSq = targetTilePoss.Min(t => (t - mover.TilePos).sqrMagnitude);

                // ターゲットがないか今のターゲットより3マス以上近いとき
                if (targetTilePos == null || minDistanceSq+2 < (targetTilePos.Value - mover.TilePos).sqrMagnitude)
                {

                    // 直線距離が一番近いやつら（複数）
                    var minDistanceElements = targetTilePoss.Where(t => minDistanceSq == (t - mover.TilePos).sqrMagnitude).ToArray();

                    var (element, index) = minDistanceElements
                        .Select(targetPos =>
                        {
                            // 経路探索して
                            mover.UpdateDir(targetPos, true);
                            return (targetPos, beforeTargetPos: mover.Routing.First());
                        })
                        .MaxBy(tuple => attacker.GetAttackRangeCount(tuple.targetPos, tuple.targetPos - tuple.beforeTargetPos, true));


                    // ターゲットマーカー更新
                    targetMarkerManager.ResetAllMarker();
                    targetMarkerManager.SetMarker<TargetMarker>(element.targetPos);

                    targetTilePos = element.targetPos;
                }
            }

            if (targetTilePos == null)
                return;

            // 経路探索更新
            if (mover.WasChangedTilePosPrevFrame)
            {
                mover.UpdateDir(targetTilePos.Value, true);
            }

            if (mover.Move(enemyMoverInfo.Speed, true))
            {
                targetTilePos = null;
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
