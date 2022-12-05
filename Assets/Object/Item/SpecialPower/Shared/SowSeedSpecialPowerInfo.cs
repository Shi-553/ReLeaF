using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("種をまくスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/Shared/SowSeedSpecialPower")]
    public class SowSeedSpecialPowerInfo : ScriptableObject
    {

        [SerializeField, Rename("種をまくマス")]
        LocalTilePos seedLocalTilePos;
        public LocalTilePos SeedLocalTilePos => seedLocalTilePos;
    }
}
