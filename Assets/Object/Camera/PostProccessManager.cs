using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utility;

namespace ReLeaf
{
    public class PostProccessManager : SingletonBase<PostProccessManager>
    {
        ColorAdjustments colorAdjustments;
        [SerializeField]
        float changeDuration = 0.5f;
        [SerializeField]
        float darkModeColor = 0.4f;
        float lightModeColor = 0.9649041f;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                GetComponent<Volume>()
                    .sharedProfile
                    .TryGet(out colorAdjustments);

                lightModeColor = colorAdjustments.colorFilter.value.r;

            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            colorAdjustments.colorFilter.value = new(lightModeColor, lightModeColor, lightModeColor, colorAdjustments.colorFilter.value.a);
        }
        public void ToDarkMode()
        {
            if (co != null)
                StopCoroutine(co);
            co = StartCoroutine(Change(darkModeColor));
        }
        public void ToLightMode()
        {
            if (co != null)
                StopCoroutine(co);
            co = StartCoroutine(Change(lightModeColor));
        }

        Coroutine co;
        IEnumerator Change(float targetColor)
        {
            var before = colorAdjustments.colorFilter.value;

            float time = 0;
            while (true)
            {
                var t = Mathf.Min(1, time / changeDuration);
                var current = Mathf.Lerp(before.r, targetColor, t);

                colorAdjustments.colorFilter.value = new(current, current, current, before.a);
                yield return null;
                time += Time.deltaTime;
                if (time > changeDuration)
                {
                    break;
                }
            }
            co = null;
        }
    }
}
