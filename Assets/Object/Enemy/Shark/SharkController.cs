using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SharkController : MonoBehaviour
    {

        [SerializeField]
        Vision searchVision;

        EnemyMover mover;
        IEnemyAttacker attacker;


        [SerializeField]
        MarkerManager targetMarkerManager;

        Vector2Int? targetTilePos;

        // 絶対到達できないターゲットたち
        HashSet<Vector2Int> impossibleTargets = new();
        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out mover);
        }


        void Update()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;
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

            bool isUpdateTarget = false;
            if (mover.WasChangedTilePosPrevMove || targetTilePos == null)
            {

                // 一番近い直線距離
                var minDistanceSq = float.MaxValue;
                // 直線距離が一番近いやつ
                Vector2Int minElement = Vector2Int.zero;
                foreach (var target in searchVision.Targets())
                {
                    var tilePos = DungeonManager.Singleton.WorldToTilePos(target.position);
                    if (impossibleTargets.Contains(tilePos))
                        continue;

                    var distanceSq = (tilePos - mover.GetNearest(tilePos)).sqrMagnitude;
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        minElement = tilePos;
                    }
                }

                // ターゲットがないか今のターゲットより近いとき
                if (targetTilePos == null || minDistanceSq < (targetTilePos.Value - mover.GetNearest(targetTilePos.Value)).sqrMagnitude)
                {
                    if (minDistanceSq != float.MaxValue)
                    {
                        isUpdateTarget = true;
                        targetTilePos = minElement;
                    }
                }
            }

            if (targetTilePos == null)
                return;

            // 経路探索更新
            if (isUpdateTarget)
            {
                if (!mover.UpdateTargetAutoRouting(targetTilePos.Value))
                {
                    // 到達不可能なターゲット
                    impossibleTargets.Add(targetTilePos.Value);
                    targetTilePos = null;
                    return;
                }
                // ターゲットマーカー更新
                targetMarkerManager.ResetAllMarker();
                foreach (var target in mover.Targets)
                {
                    targetMarkerManager.SetMarker<TargetMarker>(target, mover.ToTargetDir.GetRotation());
                }
            }

            var result = mover.Move();
            if (result == EnemyMover.MoveResult.Finish)
            {
                mover.UpdateTargetStraight(targetTilePos.Value);
                targetTilePos = null;
                StartCoroutine(attacker.Attack());
            }
            if (result == EnemyMover.MoveResult.Error)
            {
                targetTilePos = null;
            }
        }
    }
}
