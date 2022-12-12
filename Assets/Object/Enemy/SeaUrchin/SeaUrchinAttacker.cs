using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{

    public class SeaUrchinAttacker : MonoBehaviour, IEnemyAttacker
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

        EnemyCore enemyDamageable;
        EnemyMover mover;
        List<Vector2Int> buffer = new();

        public AttackTransition Transition { get; set; }
        Coroutine IEnemyAttacker.AttackCo { get; set; }

        public EnemyAttackInfo EnemyAttackInfo => seaUrchinAttackInfo;

        [SerializeField]
        MarkerManager targetMarkerManager;

        List<Spine> currentAttackers = new();

        void Start()
        {
            TryGetComponent(out mover);
            TryGetComponent(out enemyDamageable);
            enemyDamageable.OnDeath += OnDeath;

            foreach (var select in selects)
            {
                select.Init();
            }
        }

        private void OnDeath()
        {
            if (Transition == AttackTransition.Aiming)
            {
                currentAttackers.ForEach(c => Destroy(c));
            }
        }

        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos)
        {
            mover.GetCheckPoss(pos, dir, buffer);
            return buffer;
        }


        void IEnemyAttacker.OnStartAiming()
        {
            foreach (var target in GetAttackRange(mover.TilePos, mover.DirNotZero, true))
            {
                targetMarkerManager.SetMarker<TargetMarker>(target, mover.DirNotZero.GetRotation());
            }

            currentAttackers.Clear();
            var poss = selects[mover.DirNotZero.ToDirection().ToInt32()].InitWorldPositions;
            foreach (var pos in poss)
            {
                var spine = Instantiate(seaUrchinAttackInfo.SpinePrefab, pos, Quaternion.identity);
                spine.Init(mover.DirNotZero);
                currentAttackers.Add(spine);
            }
        }
        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            targetMarkerManager.ResetAllMarker();

            enemyDamageable.SetWeekMarker();

            foreach (var spine in currentAttackers)
            {
                spine.ShotStart();
            }

            yield return new WaitForSeconds(seaUrchinAttackInfo.AttackTime);
        }
        void IEnemyAttacker.OnStartCoolTime()
        {
        }
        void IEnemyAttacker.OnEndCoolTime()
        {
            enemyDamageable.ResetWeekMarker();
        }
    }
}
