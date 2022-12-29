using DebugLogExtension;
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
            public abstract void UninitBeforeSceneUnloadDefinition();
            public abstract void UninitAfterSceneUnloadDefinition();
            public abstract void TryDestroy();
        }
    }
    public abstract class SingletonBase<T> : DefinitionSingletonBase where T : SingletonBase<T>
    {
        static bool isInitialized = false;

        public new abstract bool DontDestroyOnLoad { get; }

        static T singletonInstance;
        public static T Singleton
        {
            get
            {
                if (!isInitialized)
                {
                    if (singletonInstance != null)
                    {
                        isInitialized = true;
                    }
                    else
                    {
                        singletonInstance = FindObjectOfType<T>();

                        if (singletonInstance != null)
                        {
                            singletonInstance.Init(true, false);
                            isInitialized = true;
                        }
                    }
                }
                return singletonInstance;
            }
        }


        sealed protected override void Awake()
        {
            if (isInitialized && singletonInstance != null && singletonInstance != this)
            {
                "Destroy Duplicate Instance.".DebugLog();

                Destroy(this);
                return;
            }

            Init(!isInitialized, true);
            if (!isInitialized)
            {
                isInitialized = true;
                singletonInstance = this as T;
            }
        }


        bool InManagerScene => gameObject.scene.buildIndex == SceneType.Manager.GetBuildIndex();

        public sealed override void UninitBeforeSceneUnloadDefinition()
        {
            UninitBeforeSceneUnload(!DontDestroyOnLoad);

#if DEFINE_SCENE_TYPE_ENUM
            if (!InManagerScene)
            {
                transform.SetParent(null, false);
                SceneManager.MoveGameObjectToScene(transform.gameObject, SceneManager.GetSceneByBuildIndex(SceneType.Manager.GetBuildIndex()));
            }
#endif
        }

        public sealed override void UninitAfterSceneUnloadDefinition()
        {
            UninitAfterSceneUnload(!DontDestroyOnLoad);
        }
        public sealed override void TryDestroy()
        {
            if (!DontDestroyOnLoad)
            {
                isInitialized = false;
                singletonInstance = null;

                if (gameObject != null)
                    Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            isInitialized = false;
        }

        /// <summary>
        /// Awakeかさらに早く呼ばれる
        /// </summary>
        protected abstract void Init(bool isFirstInit, bool callByAwake);
        protected virtual void UninitBeforeSceneUnload(bool isDestroy) { }
        protected virtual void UninitAfterSceneUnload(bool isDestroy) { }

    }

}
