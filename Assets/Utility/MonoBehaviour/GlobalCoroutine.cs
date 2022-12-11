using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    // https://shamaton.orz.hm/blog/archives/448
    public class GlobalCoroutine : SingletonBase<GlobalCoroutine>
    {
        public override bool DontDestroyOnLoad => true;
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
        public new void StopCoroutine(Coroutine routine)
        {
            this.StaticCast<MonoBehaviour>().StopCoroutine(routine);
        }
        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return this.StaticCast<MonoBehaviour>().StartCoroutine(routine);
        }
        public Coroutine DestroyNextFrame(GameObject gameObject)
        {
            return StartCoroutine(DestroyNextFrameImpl(gameObject));
        }
        static IEnumerator DestroyNextFrameImpl(GameObject gameObject)
        {
            yield return null;
            Destroy(gameObject);
        }

        public Coroutine WaitNextFrame(Action action)
        {
            return StartCoroutine(WaitNextFrameImpl(action));
        }
        static IEnumerator WaitNextFrameImpl(Action action)
        {
            yield return null;
            action();
        }

    }
}
