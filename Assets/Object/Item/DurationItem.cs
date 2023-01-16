using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DurationItem : ItemBaseInfo
    {
        [SerializeField, Rename("効果時間(秒)")]
        float duration = 10;
        public float Duration => duration;

        public override string Description => description
            .Replace("{" + nameof(duration) + "}", duration.ToString("0.#"));
    }
}
