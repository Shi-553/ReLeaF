using Animancer;
using UnityEngine;

namespace ReLeaf
{
    public class SharkAnimation : MonoBehaviour
    {
        [SerializeField]
        SharkAnimationInfo info;

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
                    animancerComponent.Play(info.GetClip(SharkAnimationType.BeforeAttack));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.Attack));
                    break;
                case AttackTransition.None:
                    if (enemyMover.IsMove)
                        animancerComponent.Play(info.GetClip(SharkAnimationType.Move, enemyMover.IsLeft));
                    break;
            }
        }
    }
}
