using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum TileEffectType
    {
        ToLeaf,
        ToSand,
        Blast,
        Max
    }
    [ClassSummary("タイルエフェクト情報")]
    [CreateAssetMenu(menuName = "Effect/TileEffectInfo")]
    public class TileEffectInfo : ScriptableObject
    {
        [SerializeField]
        TileEffectType tileEffectType;
        public TileEffectType TileEffectType => tileEffectType;

        [SerializeField]
        int defaultCapacity = 10;
        public int DefaultCapacity => defaultCapacity;

        [SerializeField]
        int maxSize = 100;
        public int MaxSize => maxSize;

    }
}
