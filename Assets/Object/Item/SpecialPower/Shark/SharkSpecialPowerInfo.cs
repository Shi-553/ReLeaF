using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("Sharkのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : ScriptableObject
    {

        [SerializeField, Rename("種をまくマス")]
        LocalTilePos seedLocalTilePos;
        public LocalTilePos SeedLocalTilePos => seedLocalTilePos;
    }
}
