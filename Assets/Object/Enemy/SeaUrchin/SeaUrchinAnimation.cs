using Animancer;
using UnityEngine;

namespace ReLeaf
{
    public class SeaUrchinAnimation : MonoBehaviour
    {
        [SerializeField]
        SeaUrhinAnimationInfo info;

        AnimancerComponent animancerComponent;

        IEnemyAttacker enemyAttacker;
        void Start()
        {
            animancerComponent = GetComponentInChildren<AnimancerComponent>();
            TryGetComponent(out enemyAttacker);
        }

        void Update()
        {
            switch (enemyAttacker.Transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.BeforeAttack);
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.Attack);
                    break;
                case AttackTransition.CoolTime:
                    animancerComponent.Play(info.AfterAttack);
                    break;
            }
        }
    }
}
