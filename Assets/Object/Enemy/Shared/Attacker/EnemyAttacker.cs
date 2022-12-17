using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum AttackTransition
    {
        None,
        Aiming,
        Damageing,
        CoolTime
    }
    public abstract class EnemyAttacker : MonoBehaviour
    {
        [field: SerializeField, ReadOnly]
        public AttackTransition Transition { get; private set; }
        public event Action<AttackTransition> OnChangeTransition;


        protected EnemyMover enemyMover;
        protected EnemyCore enemyCore;

        public bool IsAttackDamageing => Transition == AttackTransition.Damageing;
        public bool IsAttack => Transition != AttackTransition.None;

        public abstract EnemyAttackInfo EnemyAttackInfo { get; }


        [SerializeField]
        protected MarkerManager attackMarkerManager;


        public abstract IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool includeMoveabePos);

        protected virtual void Init() { }

        private void Awake()
        {
            TryGetComponent(out enemyMover);
            TryGetComponent(out enemyCore);
            Init();
        }

        protected virtual void OnStartAiming()
        {
        }
        protected abstract IEnumerator OnStartDamageing();
        protected virtual void OnStartCoolTime()
        {
        }
        protected virtual void OnEndCoolTime()
        {
        }

        protected Coroutine AttackCo { get; set; }
        public void Attack()
        {
            AttackCo = StartCoroutine(AttackImpl());
        }
        public virtual void Stop()
        {
            if (AttackCo != null)
                StopCoroutine(AttackCo);
            attackMarkerManager.ResetAllMarker();
        }

        protected IEnumerator AttackImpl()
        {
            Transition = AttackTransition.Aiming;
            OnStartAiming();
            OnChangeTransition?.Invoke(Transition);

            yield return new WaitForSeconds(EnemyAttackInfo.AimTime);
            if (this == null)
                yield break;

            Transition = AttackTransition.Damageing;
            OnChangeTransition?.Invoke(Transition);
            yield return GlobalCoroutine.Singleton.StartCoroutine(OnStartDamageing());
            if (this == null)
                yield break;

            Transition = AttackTransition.CoolTime;
            OnChangeTransition?.Invoke(Transition);
            yield return new WaitUntil(() => Transition == AttackTransition.CoolTime);
            if (this == null)
                yield break;

            OnStartCoolTime();

            yield return new WaitForSeconds(EnemyAttackInfo.CoolTime);
            if (this == null)
                yield break;

            Transition = AttackTransition.None;
            OnChangeTransition?.Invoke(Transition);
            OnEndCoolTime();
            AttackCo = null;
        }
    }
}