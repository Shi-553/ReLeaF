using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace ReLeaf
{
    public class WallTile : TerrainTile
    {
        public GameObject fill;
        GameObject current;
        override public GameObject Obj => current;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (HasWallTile(tilemap, position + Vector3Int.up) &&
                HasWallTile(tilemap, position + Vector3Int.down) &&
                HasWallTile(tilemap, position + Vector3Int.left) &&
                HasWallTile(tilemap, position + Vector3Int.right))
            {
                current = fill;
                Debug.Log("f");
            }
            else
            {
                current = gameobject;
            }
            return base.StartUp(position, tilemap, go);
        }

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
            var tile = tilemap.GetTile<WallTile>(pos);
            return tile!=null;
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
