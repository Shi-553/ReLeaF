using System.Collections.Generic;
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
        List<SpawnLake> spawnLakes = new();
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            spawnLakes.Clear();
        }

        void Start()
        {

        }

        public void AddEnabledLake(SpawnLake spawnLake)
        {
            spawnLakes.Add(spawnLake);
            spawnLake.transform.SetParent(enabledLake, true);
        }
        public void ChangeToDisabledLake(SpawnLake spawnLake)
        {
            spawnLake.transform.SetParent(disabledLake, true);
        }
    }
}
