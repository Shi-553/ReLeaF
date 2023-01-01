using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(menuName = "Sound/SEManagerInfo")]
    public class SEManagerInfo : ScriptableObject
    {
        [SerializeField]
        float spatialBlend = 0.8f;
        public float SpatialBlend => spatialBlend;
    }
}
