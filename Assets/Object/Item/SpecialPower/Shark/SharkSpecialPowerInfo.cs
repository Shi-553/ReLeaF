using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : ScriptableObject
    {

        [SerializeField]
        Vector2Int[] weakLocalTilePos;
        public Vector2Int[] WeakLocalTilePos => weakLocalTilePos;
    }
}
