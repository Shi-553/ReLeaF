using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using static ReLeaf.IPool;
using Object = UnityEngine.Object;
using Transform = UnityEngine.Transform;

namespace ReLeaf
{

    public class ComponentPool : MonoBehaviour
    {
        readonly Dictionary<Type, IPool> pools = new();
        public IReadOnlyDictionary<Type, IPool> Pools => pools;

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
        public IPool SetPool<T>(T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : Component, IPoolable
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                return pool;
            }

            var poolParent = new GameObject(type.Name).transform;
            poolParent.parent = transform;

            var newPool = new Pool(poolParent, prefab, defaultCapacity, maxSize, setSizeWithCapacity);


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

    public interface IPool : IEnumerable<IPool>
    {
        ObjectPool<IPoolable> ObjectPool { get; }

        public readonly struct PoolInitializeHelper<T> : IDisposable where T : Component, IPoolable
        {
            readonly bool isCreated;
            readonly T value;
            public PoolInitializeHelper(T value, bool isCreated)
            {
                this.value = value;
                this.isCreated = isCreated;
            }
            public void Dispose()
            {
                value.Init(isCreated);
            }
        }
        public T Get<T>() where T : Component, IPoolable
        {
            bool isCreated = ObjectPool.CountInactive == 0;
            var val = ObjectPool.Get() as T;
            val.Init(isCreated);
            return val;
        }
        public PoolInitializeHelper<T> Get<T>(out T val) where T : Component, IPoolable
        {
            bool isCreated = ObjectPool.CountInactive == 0;
            return new PoolInitializeHelper<T>(val = ObjectPool.Get() as T, isCreated);
        }
        public void Release<T>(T element) where T : IPoolable
        {
            element.Uninit();
            ObjectPool.Release(element);
        }

        public void Clear() => ObjectPool.Clear();

        public void Resize(int size)
        {
            var poolables = new IPoolable[size];

            for (int i = 0; i < size; i++)
            {
                poolables[i] = ObjectPool.Get();
            }
            for (int i = 0; i < size; i++)
            {
                ObjectPool.Release(poolables[i]);
            }

        }
    }

    public class Pool : IPool
    {
        protected ObjectPool<IPoolable> pool;
        ObjectPool<IPoolable> IPool.ObjectPool => pool;

        // thisでキャプチャ
        readonly Transform parent;
        readonly IPoolable prefab;

        public Pool(Transform parent, IPoolable p, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false)
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
                             defaultCapacity: defaultCapacity,                                                       // デフォルトの容量
                             maxSize: maxSize);

            if (setSizeWithCapacity)
            {
                this.StaticCast<IPool>().Resize(defaultCapacity);
            }
        }

        public IEnumerator<IPool> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

        public IPool SetPool<T>(int index, T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : Component, IPoolable
        {
            if (pools[index] != null)
                return pools[index];

            var newPool = new Pool(parent, prefab, defaultCapacity, maxSize, setSizeWithCapacity);

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

        public IEnumerator<IPool> GetEnumerator()
        {
            foreach (var p in pools)
            {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
