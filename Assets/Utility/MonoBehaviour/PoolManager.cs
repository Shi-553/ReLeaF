using DebugLogExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Transform = UnityEngine.Transform;

namespace Utility
{

    public class PoolManager : SingletonBase<PoolManager>
    {
        readonly Dictionary<Type, IEnumerablePool> pools = new();
        public IReadOnlyDictionary<Type, IEnumerablePool> Pools => pools;

        public override bool DontDestroyOnLoad => true;
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        public void Uninit()
        {
            transform.GetChildren()
                .ForEach(t => Destroy(t.gameObject));
            foreach (var pool in pools.Values)
            {
                pool?.Clear();
            }
            pools.Clear();
        }

        public Pool GetPool<T>() where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var enumerablePool) && enumerablePool is Pool pool)
            {
                return pool;
            }
            return null;
        }
        public Pool SetPool<T>(T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var enumerablePool))
            {
                if (enumerablePool is Pool pool)
                    return pool;

                "Aleady Using".DebugLogError();
                return null;
            }

            var poolParent = new GameObject(type.Name).transform;
            poolParent.parent = transform;

            var newPool = new Pool(poolParent, prefab, defaultCapacity, maxSize, setSizeWithCapacity);


            pools[type] = newPool;

            return newPool;
        }
        public PoolArray GetPoolArray<T>() where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var enumerablePool) && enumerablePool is PoolArray array)
            {
                return array;
            }
            return null;
        }

        public PoolArray SetPoolArray<T>(int size) where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var enumerablePool))
            {
                if (enumerablePool is PoolArray array)
                    return array;

                "Aleady Using".DebugLogError();
                return null;
            }

            var poolParent = new GameObject(type.Name).transform;
            poolParent.parent = transform;

            var newPool = new PoolArray(poolParent, size);


            pools[type] = newPool;

            return newPool;
        }

    }

    public interface IEnumerablePool : IEnumerable<Pool>
    {
        public void Clear();
    }

    public class Pool : IEnumerablePool
    {
        protected ObjectPool<PoolableMonoBehaviour> pool;

        // thisでキャプチャ
        readonly Transform parent;
        readonly PoolableMonoBehaviour prefab;

        public int DefaultCapacity { get; private set; }
        public int MaxSize { get; private set; }

        public Pool(Transform parent, PoolableMonoBehaviour p, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false)
        {
            this.parent = parent;
            prefab = p;

#if UNITY_EDITOR
            if (p == null)
                "Pool Prefab null!!!".DebugLogError();
#endif

            DefaultCapacity = defaultCapacity;
            MaxSize = maxSize;

            pool = new ObjectPool<PoolableMonoBehaviour>(
                             createFunc: () => prefab.Create(this.parent, this),
                             actionOnGet: target => target.OnGetPool(),
                             actionOnRelease: target => target.OnReleasePool(),
                             actionOnDestroy: target => target.OnDestroyPool(),
                             collectionCheck: true,                                                     // 同一インスタンスが登録されていないかチェックするかどうか
                             defaultCapacity: defaultCapacity,                                                       // デフォルトの容量
                             maxSize: maxSize);

            if (setSizeWithCapacity)
            {
                Resize(defaultCapacity);
            }
        }


        public readonly struct PoolInitializeHelper<T> : IDisposable where T : PoolableMonoBehaviour
        {
            readonly T value;
            public PoolInitializeHelper(T value)
            {
                this.value = value;
            }
            public void Dispose()
            {
                value.Init();
            }
        }
        private T GetImpl<T>() where T : PoolableMonoBehaviour
        {
            while (true)
            {
                var val = pool.Get() as T;
                if (val != null)
                {
                    val.FasterInit();
                    return val;
                }
            }
        }
        public T Get<T>() where T : PoolableMonoBehaviour
        {
            var val = GetImpl<T>();
            val.Init();
            return val;
        }
        public PoolInitializeHelper<T> Get<T>(out T val) where T : PoolableMonoBehaviour
        {
            return new PoolInitializeHelper<T>(val = GetImpl<T>());
        }
        public void Release<T>(T element) where T : PoolableMonoBehaviour
        {
            if (element.IsInitialized)
            {
                element.Uninit();
                pool.Release(element);

            }
        }

        public void Clear() => pool?.Clear();

        public void Resize(int size)
        {
            if (pool.CountInactive >= size)
                return;
            var poolables = new PoolableMonoBehaviour[size];

            for (int i = 0; i < size; i++)
            {
                poolables[i] = GetImpl<PoolableMonoBehaviour>();
            }
            for (int i = 0; i < size; i++)
            {
                pool.Release(poolables[i]);
            }
        }

        public IEnumerator<Pool> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class PoolArray : IEnumerablePool
    {
        readonly IEnumerablePool[] pools;

        readonly Transform parent;

        public void Clear() => pools.ForEach(pool => pool?.Clear());

        public PoolArray(Transform parent, int size)
        {
            this.parent = parent;

            pools = new IEnumerablePool[size];
        }

        public Pool GetPool(int index)
        {
            if (pools[index] is Pool pool)
                return pool;

            return null;
        }

        public Pool SetPool<T>(int index, T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : PoolableMonoBehaviour
        {
            if (pools[index] != null)
            {
                if (pools[index] is Pool pool)
                    return pool;

                "Aleady Using".DebugLogError();
                return null;
            }

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

                "Aleady Using".DebugLogError();
                return null;
            }

            var poolParent = new GameObject(index.ToString()).transform;
            poolParent.parent = parent;

            var newPool = new PoolArray(poolParent, size);

            pools[index] = newPool;

            return newPool;
        }

        public IEnumerator<Pool> GetEnumerator()
        {
            foreach (var enumerablePool in pools)
            {
                if (enumerablePool == null)
                    continue;

                foreach (var p in enumerablePool)
                {
                    yield return p;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
