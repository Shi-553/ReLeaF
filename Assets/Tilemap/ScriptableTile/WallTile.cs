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
        public Wall block;
        [Pickle]
        public Wall fill;

        PoolArray poolArray;

        protected override void InitImpl()
        {
            base.InitImpl();

            poolArray = Pools.SetPoolArray((int)currentTileObject.TileType, 2);
            poolArray.SetPool(0, block, defaultCapacity / 2, maxSize / 2, true);
            poolArray.SetPool(1, fill, defaultCapacity / 2, maxSize / 2, true);
        }
        protected override bool UpdateTileObject(Vector3Int position, ITilemap tilemap, TileObject oldTileObject)
        {
            Wall newWall;
            if (HasWallTile(tilemap, position + Vector3Int.up) &&
                HasWallTile(tilemap, position + Vector3Int.down) &&
                HasWallTile(tilemap, position + Vector3Int.left) &&
                HasWallTile(tilemap, position + Vector3Int.right))
            {
                newWall = fill;
            }
            else
            {
                newWall = block;
            }

            currentTileObject = newWall;

            if (oldTileObject is not Wall wall)
                return true;

            return wall.IsFill != newWall.IsFill;
        }

        protected override Pool Pool => poolArray.GetPool(currentTileObject == block ? 0 : 1);
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
        [MenuItem("Assets/Create/Tile/WallTile")]
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
