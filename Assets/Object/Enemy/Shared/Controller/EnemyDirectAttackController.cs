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

        // ��Γ��B�ł��Ȃ��^�[�Q�b�g����
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

                // ��ԋ߂���������
                var minDistanceSq = float.MaxValue;
                // ������������ԋ߂����
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

                // �^�[�Q�b�g���Ȃ������̃^�[�Q�b�g���߂��Ƃ�
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

            // �o�H�T���X�V
            if (isUpdateTarget)
            {
                if (!mover.UpdateTargetAutoRouting(targetTilePos))
                {
                    // ���B�s�\�ȃ^�[�Q�b�g
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
