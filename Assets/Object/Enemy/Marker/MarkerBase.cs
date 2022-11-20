using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public abstract class MarkerBase : MonoBehaviour
    {
        protected Vector2Int tilePos;

        // マネージャーが呼ぶ
        public virtual void Init(Vector2Int tilePos)
        {
            this.tilePos = tilePos;
        }

        public virtual void TileChanged(DungeonManager.TileChangedInfo info) { }

        // マネージャーが呼ぶ
        public virtual void Uninit()
        {
            Destroy(gameObject);
        }
    }
}
