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
        [SerializeField]
        Vector3 offset;
        protected override void Init()
        {
        }
        private void Start()
        {
            effectPool = ComponentPool.Singleton.SetPool(effectPrefab);
        }

        public void SetDamageValueEffect(int damage, Vector2 pos)
        {
            var damageValueEffect = effectPool.Get<DamageValueEffect>();
            damageValueEffect.transform.SetParent(effectParent, false);
            damageValueEffect.ShowDamageValue(damage, (Vector3)pos + offset);
        }
    }
}
