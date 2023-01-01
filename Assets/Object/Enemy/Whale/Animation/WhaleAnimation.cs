using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public class WhaleAnimation : EnemyAnimationBase
    {
        [SerializeField]
        WhaleAnimationInfo info;

        public override IEnumerator DeathAnimation()
        {
            return animancerComponent.Play(info.GetClip(WhaleAnimationType.Death, enemyMover.IsLeftNow));
        }
        protected override void ChangeTransitionImpl(AttackTransition transition)
        {
            switch (transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(WhaleAnimationType.BeforeAttack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(WhaleAnimationType.Attack, enemyMover.IsLeftNow));
                    break;
            }
        }
        protected override void OnMoveImpl()
        {
            if (!enemyAttacker.IsAttack)
                animancerComponent.Play(info.GetClip(WhaleAnimationType.Move, enemyMover.IsLeftIfMove));
        }
    }
}
