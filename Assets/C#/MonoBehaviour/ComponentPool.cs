using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Transform = UnityEngine.Transform;

namespace ReLeaf
{

    public class ComponentPool : MonoBehaviour
    {
        readonly Dictionary<Type, IPool> pools = new();

        [SerializeField]
        bool isDontDestroyOnLoad;

        public static ComponentPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        private void OnDestroy()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
        }
        public IPool GetPool<T>() where T : Component, IPoolable
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                return pool;
            }
            return null;
        }
        public IPool SetPool<T>(T prefab) where T : Component, IPoolable
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                return pool;
            }

            var poolParent = new GameObject(type.Name).transform;
            poolParent.parent = transform;

            var newPool = new Pool(poolParent, prefab);


            pools[type] = newPool;

            return newPool;
        }
        public PoolArray GetPoolArray<T>() where T : Component, IPoolable
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool) && pool is PoolArray array)
            {
                return array;
            }
            return null;
        }

        public PoolArray SetPoolArray<T>(int size) where T : Component, IPoolable
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                if (pool is PoolArray array)
                    return array;

                return null;
            }

            var poolParent = new GameObject(type.Name).transform;
            poolParent.parent = transform;

            var newPool = new PoolArray(poolParent, size);


            pools[type] = newPool;

            return newPool;
        }
    }

    public interface IPool
    {
        ObjectPool<IPoolable> ObjectPool { get; }

        public T Get<T>(Action<T> action) where T : Component, IPoolable
        {
            bool isCreated = ObjectPool.CountInactive==0;
            var t = ObjectPool.Get() as T;
            action(t);
            t.Init(isCreated);
            return t;
        }

        public void Release<T>(T element) where T : IPoolable {
            element.Uninit();
            ObjectPool.Release(element);
        }

        public void Clear() => ObjectPool.Clear();

    }

    public class Pool : IPool
    {
        protected ObjectPool<IPoolable> pool;
        ObjectPool<IPoolable> IPool.ObjectPool => pool;

        // thisでキャプチャ
        readonly Transform parent;
        readonly IPoolable prefab;

        public Pool(Transform parent, IPoolable p)
        {
            this.parent = parent;
            prefab = p;

#if UNITY_EDITOR
            if (p == null)
                Debug.LogError("Pool Prefab null!!!");
#endif

            pool = new ObjectPool<IPoolable>(
                             createFunc: () =>
                             {
                                 var instance = prefab.Create(this.parent);
                                 if (instance is IPoolableSelfRelease poolableSelfRelease)
                                     poolableSelfRelease.SetPool(this);

                                 return instance;
                             },
                             actionOnGet: target => target.OnGetPool(),
                             actionOnRelease: target => target.OnReleasePool(),
                             actionOnDestroy: target => target.OnDestroyPool(),
                             collectionCheck: true,                                                     // 同一インスタンスが登録されていないかチェックするかどうか
                             defaultCapacity: 10,                                                       // デフォルトの容量
                             maxSize: 100);

        }

    }

    public class PoolArray : IPool
    {
        readonly IPool[] pools;

        // thisでキャプチャ
        readonly Transform parent;

        ObjectPool<IPoolable> IPool.ObjectPool => pools[0].ObjectPool;

        public PoolArray(Transform parent, int size)
        {
            this.parent = parent;

            pools = new IPool[size];
        }

        public IPool GetPool(int index)
        {
            if (pools[index] != null)
                return pools[index];

            return null;
        }

        public IPool SetPool<T>(int index, T prefab) where T : Component, IPoolable
        {
            if (pools[index] != null)
                return pools[index];

            var newPool = new Pool(parent, prefab);

            pools[index] = newPool;

            return newPool;
        }

        public PoolArray GetPoolArray(int index)
        {
            if (pools[index] is PoolArray array)
            {
                return array;
            }
            return null;
        }

        public PoolArray SetPoolArray(int index, int size)
        {
            if (pools[index] != null)
            {
                if (pools[index] is PoolArray array)
                    return array;

                return null;
            }

            var poolParent = new GameObject(index.ToString()).transform;
            poolParent.parent = parent;

            var newPool = new PoolArray(poolParent, size);

            pools[index] = newPool;

            return newPool;
        }
    }
}
