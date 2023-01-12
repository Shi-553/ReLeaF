using UnityEngine;

namespace ReLeaf
{
    public class Wall : TileObject
    {
        [SerializeField]
        bool isFill;
        public bool IsFill => isFill;
    }
}
