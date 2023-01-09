using Pickle;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
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

            selected = currentTileObject as IMultipleVisual;
            poolArray = Pools.SetPoolArray((int)currentTileObject.TileType, selected.VisualMax);

            foreach (var info in randomInfos)
            {
                var v = info.multipleVisualTile as IMultipleVisual;
                poolArray.SetPool(v.VisualType, info.multipleVisualTile,
                    (defaultCapacity * info.probability / randomIndex.totalWeight).ConvertTo<int>(),
                    (maxSize * info.probability / randomIndex.totalWeight).ConvertTo<int>(),
                    true);
            }
        }

        override protected bool UpdateTileObject(Vector3Int position, ITilemap tilemap, TileObject oldTileObject)
        {
            if (oldTileObject != null)
                return false;

            currentTileObject = randomInfos[randomIndex.Get()].multipleVisualTile;
            selected = currentTileObject as IMultipleVisual;

            return true;
        }

        protected override Pool Pool => poolArray.GetPool(selected.VisualType);

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tile/RandomTile")]
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
