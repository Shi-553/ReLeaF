using Animancer;
using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public class EnemyAnimationBase : MonoBehaviour
    {
        protected AnimancerComponent animancerComponent;

        protected EnemyMover enemyMover;
        protected IEnemyAttacker enemyAttacker;

        bool isInit = false;
        public virtual void Init()
        {
            if (!isInit)
            {
                isInit = true;
                animancerComponent = GetComponentInChildren<AnimancerComponent>();
                TryGetComponent(out enemyAttacker);
                TryGetComponent(out enemyMover);
            }

        }
        void Start()
        {
            Init();
        }

        public virtual IEnumerator SpawnAnimation(Vector3 current, Vector3 target, float SpwanInitAnimationTime)
        {
            yield break;
        }
    }
}
