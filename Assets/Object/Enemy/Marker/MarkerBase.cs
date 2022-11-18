using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public abstract class MarkerBase : MonoBehaviour
    {
        protected Vector2Int tilePos;
        public virtual void Init(Vector2Int tilePos)
        {
            this.tilePos = tilePos;
        }
        public virtual void TileChanged(DungeonManager.TileChangedInfo info) { }
        public virtual void Uninit()
        {
            Destroy(gameObject);
        }
    }
}
