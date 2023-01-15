using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class EnemyDirectAttackController : MonoBehaviour
    {

        [SerializeField]
        Vision searchVision;

        EnemyCore core;
        EnemyMover mover;
        EnemyAttacker attacker;

        bool hasTarget = false;
        Vector2Int targetTilePos;

        // 絶対到達できないターゲットたち
        HashSet<Vector2Int> impossibleTargets = new();


        [SerializeField]
        bool isMoveAttacker = false;

        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out mover);
            TryGetComponent(out core);
        }

        float nullTime = 0;

        void Update()
        {
            if (GameRuleManager.Singleton.IsPrepare || core.IsDeath)
                return;
            if (attacker.IsAttack)
            {
                nullTime = 1;
                return;
            }

            bool isUpdateTarget = false;
            if (hasTarget && mover.WasChangedTilePosPrevMove)
            {
                if (!DungeonManager.Singleton.TryGetTile(targetTilePos, out var tile) || !tile.CanEnemyAttack(false))
                {
                    hasTarget = false;
                }
            }

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
                var minDistance = float.MaxValue;
                // 直線距離が一番近いやつ
                List<Vector2Int> minElements = new();
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

                    var distance = (tilePos - mover.GetNearest(tilePos)).magnitude;
                    if (distance == minDistance || (minDistance > 1 && distance - 1 == minDistance))
                    {
                        minElements.Add(tilePos);
                    }
                    else if (distance < minDistance)
                    {
                        if (distance + 1 < minDistance)
                            minElements.Clear();
                        minDistance = distance;
                        minElements.Add(tilePos);
                    }
                }

                if (minDistance != float.MaxValue)
                {
                    var minElement = minElements[Random.Range(0, minElements.Count)];

                    if (minDistance <= 1)
                    {
                        targetTilePos = minElement;
                        Attack();
                        return;
                    }

                    // ターゲットがないか今のターゲットより近いとき
                    if (!hasTarget || minDistance - 1 < (targetTilePos - mover.GetNearest(targetTilePos)).magnitude)
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
                Attack();
            }
            if (result == EnemyMover.MoveResult.Error)
            {
                impossibleTargets.Add(targetTilePos);
                hasTarget = false;
                nullTime = 1;
            }
        }
        List<Vector2Int> buffer = new();
        void Attack()
        {
            impossibleTargets.Clear();
            mover.UpdateTargetStraight(targetTilePos);
            hasTarget = false;

            if (isMoveAttacker)
            {
                mover.GetCheckPoss(mover.TilePos, mover.DirNotZero, buffer);

                var isMovable = buffer.All(b =>
                {
                    if (!DungeonManager.Singleton.TryGetTile(b, out var tile))
                        return false;
                    return tile.CanEnemyMoveAttack(true) && tile.ParentOrThis.CanEnemyMoveAttack(true);
                });

                if (!isMovable)
                {
                    impossibleTargets.Add(targetTilePos);
                    nullTime = 1;
                    return;
                }
            }
            attacker.Attack();
        }
    }

}
