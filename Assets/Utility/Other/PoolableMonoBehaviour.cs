using DebugLogExtension;
using UnityEngine;

namespace Utility
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {

        public Pool Pool { get; set; }

        public virtual PoolableMonoBehaviour Create(Transform parent, Pool pool)
        {
            var instance = Instantiate(this, parent);
            instance.IsInitialized = false;
            instance.Pool = pool;
            return instance;
        }
        public virtual void OnDestroyPool()
        {
            if (this == null)
                return;
            if (Application.isEditor)
                GetType().DebugLog();
            Destroy(gameObject);
        }
        public virtual void OnGetPool()
        {
            if (this == null)
                return;
            gameObject.SetActive(true);
        }
        public virtual void OnReleasePool() => gameObject.SetActive(false);


        public void Release()
        {
            if (Pool != null)
            {
                Pool.Release(this);
                return;
            }
            OnReleasePool();
        }


        public bool IsInitialized { get; protected set; }

        public void FasterInit()
        {
            if (!IsInitialized)
                FasterInitOnlyOnceImpl();
        }
        public void Init()
        {
            InitImpl();
            IsInitialized = true;
        }
        protected virtual void FasterInitOnlyOnceImpl() { }
        protected abstract void InitImpl();

        public void Uninit()
        {
            UninitImpl();
            IsInitialized = false;
        }
        protected abstract void UninitImpl();
    }
}
