using System.Collections.Generic;
using UnityEngine;

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

        // ��Γ��B�ł��Ȃ��^�[�Q�b�g����
        HashSet<Vector2Int> impossibleTargets = new();
        void Start()
        {
            TryGetComponent(out attacker);
            TryGetComponent(out mover);
        }

        int GetNearest(int target, int min, int max)
        {
            if (min <= target && target <= max)
                return target;
            else if (target < min)
                return min;
            else
                return max;
        }
        Vector2Int GetNearest(Vector2Int target)
        {
            var tilePos = mover.TilePos;
            var tileSize = mover.TileSize;

            return new Vector2Int(GetNearest(target.x, tilePos.x, tilePos.x + tileSize.x),
                GetNearest(target.y, tilePos.y, tilePos.y + tileSize.y));
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


            if (mover.WasChangedTilePosPrevFrame || targetTilePos == null)
            {

                // ��ԋ߂���������
                var minDistanceSq = float.MaxValue;
                // ������������ԋ߂����
                Vector2Int minElement = Vector2Int.zero;
                foreach (var target in searchVision.Targets())
                {
                    var tilePos = DungeonManager.Singleton.WorldToTilePos(target.position);
                    if (impossibleTargets.Contains(tilePos))
                        continue;

                    var distanceSq = (tilePos - GetNearest(tilePos)).sqrMagnitude;
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        minElement = tilePos;
                    }
                }

                // �^�[�Q�b�g���Ȃ������̃^�[�Q�b�g���߂��Ƃ�
                if (targetTilePos == null || minDistanceSq + 1 < (targetTilePos.Value - GetNearest(targetTilePos.Value)).sqrMagnitude)
                {

                    if (minDistanceSq != float.MaxValue)
                    {
                        // �^�[�Q�b�g�}�[�J�[�X�V
                        targetMarkerManager.ResetAllMarker();
                        targetMarkerManager.SetMarker<TargetMarker>(minElement);

                        targetTilePos = minElement;
                    }
                }
            }

            if (targetTilePos == null)
                return;

            // �o�H�T���X�V
            if (mover.WasChangedTilePosPrevFrame)
            {
                if (!mover.UpdateDir(targetTilePos.Value))
                {
                    impossibleTargets.Add(targetTilePos.Value);
                    targetTilePos = null;
                    return;
                }
            }

            if (mover.Move(true))
            {
                mover.UpdateDir(targetTilePos.Value);
                targetTilePos = null;
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
