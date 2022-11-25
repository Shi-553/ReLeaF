using DebugLogExtension;
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
        Vector2Int? targetTileBeferePos;

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
                if (attacker.Transition == AttackTransition.Damageing)
                    targetMarkerManager.ResetAllMarker();

                return;
            }


            if (mover.WasChangedTilePosPrevFrame || targetTilePos == null)
            {

                // 一番近い直線距離
                var minDistanceSq = float.MaxValue;
                // 直線距離が一番近いやつら（複数）
                var minElements = new List<Vector2Int>();
                foreach (var target in searchVision.Targets())
                {
                    var tilePos = DungeonManager.Instance.WorldToTilePos(target.position);
                    var distanceSq = (tilePos - mover.TilePos).sqrMagnitude;
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        minElements.Clear();
                        minElements.Add(tilePos);
                    }
                    if (distanceSq == minDistanceSq)
                        minElements.Add(tilePos);
                }

                // ターゲットがないか今のターゲットより3マス以上近いとき
                if (targetTilePos == null || minDistanceSq + 2 < (targetTileBeferePos.Value - mover.TilePos).sqrMagnitude)
                {
                    var tuples = minElements
                        .Select(targetPos =>
                        {
                            var before = targetPos + (mover.TilePos - targetPos).ClampOneMagnitude();
                            if (DungeonManager.Instance.TryGetTile(before, out var tile) && tile.CanEnemyMove)
                            {
                                return (targetPos, beforeTargetPos: before);
                            }
                            mover.UpdateDir(targetPos, true);
                            if (mover.Routing.Count == 0)
                            {
                                return (targetPos, beforeTargetPos: targetPos);
                            }
                            return (targetPos, beforeTargetPos: targetPos - mover.Routing.First());
                        })
                        .Where(t => t.targetPos != t.beforeTargetPos)
                        .ToArray();

                    if (tuples.Length != 0)
                    {

                        var (element, index) = tuples.MaxBy(tuple => attacker.GetAttackRangeCount(tuple.targetPos, tuple.targetPos - tuple.beforeTargetPos, true));


                        // ターゲットマーカー更新
                        targetMarkerManager.ResetAllMarker();
                        targetMarkerManager.SetMarker<TargetMarker>(element.targetPos);

                        targetTileBeferePos = element.beforeTargetPos;
                        targetTilePos = element.targetPos;
                    }
                    else
                    {
                        Debug.Log("0");
                    }
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
                mover.UpdateDir(targetTilePos.Value, false);
                targetTilePos = null;
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
