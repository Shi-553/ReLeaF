using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class EnemyDirectAttackController : MonoBehaviour
    {

        [SerializeField]
        Vision searchVision;

        EnemyMover mover;
        EnemyAttacker attacker;

        bool hasTarget = false;
        Vector2Int targetTilePos;
        Vector2Int lastTargetTilePos;

        // 絶対到達できないターゲットたち
        HashSet<Vector2Int> impossibleTargets = new();

        public void AddImpossibleLastTargets()
        {
            impossibleTargets.Add(lastTargetTilePos);
        }

        [SerializeField]
        bool isMoveAttacker = false;

        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out mover);
        }

        float nullTime = 0;

        void Update()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;
            if (attacker.IsAttack)
            {
                nullTime = 1;
                return;
            }

            bool isUpdateTarget = false;
            if (mover.WasChangedTilePosPrevMove || !hasTarget)
            {
                nullTime += Time.deltaTime;
                if (!hasTarget && nullTime < 1)
                {
                    return;
                }
                nullTime = 0;

                if (!searchVision.UpdateTarget())
                {
                    return;
                }

                // 一番近い直線距離
                var minDistanceSq = float.MaxValue;
                // 直線距離が一番近いやつ
                Vector2Int minElement = Vector2Int.zero;
                foreach (var target in searchVision.Targets())
                {
                    var tilePos = DungeonManager.Singleton.WorldToTilePos(target.position);
                    if (impossibleTargets.Contains(tilePos))
                        continue;

                    if (isMoveAttacker)
                    {
                        if (!DungeonManager.Singleton.TryGetTile(tilePos, out var tile) || !tile.CanEnemyMoveAttack(true) || !tile.ParentOrThis.CanEnemyMoveAttack(true))
                        {
                            impossibleTargets.Add(tilePos);
                            continue;
                        }
                    }

                    var distanceSq = (tilePos - mover.GetNearest(tilePos)).sqrMagnitude;
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        minElement = tilePos;
                    }
                }

                // ターゲットがないか今のターゲットより近いとき
                if (!hasTarget || minDistanceSq < (targetTilePos - mover.GetNearest(targetTilePos)).sqrMagnitude)
                {
                    if (minDistanceSq != float.MaxValue)
                    {
                        isUpdateTarget = true;
                        targetTilePos = minElement;
                        hasTarget = true;
                    }
                }
            }

            if (!hasTarget)
                return;

            // 経路探索更新
            if (isUpdateTarget)
            {
                if (!mover.UpdateTargetAutoRouting(targetTilePos))
                {
                    // 到達不可能なターゲット
                    impossibleTargets.Add(targetTilePos);
                    hasTarget = false;
                    nullTime = 1;
                    return;
                }
            }

            var result = mover.Move();
            if (result == EnemyMover.MoveResult.Finish)
            {
                lastTargetTilePos = targetTilePos;
                impossibleTargets.Clear();
                mover.UpdateTargetStraight(targetTilePos);
                hasTarget = false;
                attacker.Attack();
            }
            if (result == EnemyMover.MoveResult.Error)
            {
                impossibleTargets.Add(targetTilePos);
                hasTarget = false;
                nullTime = 1;
            }

        }
    }
}
