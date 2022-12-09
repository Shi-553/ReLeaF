using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    // https://shamaton.orz.hm/blog/archives/448
    public class GlobalCoroutine : SingletonBase<GlobalCoroutine>
    {
        public override bool DontDestroyOnLoad => true;
        protected override void Init()
        {
        }
        public static new void StopCoroutine(Coroutine routine)
        {
            Singleton.StaticCast<MonoBehaviour>().StopCoroutine(routine);
        }
        public static new Coroutine StartCoroutine(IEnumerator routine)
        {
            return Singleton.StaticCast<MonoBehaviour>().StartCoroutine(routine);
        }
        public static Coroutine DestroyNextFrame(GameObject gameObject)
        {
            return StartCoroutine(DestroyNextFrameImpl(gameObject));
        }
        static IEnumerator DestroyNextFrameImpl(GameObject gameObject)
        {
            yield return null;
            Destroy(gameObject);
        }

        public static Coroutine WaitNextFrame(Action action)
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
