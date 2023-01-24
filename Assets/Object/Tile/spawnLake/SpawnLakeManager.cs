using UnityEngine;
using Utility;

namespace ReLeaf
{

    public class SpawnLakeManager : SingletonBase<SpawnLakeManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        Transform enabledLake;
        [SerializeField]
        Transform disabledLake;
        [SerializeField]
        AudioInfo seLakeClean;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        public void AddEnabledLake(SpawnLake spawnLake)
        {
            spawnLake.transform.SetParent(enabledLake, true);
        }

        public void ChangeToDisabledLake(SpawnLake spawnLake)
        {
            SEManager.Singleton.Play(seLakeClean);
            spawnLake.transform.SetParent(disabledLake, true);
        }
    }
}
