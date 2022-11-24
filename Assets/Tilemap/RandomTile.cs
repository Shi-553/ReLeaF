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
    public class RandomTile : TerrainTile
    {
        [Serializable]
        public class RandomInfo
        {
            public float probability = 50.0f;
            [Pickle(typeof(IMultipleVisual))]
            public TileObject multipleVisualTile;
        }

        public RandomInfo[] randomInfos;
        IMultipleVisual selected;

        PoolArray poolArray;
        override protected void UpdateTileObject(Vector3Int position, ITilemap tilemap)
        {
            var index = MathExtension.GetRandomIndex(randomInfos.Select(r => r.probability).ToArray());
            currentTileObject = randomInfos[index].multipleVisualTile;
            selected = currentTileObject as IMultipleVisual;
        }
        protected override IPool Pool
        {
            get
            {
                poolArray ??= Pools.SetPoolArray((int)currentTileObject.TileType, selected.VisualTypeMax);

                return poolArray.SetPool(selected.VisualType,currentTileObject);
            }
        }


#if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/2D/Tiles/RandomTile")]
        public static void CreateRandomTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Random Tile", "New Random Tile", "Asset", "Save Random Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RandomTile>(), path);
        }
#endif
    }
}
