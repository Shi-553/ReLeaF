using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : ScriptableObject
    {

        [SerializeField]
        Vector2Int[] seedLocalTilePos;
        public Vector2Int[] SeedLocalTilePos => seedLocalTilePos;
    }
}
