using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    // https://shamaton.orz.hm/blog/archives/448
    public class GlobalCoroutine : MonoBehaviour
    {

        // singleton
        private static GlobalCoroutine instance;
        private static GlobalCoroutine Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GlobalCoroutine";
                    instance = obj.AddComponent<GlobalCoroutine>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }
        public static new Coroutine StartCoroutine(IEnumerator routine)
        {
            return Instance.StaticCast<MonoBehaviour>().StartCoroutine(routine);
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
