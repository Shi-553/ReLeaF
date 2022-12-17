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
            transform.position = current;
            yield return null;
            animancerComponent.Play(info.GetClip(SharkAnimationType.Move, current.x > target.x));
            while (true)
            {
                var t = time / SpwanInitAnimationTime;

                transform.localScale = Vector3.Lerp(Vector2.one / 2, Vector2.one, t);
                transform.position = Vector3.Lerp(current, target, t);

                time += Time.deltaTime;
                if (time > SpwanInitAnimationTime)
                    break;

                yield return null;
            }
            transform.position = target;
        }

        void Update()
        {
            switch (enemyAttacker.Transition)
            {
                case AttackTransition.Aiming:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.BeforeAttack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(SharkAnimationType.Attack, enemyMover.IsLeftNow));
                    break;
                case AttackTransition.None:
                    if (enemyMover.IsMove)
                        animancerComponent.Play(info.GetClip(SharkAnimationType.Move, enemyMover.IsLeftIfMove));
                    break;
            }
        }
    }
}
