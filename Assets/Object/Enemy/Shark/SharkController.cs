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
                // ��ԋ߂���������
                var minDistanceSq = targetTilePoss.Min(t => (t - mover.TilePos).sqrMagnitude);

                // �^�[�Q�b�g���Ȃ������̃^�[�Q�b�g���3�}�X�ȏ�߂��Ƃ�
                if (targetTilePos == null || minDistanceSq+2 < (targetTilePos.Value - mover.TilePos).sqrMagnitude)
                {

                    // ������������ԋ߂����i�����j
                    var minDistanceElements = targetTilePoss.Where(t => minDistanceSq == (t - mover.TilePos).sqrMagnitude).ToArray();

                    var (element, index) = minDistanceElements
                        .Select(targetPos =>
                        {
                            // �o�H�T������
                            mover.UpdateDir(targetPos, true);
                            return (targetPos, beforeTargetPos: mover.Routing.First());
                        })
                        .MaxBy(tuple => attacker.GetAttackRangeCount(tuple.targetPos, tuple.targetPos - tuple.beforeTargetPos, true));


                    // �^�[�Q�b�g�}�[�J�[�X�V
                    targetMarkerManager.ResetAllMarker();
                    targetMarkerManager.SetMarker<TargetMarker>(element.targetPos);

                    targetTilePos = element.targetPos;
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
                targetTilePos = null;
                StartCoroutine(attacker.Attack());
            }
        }
    }
}
