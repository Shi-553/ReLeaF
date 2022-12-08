using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ToLeafEffect : MonoBehaviour, IPoolableSelfRelease
    {
        ParticleSystem particle;
        WaitForSeconds wait;
        IPool IPoolableSelfRelease.Pool { get; set; }

        public void Init(bool isCreated)
        {
            if (isCreated)
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
            this.StaticCast<IPoolableSelfRelease>().Release();
        }
        public void Uninit()
        {
        }
    }
}
