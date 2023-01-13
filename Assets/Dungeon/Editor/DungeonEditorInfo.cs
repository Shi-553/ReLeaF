using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("ダンジョンエディタの情報")]
    [CreateAssetMenu(menuName = "Tile/DungeonEditorInfo")]
    public class DungeonEditorInfo : ScriptableObject
    {
        [SerializeField]
        TerrainTile wallTile;
        public TerrainTile WallTile => wallTile;
        [SerializeField]
        TerrainTile altWallTile;
        public TerrainTile AltWallTile => altWallTile;
    }
}