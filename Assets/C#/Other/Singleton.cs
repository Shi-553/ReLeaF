using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.Definition;

namespace Utility
{
    namespace Definition
    {
        public abstract class DefinitionSingletonBase : MonoBehaviour
        {
            protected abstract void Awake();
        }
    }
    public abstract class SingletonBase<T> : DefinitionSingletonBase where T : SingletonBase<T>
    {
        static T singletonInstance;
        public static T Singleton
        {
            get
            {
                if (!isInit)
                {
                    isInit = true;
                    singletonInstance = FindObjectOfType<T>();
                    singletonInstance.Init();
                }
                return singletonInstance;
            }
        }

        static bool isInit = false;
        sealed protected override void Awake()
        {
            if (singletonInstance != null && singletonInstance != this)
            {
                Destroy(this);
                return;
            }

            if (!isInit)
            {
                isInit = true;
                singletonInstance = this as T;
                Init();
            }
        }
        protected abstract void Init();

    }

}
