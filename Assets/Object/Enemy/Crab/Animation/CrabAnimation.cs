using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class CrabAnimation : EnemyAnimationBase
    {
        [SerializeField]
        CrabAnimationInfo info;

        public override IEnumerator SpawnAnimation(Vector3 current, Vector3 target, float SpwanInitAnimationTime)
        {
            float time = 0;
            animancerComponent.Play(info.GetClip(CrabAnimationType.Move, current.x > target.x));

            SortedList<float, float> pairs = new(){
                {0,current.y },
                {0.5f,current.y+0.5f },
                {1,target.y },
            };
            while (true)
            {
                if (this == null)
                    yield break;

                var t = time / SpwanInitAnimationTime;

                transform.localScale = Vector3.Lerp(Vector3.one / 2, Vector3.one, t);
                transform.position = new Vector3(Mathf.Lerp(current.x, target.x, t), MathExtension.LerpPairs(pairs, t * (2 - t)), Mathf.Lerp(current.z, target.z, t));

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
            yield return animancerComponent.Play(info.GetClip(CrabAnimationType.Death));
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
                    animancerComponent.Play(info.GetClip(CrabAnimationType.BeforeAttack));
                    break;
                case AttackTransition.Damageing:
                    animancerComponent.Play(info.GetClip(CrabAnimationType.Attack));
                    break;
            }
        }

        protected override void OnMove()
        {
            if (!enemyAttacker.IsAttack)
                animancerComponent.Play(info.GetClip(CrabAnimationType.Move, enemyMover.IsLeftIfMove));
        }
    }
}
