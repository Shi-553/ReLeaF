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
        public virtual void OnDestroyPool() => Destroy(gameObject);
        public virtual void OnGetPool() => gameObject.SetActive(true);
        public virtual void OnReleasePool() => gameObject.SetActive(false);


        public void Release()
        {
            Pool.Release(this);
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
