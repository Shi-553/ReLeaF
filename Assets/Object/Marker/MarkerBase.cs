using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class MarkerBase : PoolableMonoBehaviour
    {
        public Vector2Int TilePos { get; protected set; }


        protected override void InitImpl()
        {
            TilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);
        }
        protected override void UninitImpl()
        {
        }


        /// <returns>消すときはtrue</returns>
        public virtual bool OnGreening(DungeonManager.GreeningInfo info) => false;

    }
}
