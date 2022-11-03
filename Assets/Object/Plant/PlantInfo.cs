using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu]
    public class PlantInfo : ScriptableObject
    {
        [SerializeField]
        PlantType plantType;
        public PlantType PlantType => plantType;

        [SerializeField]
        float hpMax;
        public float HpMax => hpMax;

        [SerializeField]
        float growTime = 10.0f;
        public float GrowTime => growTime;

        [SerializeField]
        DamageMagnification[] damageMagnifications;
        public IReadOnlyCollection<DamageMagnification> DamageMagnifications => damageMagnifications;
    }
}