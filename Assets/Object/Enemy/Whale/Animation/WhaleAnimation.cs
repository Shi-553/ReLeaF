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
        protected override void ChangeTransition(AttackTransition transition)
        {
            if (enemyCore.IsDeath)
            {
                return;
            }
            switch (transition)
            {
                case AttackTransition.None:
                    animancerComponent.Play(info.GetClip(WhaleAnimationType.Move, enemyMover.IsLeftIfMove));
                    break;
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(WhaleAnimationType.BeforeAttack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(WhaleAnimationType.Attack, enemyMover.IsLeftNow));
                    break;
            }
        }
    }
}
