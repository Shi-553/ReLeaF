using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReLeaf
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
        public static  Coroutine DestroyNextFrame(GameObject gameObject)
        {
            return StartCoroutine(DestroyNextFrameImpl(gameObject));
        }
        static IEnumerator DestroyNextFrameImpl(GameObject gameObject)
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}
