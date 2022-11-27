using UnityEngine;
using Utility.Definition;

namespace Utility
{
    namespace Definition
    {
        public abstract class DefinitionSingletonBase : MonoBehaviour
        {
            protected abstract void Awake();
            protected abstract void OnDestroy();
        }
    }
    public abstract class SingletonBase<T> : DefinitionSingletonBase where T : SingletonBase<T>
    {
        static bool isInit = false;

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

        sealed protected override void Awake()
        {
            if (isInit && singletonInstance != this)
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

        protected sealed override void OnDestroy()
        {
            singletonInstance = null;
            isInit = false;
            Uninit();
        }

        /// <summary>
        /// Awakeかさらに早く呼ばれる
        /// </summary>
        protected abstract void Init();
        protected virtual void Uninit() { }

    }

}
