using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class MarkerBase : PoolableMonoBehaviour
    {
        protected Vector2Int tilePos;


        protected override void InitImpl()
        {
            tilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);
        }
        protected override void UninitImpl()
        {
        }

        public virtual void TileChanged(DungeonManager.TileChangedInfo info) { }

    }
}
