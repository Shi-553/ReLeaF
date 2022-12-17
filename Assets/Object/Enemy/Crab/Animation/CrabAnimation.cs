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
                var t = time / SpwanInitAnimationTime;

                transform.localScale = Vector3.Lerp(Vector2.one / 2, Vector2.one, t);
                transform.position = new Vector3(Mathf.Lerp(current.x, target.x, t), MathExtension.LerpPairs(pairs, t * (2 - t)), Mathf.Lerp(current.z, target.z, t));

                time += Time.deltaTime;
                if (time > SpwanInitAnimationTime)
                    break;

                yield return null;
            }
            transform.position = target;
        }

        void Update()
        {
            if (enemyCore.IsDeath)
            {
                animancerComponent.Play(info.GetClip(CrabAnimationType.Death));
                return;
            }

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
