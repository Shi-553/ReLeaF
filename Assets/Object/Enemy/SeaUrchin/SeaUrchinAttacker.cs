using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{

    public class SeaUrchinAttacker : EnemyAttacker
    {
        [Serializable]
        class SpineInitPosSelect
        {
            public Direction Direction;
            public Transform root;

            List<Vector3> initWorldPositions = new();
            public IReadOnlyList<Vector3> InitWorldPositions => initWorldPositions;

            public void Init()
            {
                foreach (Transform child in root)
                {
                    initWorldPositions.Add(child.position);
                }
            }
        }


        [SerializeField]
        SeaUrchinAttackInfo seaUrchinAttackInfo;
        [SerializeField]
        SpineInitPosSelect[] selects;

        List<Vector2Int> buffer = new();

        public override EnemyAttackInfo EnemyAttackInfo => seaUrchinAttackInfo;


        List<Spine> currentAttackers = new();

        [SerializeField]
        AudioInfo seBeforeAttack;

        [SerializeField]
        AudioInfo seAttack;

        [SerializeField]
        AudioInfo seAfterAttack;


        protected override void Init()
        {
            foreach (var select in selects)
            {
                select.Init();
            }
        }

        public override void Stop()
        {
            base.Stop();

            if (Transition == AttackTransition.Aiming)
            {
                currentAttackers.ForEach(c => Destroy(c));
            }
        }

        public override IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos)
        {
            enemyMover.GetCheckPoss(pos, dir, buffer);
            return buffer;
        }


        protected override void OnStartAiming()
        {
            foreach (var target in GetAttackRange(enemyMover.TilePos, enemyMover.DirNotZero, true))
            {
                attackMarkerManager.SetMarker<TargetMarker>(target, enemyMover.DirNotZero.GetRotation());
            }

            currentAttackers.Clear();
            var poss = selects[enemyMover.DirNotZero.ToDirection().ToInt32()].InitWorldPositions;
            foreach (var pos in poss)
            {
                var spine = Instantiate(seaUrchinAttackInfo.SpinePrefab, pos, Quaternion.identity);
                spine.Init(enemyMover.DirNotZero);
                currentAttackers.Add(spine);
            }
            SEManager.Singleton.Play(seBeforeAttack, transform.position);
        }
        protected override IEnumerator OnStartDamageing()
        {
            attackMarkerManager.ResetAllMarker();

            enemyCore.SetWeekMarker();
            SEManager.Singleton.Play(seAttack, transform.position,0.4f);

            foreach (var spine in currentAttackers)
            {
                spine.ShotStart();
            }

            yield return new WaitForSeconds(seaUrchinAttackInfo.AttackTime);
        }
        protected override void OnStartCoolTime()
        {
            SEManager.Singleton.Play(seAfterAttack, transform.position,0.4f);
        }
        protected override void OnEndCoolTime()
        {
            enemyCore.ResetWeekMarker();
        }
    }
}
