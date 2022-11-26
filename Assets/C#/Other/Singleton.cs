using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.Impl;

namespace Utility
{
    namespace Impl
    {
        public abstract class SingletonBase : MonoBehaviour
        {
            protected abstract void Awake();
        }
    }
    public abstract class SingletonBase<T> : SingletonBase where T : SingletonBase<T>
    {
        static T singletonInstance;
        public static T Singleton
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = FindObjectOfType<T>();
                    singletonInstance.Awake();
                }
                return singletonInstance;
            }
        }

        bool isInit = false;
        sealed protected override void Awake()
        {
            if (!isInit)
            {
                isInit = true;
                Init();
            }
        }
        protected abstract void Init();

    }

    public static class SingletonSceneInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SingletonSceneInitialize()
        {
            string managerSceneName = "Manager";
            if (SceneManager.GetSceneByName(managerSceneName).IsValid())
            {
                SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning("Manager scene not found.");
            }
        }
    }
}
