using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace ReLeaf
{
    public interface IPoolable
    {
        Component Component => this as Component;

        public virtual IPoolable Create(Transform parent)=> Object.Instantiate(Component, parent) as IPoolable;
        public virtual void OnDestroyPool() => Object.Destroy(Component);
        public virtual void OnGetPool() => Component.gameObject.SetActive(true);
        public virtual void OnReleasePool() => Component.gameObject.SetActive(false);


        public void Init(bool isCreated);
        public void Uninit();
    }


    public interface IPoolableSelfRelease : IPoolable
    {
        IPool Pool { get; protected set; }

        public void SetPool(IPool pool)
        {
            Pool = pool;
        }
        public void Release()
        {
            Pool?.Release(this);
        }
    }
}
