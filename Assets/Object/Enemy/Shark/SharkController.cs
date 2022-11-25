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

                // ��ԋ߂���������
                var minDistanceSq = float.MaxValue;
                // ������������ԋ߂����i�����j
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

                // �^�[�Q�b�g���Ȃ������̃^�[�Q�b�g���3�}�X�ȏ�߂��Ƃ�
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


                        // �^�[�Q�b�g�}�[�J�[�X�V
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

            // �o�H�T���X�V
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
