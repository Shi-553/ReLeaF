using System;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class RobotGreening : SingletonBase<RobotGreening>
    {
        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        [SerializeField]
        ParticleSystem particle;

        public struct AutoFinish : IDisposable
        {
            public void Dispose()
            {
                RobotGreening.Singleton.FinishGreening();
            }
        }
        public AutoFinish StartGreening()
        {
            particle.Play(true);
            return new();
        }
        public void FinishGreening()
        {
            particle.Stop(true);
        }
    }
}
