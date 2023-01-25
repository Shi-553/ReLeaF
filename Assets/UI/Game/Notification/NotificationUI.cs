using Animancer;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class NotificationUI : SingletonBase<NotificationUI>
    {
        public enum NotificationType
        {
            GameReady,
            GameStart,
            DestroyAll,
            Blast
        }

        [SerializeField]
        AnimationClip showAnimationClip;

        AnimancerComponent animancer;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                TryGetComponent(out animancer);

                transform.GetChildren().ForEach(t => t.gameObject.SetActive(false));
            }
        }

        Coroutine co;

        GameObject lastTarget;
        public Coroutine Notice(NotificationType type, float duration)
        {
            StopNotice();
            lastTarget = transform.GetChild(type.ToInt32()).gameObject;

            co = StartCoroutine(WaitNotice(duration));
            return co;
        }

        public IEnumerator WaitNotice(float duration)
        {
            lastTarget.SetActive(true);
            animancer.Stop();

            animancer.Play(showAnimationClip);
            yield return new WaitForSeconds(duration);
            lastTarget.SetActive(false);
            co = null;
        }
        void StopNotice()
        {
            if (co != null)
            {
                StopCoroutine(co);
                lastTarget.SetActive(false);
            }
        }
    }
}
