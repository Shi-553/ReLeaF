using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class TileEffect : PoolableMonoBehaviour, IMultipleVisual
    {
        ParticleSystem particle;
        WaitForSeconds wait;

        [SerializeField]
        TileEffectInfo info;
        public TileEffectInfo TileEffectInfo => info;

        public int VisualType => info.TileEffectType.ToInt32();
        public int VisualMax => TileEffectType.Max.ToInt32();

        protected override void InitImpl()
        {
            if (!IsInitialized)
            {
                particle = GetComponentInChildren<ParticleSystem>();
                wait = new WaitForSeconds(particle.main.duration);
            }
            particle.Play(true);
            StartCoroutine(WaitParticle());
        }
        IEnumerator WaitParticle()
        {
            yield return wait;
            Release();
        }

        protected override void UninitImpl()
        {
        }

    }
}
