using Animancer;
using UnityEngine;

namespace ReLeaf
{
    public class CrabAnimation : MonoBehaviour
    {
        [SerializeField]
        CrabAnimationInfo info;

        AnimancerComponent animancerComponent;

        EnemyMover enemyMover;
        IEnemyAttacker enemyAttacker;
        void Start()
        {
            animancerComponent = GetComponentInChildren<AnimancerComponent>();
            TryGetComponent(out enemyAttacker);
            TryGetComponent(out enemyMover);
        }

        void Update()
        {
            switch (enemyAttacker.Transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(CrabAnimationType.BeforeAttack));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(CrabAnimationType.Attack));
                    break;
                case AttackTransition.None:
                    if (enemyMover.IsMove)
                        animancerComponent.Play(info.GetClip(CrabAnimationType.Move, enemyMover.IsLeftIfMove));
                    break;
            }
        }
    }
}
