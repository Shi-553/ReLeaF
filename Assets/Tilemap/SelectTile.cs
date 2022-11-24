
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using Pickle;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public class SelectTile : TerrainTile
    {
        [Pickle]
        public TileObject[] selectTile;

        public IMultipleVisual Selected { get; set; }

        PoolArray poolArray;
        override protected void UpdateTileObject(Vector3Int position, ITilemap tilemap)
        {
            currentTileObject = selectTile[Selected.VisualType];
        }
        protected override IPool Pool
        {
            get
            {
                poolArray ??= Pools.SetPoolArray((int)currentTileObject.TileType, Selected.VisualTypeMax);

                return poolArray.SetPool(Selected.VisualType, currentTileObject);
            }
        }


#if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/2D/Tiles/SelectTile")]
        public static void CreateSelectTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Select Tile", "New Select Tile", "Asset", "Save Select Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SelectTile>(), path);
        }
#endif
    }
}
