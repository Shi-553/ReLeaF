using UnityEngine;

namespace Utility
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {

        public IPool Pool { get; set; }

        public virtual PoolableMonoBehaviour Create(Transform parent, IPool pool)
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

        public void Init()
        {
            InitImpl();
            IsInitialized = true;
        }
        protected abstract void InitImpl();

        public void Uninit()
        {
            UninitImpl();
            IsInitialized = false;
        }
        protected abstract void UninitImpl();
    }
}
