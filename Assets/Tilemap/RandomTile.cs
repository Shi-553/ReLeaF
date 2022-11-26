using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using Pickle;
using UnityEngine.Tilemaps;
using Utility;
using Unity.VisualScripting;
using static Utility.MathExtension;

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
        RandomIndex randomIndex;

        protected override void InitImpl()
        {
            base.InitImpl();

            randomIndex = new RandomIndex(randomInfos.Select(r => r.probability).ToArray());

            poolArray = Pools.SetPoolArray((int)currentTileObject.TileType, randomInfos.Length);

            foreach (var info in randomInfos)
            {
                var v = info.multipleVisualTile as IMultipleVisual;
                poolArray.SetPool(v.VisualType, info.multipleVisualTile, 
                    (defaultCapacity* info.probability/ randomIndex.totalWeight).ConvertTo<int>(), 
                    (maxSize * info.probability / randomIndex.totalWeight).ConvertTo<int>(),
                    true);
            }
        }

        override protected void UpdateTileObject(Vector3Int position, ITilemap tilemap)
        {
            currentTileObject = randomInfos[randomIndex.Get()].multipleVisualTile;
            selected = currentTileObject as IMultipleVisual;
        }

        protected override IPool Pool => poolArray.GetPool(selected.VisualType);

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
