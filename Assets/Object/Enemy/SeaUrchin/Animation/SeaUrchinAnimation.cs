using UnityEngine;

namespace ReLeaf
{
    public class SeaUrchinAnimation : EnemyAnimationBase
    {
        [SerializeField]
        SeaUrhinAnimationInfo info;

        void Update()
        {
            if (enemyCore.IsDeath)
            {
                animancerComponent.Play(info.GetClip(SeaUrhinAnimationType.Death));
                return;
            }
            switch (enemyAttacker.Transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(SeaUrhinAnimationType.BeforeAttack));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(SeaUrhinAnimationType.Attack));
                    break;
                case AttackTransition.CoolTime:
                    animancerComponent.Play(info.GetClip(SeaUrhinAnimationType.AfterAttack));
                    break;
            }
        }
    }
}
