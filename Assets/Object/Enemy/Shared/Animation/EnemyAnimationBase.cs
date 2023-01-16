using Animancer;
using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public abstract class EnemyAnimationBase : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        protected AnimancerComponent animancerComponent;

        protected EnemyMover enemyMover;
        protected EnemyAttacker enemyAttacker;
        protected EnemyCore enemyCore;

        bool isInit = false;
        public virtual void Init()
        {
            if (!isInit)
            {
                isInit = true;
                animancerComponent = GetComponentInChildren<AnimancerComponent>();
                TryGetComponent(out enemyAttacker);
                TryGetComponent(out enemyMover);
                TryGetComponent(out enemyCore);
                enemyAttacker.OnChangeTransition += ChangeTransition;
                enemyMover.OnMove += OnMove;

                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                spriteOriginalLocalPos = spriteRenderer.transform.localPosition;
            }
        }
        void ChangeTransition(AttackTransition transition)
        {
            if (enemyCore.IsValid)
                ChangeTransitionImpl(transition);
        }
        protected abstract void ChangeTransitionImpl(AttackTransition transition);

        void OnMove()
        {
            if (enemyCore.IsValid)
                OnMoveImpl();
        }
        protected abstract void OnMoveImpl();

        void Start()
        {
            Init();
        }

        public virtual IEnumerator SpawnAnimation(Vector3 current, Vector3 target, float SpwanInitAnimationTime)
        {
            yield break;
        }

        public virtual IEnumerator DeathAnimation()
        {
            yield break;
        }

        float stanMotionOffsetX = 0.03f;
        float stanOneMotionDuration = 0.07f;
        Coroutine motionCo;
        public void StanAnimation()
        {
            spriteRenderer.color = Color.white;
            if (motionCo != null)
            {
                StopCoroutine(motionCo);
            }
            motionCo = StartCoroutine(StanAnimationImpl());
        }
        IEnumerator StanAnimationImpl()
        {
            OnMoveImpl();

            var spriteTransform = spriteRenderer.transform;
            var targetFirst = spriteOriginalLocalPos + Vector3.right * stanMotionOffsetX;
            var targetSecond = spriteOriginalLocalPos - Vector3.right * stanMotionOffsetX;

            if (spriteTransform.localPosition.x < spriteOriginalLocalPos.x)
                (targetFirst, targetSecond) = (targetSecond, targetFirst);

            float time = 0;
            while (true)
            {
                int motion = (((int)(time / stanOneMotionDuration)) % 2);
                if (!enemyCore.IsStan)
                    motion = 2;

                var target = motion switch
                {
                    0 => targetFirst,
                    1 => targetSecond,
                    _ => spriteOriginalLocalPos
                };

                spriteTransform.localPosition = Vector3.Lerp(spriteTransform.localPosition, target, 0.5f);

                if (!enemyCore.IsStan && (spriteTransform.localPosition - target).sqrMagnitude < 0.1f)
                {
                    motionCo = null;
                    yield break;
                }

                yield return null;
                time += Time.deltaTime;
            }
        }

        float damagedTime = 0;
        public void DamagedMotion()
        {
            var currentTime = Time.time;

            if (currentTime - damagedTime > 0.1f)
            {
                if (motionCo != null)
                {
                    StopCoroutine(motionCo);
                }

                motionCo = StartCoroutine(DamagedMotionWait());
            }

            damagedTime = currentTime;
        }
        Vector3 spriteOriginalLocalPos;
        Color damagedColor = new(1, 0.5f, 0.5f);
        float damagedOneMotionDuration = 0.1f;
        float damagedMotionOffsetX = 0.1f;
        IEnumerator DamagedMotionWait()
        {
            var spriteTransform = spriteRenderer.transform;
            var targetFirst = spriteOriginalLocalPos + Vector3.right * damagedMotionOffsetX;
            var targetSecond = spriteOriginalLocalPos - Vector3.right * damagedMotionOffsetX;

            if (spriteTransform.localPosition.x < spriteOriginalLocalPos.x)
                (targetFirst, targetSecond) = (targetSecond, targetFirst);

            float time = 0;

            var color = Color.white;
            var wasFlashing = color != spriteRenderer.color;

            while (true)
            {
                Vector3 target;

                if (time < damagedOneMotionDuration)
                    target = targetFirst;
                else if (time < damagedOneMotionDuration * 2)
                    target = targetSecond;
                else if (time < damagedOneMotionDuration * 3)
                    target = spriteOriginalLocalPos;
                else
                    break;

                spriteTransform.localPosition = Vector3.Lerp(spriteTransform.localPosition, target, 0.5f);


                bool isFlashing = (((int)(time / damagedOneMotionDuration)) % 2) == 0;
                if (wasFlashing)
                    isFlashing = !isFlashing;

                var currentColor = isFlashing ? damagedColor : color;

                if (currentColor != spriteRenderer.color)
                    spriteRenderer.color = currentColor;

                yield return null;
                time += Time.deltaTime;
            }
            spriteTransform.localPosition = spriteOriginalLocalPos;
            spriteRenderer.color = color;
            motionCo = null;
        }
    }
}
