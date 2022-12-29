using UnityEngine;

namespace ReLeaf
{
    public interface ISowSeedSpecialPowerInfo
    {
        public Vector2Int[] GetSeedLocalTilePos(Vector2Int dir);
    }
}
