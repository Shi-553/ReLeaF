using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public class SharkAnimation : EnemyAnimationBase
    {
        [SerializeField]
        SharkAnimationInfo info;

        public override IEnumerator SpawnAnimation(Vector3 current, Vector3 target, float SpwanInitAnimationTime)
        {
            float time = 0;
            animancerComponent.Play(info.GetClip(SharkAnimationType.Move, current.x > target.x));
            while (true)
            {
                if (this == null)
                    yield break;

                var t = time / SpwanInitAnimationTime;

                transform.localScale = Vector3.Lerp(Vector3.one / 2, Vector3.one, t);
                transform.position = Vector3.Lerp(current, target, t);

                time += Time.deltaTime;
                if (time > SpwanInitAnimationTime)
                    break;

                yield return null;
            }
            transform.localScale = Vector3.one;
            transform.position = target;
        }

        public override IEnumerator DeathAnimation()
        {
            yield return animancerComponent.Play(info.GetClip(SharkAnimationType.Death, enemyMover.IsLeftNow));
        }

        protected override void ChangeTransition(AttackTransition transition)
        {

            if (enemyCore.IsDeath)
            {
                return;
            }

            switch (transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.BeforeAttack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.Attack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.None:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.Move, enemyMover.IsLeftIfMove));
                    break;
            }
        }
    }
}
