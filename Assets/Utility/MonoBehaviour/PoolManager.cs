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
        readonly Dictionary<Type, IPool> pools = new();
        public IReadOnlyDictionary<Type, IPool> Pools => pools;

        public override bool DontDestroyOnLoad => true;
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        protected override void UninitAfterSceneUnload(bool isDestroy)
        {
            foreach (var pool in pools.Values)
            {
                foreach (var p in pool)
                {
                    p?.Clear();
                }
            }
            pools.Clear();
            transform.GetChildren()
                .ForEach(t => Destroy(t.gameObject));
        }

        public IPool GetPool<T>() where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                return pool;
            }
            return null;
        }
        public IPool SetPool<T>(T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : PoolableMonoBehaviour
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
        public PoolArray GetPoolArray<T>() where T : PoolableMonoBehaviour
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool) && pool is PoolArray array)
            {
                return array;
            }
            return null;
        }

        public PoolArray SetPoolArray<T>(int size) where T : PoolableMonoBehaviour
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
        ObjectPool<PoolableMonoBehaviour> ObjectPool { get; }

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
        public T Get<T>() where T : PoolableMonoBehaviour
        {
            var val = ObjectPool.Get() as T;
            val.Init();
            return val;
        }
        public PoolInitializeHelper<T> Get<T>(out T val) where T : PoolableMonoBehaviour
        {
            return new PoolInitializeHelper<T>(val = ObjectPool.Get() as T);
        }
        public void Release<T>(T element) where T : PoolableMonoBehaviour
        {
            if (element.IsInitialized)
            {
                element.Uninit();
                ObjectPool.Release(element);
            }
        }

        public void Clear() => ObjectPool.Clear();

        public void Resize(int size)
        {
            var poolables = new PoolableMonoBehaviour[size];

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
        protected ObjectPool<PoolableMonoBehaviour> pool;
        ObjectPool<PoolableMonoBehaviour> IPool.ObjectPool => pool;

        // thisでキャプチャ
        readonly Transform parent;
        readonly PoolableMonoBehaviour prefab;

        public Pool(Transform parent, PoolableMonoBehaviour p, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false)
        {
            this.parent = parent;
            prefab = p;

#if UNITY_EDITOR
            if (p == null)
                Debug.LogError("Pool Prefab null!!!");
#endif

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

        ObjectPool<PoolableMonoBehaviour> IPool.ObjectPool => pools[0].ObjectPool;

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

        public IPool SetPool<T>(int index, T prefab, int defaultCapacity = 10, int maxSize = 100, bool setSizeWithCapacity = false) where T : PoolableMonoBehaviour
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
