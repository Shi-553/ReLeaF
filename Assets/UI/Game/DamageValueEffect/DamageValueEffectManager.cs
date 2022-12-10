using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DamageValueEffectManager : SingletonBase<DamageValueEffectManager>
    {
        public override bool DontDestroyOnLoad => false;

        IPool effectPool;
        [SerializeField]
        DamageValueEffect effectPrefab;

        [SerializeField]
        Transform effectParent;
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
            damageValueEffect.transform.SetParent(effectParent, false);
            damageValueEffect.ShowDamageValue(damage, (Vector3)pos);
        }
    }
}
