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

        Vector2Int? targetTilePos;

        // ��Γ��B�ł��Ȃ��^�[�Q�b�g����
        HashSet<Vector2Int> impossibleTargets = new();

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
            if (mover.WasChangedTilePosPrevMove || targetTilePos == null)
            {
                nullTime += Time.deltaTime;
                if (targetTilePos == null && nullTime < 1)
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

                    var distanceSq = (tilePos - mover.GetNearest(tilePos)).sqrMagnitude;
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        minElement = tilePos;
                    }
                }

                // �^�[�Q�b�g���Ȃ������̃^�[�Q�b�g���߂��Ƃ�
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

            // �o�H�T���X�V
            if (isUpdateTarget)
            {
                if (!mover.UpdateTargetAutoRouting(targetTilePos.Value))
                {
                    // ���B�s�\�ȃ^�[�Q�b�g
                    impossibleTargets.Add(targetTilePos.Value);
                    targetTilePos = null;
                    return;
                }
            }

            var result = mover.Move();
            if (result == EnemyMover.MoveResult.Finish)
            {
                mover.UpdateTargetStraight(targetTilePos.Value);
                targetTilePos = null;
                attacker.Attack();
            }
            if (result == EnemyMover.MoveResult.Error)
            {
                targetTilePos = null;
            }

        }
    }
}
