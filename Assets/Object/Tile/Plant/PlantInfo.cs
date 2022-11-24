using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("植物({asset.dirname})のパラメータ")]
    [CreateAssetMenu]
    public class PlantInfo : ScriptableObject
    {
        [SerializeField, Rename("植物タイプ")]
        PlantType plantType;
        public PlantType PlantType => plantType;

        [SerializeField, Rename("最大HP")]
        float hpMax;
        public float HpMax => hpMax;

        [SerializeField, Rename("成長にかかる時間(秒)")]
        float growTime = 10.0f;
        public float GrowTime => growTime;


        [SerializeField, Rename("ダメージタイプごとの被ダメージ倍率")]
        DamageMagnification[] damageMagnifications;
        public IReadOnlyCollection<DamageMagnification> DamageMagnifications => damageMagnifications;
    }
}