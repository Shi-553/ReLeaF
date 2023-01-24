using Animancer;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DestroyAllUI : SingletonBase<DestroyAllUI>
    {
        [SerializeField]
        AnimationClip showAnimationClip;

        AnimancerComponent animancer;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                TryGetComponent(out animancer);
                gameObject.SetActive(false);
            }
        }
        Coroutine co;
        public void ShowDestroyAll()
        {
            if (co != null)
                StopCoroutine(co);

            gameObject.SetActive(true);
            co = StartCoroutine(WaitDestroyAll());
        }

        public IEnumerator WaitDestroyAll()
        {
            animancer.Stop();

            yield return animancer.Play(showAnimationClip);
            gameObject.SetActive(false);
            co = null;
        }
    }
}
