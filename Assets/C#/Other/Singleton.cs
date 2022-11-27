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
        }
    }
    public abstract class SingletonBase<T> : DefinitionSingletonBase where T : SingletonBase<T>
    {
        static bool isInit = false;

        public new abstract bool DontDestroyOnLoad { get; }

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


        bool InManagerScene => gameObject.scene.buildIndex == SceneType.Manager.GetBuildIndex();

        public sealed override void UninitBeforeSceneUnloadDefinition()
        {
            UninitBeforeSceneUnload(!DontDestroyOnLoad);

#if DEFINE_SCENE_TYPE_ENUM
            if (!InManagerScene)
                SceneManager.MoveGameObjectToScene(transform.root.gameObject, SceneManager.GetSceneByBuildIndex(SceneType.Manager.GetBuildIndex()));
#endif
        }

        public sealed override void UninitAfterSceneUnloadDefinition()
        {
            UninitAfterSceneUnload(!DontDestroyOnLoad);

            if (!DontDestroyOnLoad)
            {
                isInit = false;
                singletonInstance = null;

                if (gameObject.GetComponents<Component>().Length <= 2)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
        }

        /// <summary>
        /// Awakeかさらに早く呼ばれる
        /// </summary>
        protected abstract void Init();
        protected virtual void UninitBeforeSceneUnload(bool isDestroy) { }
        protected virtual void UninitAfterSceneUnload(bool isDestroy) { }

    }

}
