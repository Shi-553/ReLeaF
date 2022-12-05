using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("����܂��X�y�V�����p���[�p�����[�^")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/Shared/SowSeedSpecialPower")]
    public class SowSeedSpecialPowerInfo : ScriptableObject
    {

        [SerializeField, Rename("����܂��}�X")]
        LocalTilePos seedLocalTilePos;
        public LocalTilePos SeedLocalTilePos => seedLocalTilePos;
    }
}
