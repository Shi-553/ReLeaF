using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ToLeafEffect : PoolableMonoBehaviour
    {
        ParticleSystem particle;
        WaitForSeconds wait;

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
