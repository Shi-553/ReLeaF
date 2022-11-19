using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField]
        Sprite inCanvas;
        public Sprite InCanvas => inCanvas;

        [SerializeField]
        bool isImmediate = false;
        public bool IsImmediate => isImmediate;

        public bool IsFetched { get; private set; }
        public GameObject Fetcher { get; private set; }

        public bool Fetch(GameObject fetcher)
        {
            if (IsFetched)
                return false;

            Fetcher= fetcher;
            gameObject.SetActive(false);

            return true;
        }

        public abstract bool Use(Vector2Int tilePos,Vector2Int dir);
    }
}
