using DebugLogExtension;
using UnityEditor;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SelectTile : TerrainTile
    {
        PoolArray poolArray;
        protected virtual IMultipleVisual MultipleVisual => currentTileObject as IMultipleVisual;

        protected override void InitImpl()
        {
            base.InitImpl();

            if (MultipleVisual == null)
            {
                $"{currentTileObject.name} is not IMultipleVisual".DebugLog();
                return;
            }
            poolArray = Pools.SetPoolArray((int)currentTileObject.TileType, MultipleVisual.VisualMax);
            poolArray.SetPool(MultipleVisual.VisualType, currentTileObject, defaultCapacity, maxSize);
        }


        protected override Pool Pool => poolArray.GetPool(MultipleVisual.VisualType);


#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tile/SelectTile")]
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
