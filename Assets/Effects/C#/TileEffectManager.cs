using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class TileEffectManager : SingletonBase<TileEffectManager>
    {
        [SerializeField]
        TileEffect[] tileEffects;

        PoolArray pool;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                pool = PoolManager.Singleton.SetPoolArray<TileEffect>(tileEffects.First().VisualMax);

                foreach (var tileEffect in tileEffects)
                {
                    pool.SetPool(
                        tileEffect.VisualType,
                        tileEffect,
                        tileEffect.TileEffectInfo.DefaultCapacity,
                        tileEffect.TileEffectInfo.MaxSize,
                        true);
                }

            }
        }
        public void SetEffect(TileEffectType type, Vector3 pos)
        {
            var effect = pool.GetPool(type.ToInt32()).Get<TileEffect>();
            effect.transform.position = pos;
        }
    }
}
