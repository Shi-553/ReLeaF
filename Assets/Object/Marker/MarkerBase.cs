using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public abstract class MarkerBase : MonoBehaviour,IPoolable
    {
        protected Vector2Int tilePos;

        public void Init(bool isCreate)
        {
            tilePos = DungeonManager.Instance.WorldToTilePos(transform.position);
        }
        public void Uninit()
        {
        }

        public virtual void TileChanged(DungeonManager.TileChangedInfo info) { }

    }
}
