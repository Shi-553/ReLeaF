using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public class SeaUrchinAnimation : EnemyAnimationBase
    {
        [SerializeField]
        SeaUrhinAnimationInfo info;


        public override IEnumerator DeathAnimation()
        {
            return animancerComponent.Play(info.GetClip(SeaUrhinAnimationType.Death));
        }

        protected override void ChangeTransitionImpl(AttackTransition transition)
        {
            switch (transition)
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
        protected override void OnMoveImpl()
        {
        }
    }
}
