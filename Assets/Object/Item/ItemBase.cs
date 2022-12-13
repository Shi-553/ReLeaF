using System.Collections;
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


        public abstract void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns);
        public abstract IEnumerator Use(Vector2Int tilePos, Vector2Int dir);
    }
}
