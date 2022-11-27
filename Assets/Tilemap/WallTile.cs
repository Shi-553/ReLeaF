using Pickle;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public class WallTile : TerrainTile
    {
        [Pickle]
        public TileObject block;
        [Pickle]
        public TileObject fill;

        PoolArray poolArray;

        protected override void InitImpl()
        {
            base.InitImpl();

            poolArray = Pools.SetPoolArray((int)currentTileObject.TileType, 2);
            poolArray.SetPool(0, block);
            poolArray.SetPool(1, fill);
        }
        protected override void UpdateTileObject(Vector3Int position, ITilemap tilemap)
        {
            if (HasWallTile(tilemap, position + Vector3Int.up) &&
                HasWallTile(tilemap, position + Vector3Int.down) &&
                HasWallTile(tilemap, position + Vector3Int.left) &&
                HasWallTile(tilemap, position + Vector3Int.right))
            {
                currentTileObject = fill;
            }
            else
            {
                currentTileObject = block;
            }
        }

        protected override IPool Pool => poolArray.GetPool(currentTileObject == block ? 0 : 1);
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            tilemap.RefreshTile(position);

            var up = position + Vector3Int.up;
            if (HasWallTile(tilemap, up))
                tilemap.RefreshTile(up);

            var down = position + Vector3Int.down;
            if (HasWallTile(tilemap, down))
                tilemap.RefreshTile(down);

            var left = position + Vector3Int.left;
            if (HasWallTile(tilemap, left))
                tilemap.RefreshTile(left);

            var right = position + Vector3Int.right;
            if (HasWallTile(tilemap, right))
                tilemap.RefreshTile(right);
        }
        bool HasWallTile(ITilemap tilemap, Vector3Int pos)
        {
            return tilemap.GetTile(pos) == this;
        }


#if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/2D/Tiles/WallTile")]
        public static void CreateWallTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Wall Tile", "New Wall Tile", "Asset", "Save Wall Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WallTile>(), path);
        }
#endif
    }
}
