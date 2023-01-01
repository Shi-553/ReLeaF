using Animancer;
using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public abstract class EnemyAnimationBase : MonoBehaviour
    {
        protected AnimancerComponent animancerComponent;

        protected EnemyMover enemyMover;
        protected EnemyAttacker enemyAttacker;
        protected EnemyCore enemyCore;

        bool isInit = false;
        public virtual void Init()
        {
            if (!isInit)
            {
                isInit = true;
                animancerComponent = GetComponentInChildren<AnimancerComponent>();
                TryGetComponent(out enemyAttacker);
                TryGetComponent(out enemyMover);
                TryGetComponent(out enemyCore);
                enemyAttacker.OnChangeTransition += ChangeTransition;
                enemyMover.OnMove += OnMove;
            }
        }
        void ChangeTransition(AttackTransition transition)
        {
            if (!enemyCore.IsDeath)
                ChangeTransitionImpl(transition);
        }
        protected abstract void ChangeTransitionImpl(AttackTransition transition);

        void OnMove()
        {
            if (!enemyCore.IsDeath)
                OnMoveImpl();
        }
        protected abstract void OnMoveImpl();

        void Start()
        {
            Init();
        }

        public virtual IEnumerator SpawnAnimation(Vector3 current, Vector3 target, float SpwanInitAnimationTime)
        {
            yield break;
        }

        public virtual IEnumerator DeathAnimation()
        {
            yield break;
        }
    }
}
