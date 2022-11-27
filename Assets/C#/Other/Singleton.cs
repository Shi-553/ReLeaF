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
            public abstract void Destroy();
        }
    }
    public abstract class SingletonBase<T> : DefinitionSingletonBase where T : SingletonBase<T>
    {
        static bool isInit = false;
        static protected bool isDontDestroy = false;

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

            if (isDontDestroy)
            {
#if DEFINE_SCENE_TYPE_ENUM
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(SceneType.Manager.GetBuildIndex()));
#endif
            }
        }

        public sealed override void Destroy()
        {
            isInit = false;
            singletonInstance = null;
            Uninit();
        }

        /// <summary>
        /// Awakeかさらに早く呼ばれる
        /// </summary>
        protected abstract void Init();
        protected virtual void Uninit() { }

    }

}
