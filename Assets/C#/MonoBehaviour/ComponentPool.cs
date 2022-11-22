using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace ReLeaf
{

    public class ComponentPool : MonoBehaviour
    {
        Dictionary<Type, Pool> pools = new Dictionary<Type, Pool>();

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
        public Pool GetPool<T>(T prefab) where T : Component
        {
            var type = typeof(T);
            return GetPool(type, prefab);
        }
        public Pool GetPool(Type type,Component prefab) 
        {
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
    }

    public class Pool
    {
        ObjectPool<Component> pool;

        // thisでキャプチャ
        readonly Transform parent;
        readonly Component prefab;

        public Pool(Transform parent, Component prefab)
        {
            this.parent = parent;
            this.prefab = prefab;

            var test= Object.Instantiate(this.prefab, this.parent);
            Debug.Log(test.name);

            pool = new ObjectPool<Component>(
                             createFunc:()=> Object.Instantiate(this.prefab, this.parent),                               // プールが空のときに新しいインスタンスを生成する処理
                             actionOnGet: target => target.gameObject.SetActive(true),                  // プールから取り出されたときの処理 
                             actionOnRelease: target => target.gameObject.SetActive(false),             // プールに戻したときの処理
                             actionOnDestroy: target => Object.Destroy(target),                                // プールがmaxSizeを超えたときの処理
                             collectionCheck: true,                                                     // 同一インスタンスが登録されていないかチェックするかどうか
                             defaultCapacity: 10,                                                       // デフォルトの容量
                             maxSize: 100);

        }

        public T Get<T>() where T : Component => pool.Get() as T;

        public void Release<T>(T element) where T : Component => pool.Release(element);

        public void Clear() => pool.Clear();


        public PoolHelper<T> GetHelper<T>() where T : Component => new PoolHelper<T>(pool);

        public class PoolHelper<T> where T : Component
        {
            ObjectPool<Component> pool;
            public PoolHelper(ObjectPool<Component> pool)
            {
                this.pool = pool;
            }


            public T Get() => pool.Get() as T;

            public void Release(T element) => pool.Release(element);

            public void Clear() => pool.Clear();
        }
    }
}
