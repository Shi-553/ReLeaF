using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField]
        Sprite icon;
        public Sprite Icon => icon;

        [SerializeField]
        bool isImmediate = false;
        public bool IsImmediate => isImmediate;

        public bool IsFetched { get; private set; }


        public bool Fetch()
        {
            if (IsFetched)
                return false;

            gameObject.SetActive(false);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tilePos"></param>
        /// <param name="dir"></param>
        /// <returns>world tile pos</returns>
        public abstract List<Vector2Int> PreviewRange(Vector2Int tilePos, Vector2Int dir);
        public abstract void Use(Vector2Int tilePos, Vector2Int dir);
    }
}
