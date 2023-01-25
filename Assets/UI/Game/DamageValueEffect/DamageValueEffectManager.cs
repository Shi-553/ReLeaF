using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DamageValueEffectManager : SingletonBase<DamageValueEffectManager>
    {
        public override bool DontDestroyOnLoad => true;

        Pool effectPool;
        [SerializeField]
        DamageValueEffect effectPrefab;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
        private void Start()
        {
            effectPool = PoolManager.Singleton.SetPool(effectPrefab, 10, 100, true);
        }

        public void SetDamageValueEffect(int damage, Vector2 pos)
        {
            var damageValueEffect = effectPool.Get<DamageValueEffect>();
            damageValueEffect.transform.SetParent(transform, false);
            damageValueEffect.ShowDamageValue(damage, (Vector3)pos);
        }
    }
}
