using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("植物({asset.dirname})のパラメータ")]
    [CreateAssetMenu(menuName = "Tile/PlantInfo")]
    public class PlantInfo : TileObjectInfo
    {
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