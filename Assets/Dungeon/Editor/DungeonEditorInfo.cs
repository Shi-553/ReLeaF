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

        [SerializeField]
        int left = 10;
        public int Left => left;
        [SerializeField]
        int right = 10;
        public int Right => right;
        [SerializeField]
        int down = 5;
        public int Down => down;
        [SerializeField]
        int up = 7;
        public int Up => up;
    }
}