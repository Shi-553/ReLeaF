using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace ReLeaf
{
    [CustomGridBrush(false, true, false, "Layer Fixed Brush")]
    public class LayerFixedBrush : GridBrush
    {
        [SerializeField]
        TerrainTile sandTile;

        [SerializeField]
        [HideInInspector]
        List<Tilemap> tilemaps = new();
        [SerializeField]
        [HideInInspector]
        Dictionary<TileLayerType, Tilemap> tilemapLayerDic = new();

        [SerializeField]
        [HideInInspector]
        GridLayout beforeGridLayout;

        bool UpdateTilemapDic(GridLayout gridLayout = null)
        {
            if (gridLayout == null)
                gridLayout = Object.FindObjectOfType<GridLayout>();

            if (gridLayout == null)
                return false;

            beforeGridLayout = gridLayout;

            gridLayout.GetComponentsInChildren(tilemaps);

            tilemapLayerDic.Clear();
            foreach (var map in tilemaps)
            {
                if (Enum.TryParse<TileLayerType>(map.name, out var mapType))
                    tilemapLayerDic.Add(mapType, map);
            }
            return tilemapLayerDic.Count != 0;
        }

        [SerializeField]
        [HideInInspector]
        private List<TileChangeData> tileChangeDataList;

        Tilemap GroundTileMap => tilemapLayerDic[TileLayerType.Ground];

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!UpdateTilemapDic(gridLayout))
                return;

            foreach (var tilemap in tilemaps)
            {
                base.BoxErase(gridLayout, tilemap.gameObject, position);
            }
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!UpdateTilemapDic(gridLayout))
                return;

            int count = 0;
            var listSize = position.size.x * position.size.y * position.size.z;
            if (tileChangeDataList == null || tileChangeDataList.Capacity != listSize)
                tileChangeDataList = new List<TileChangeData>(listSize);
            tileChangeDataList.Clear();
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell.tile == null)
                    continue;

                var tcd = new TileChangeData { position = location, tile = cell.tile, transform = cell.matrix, color = cell.color };
                tileChangeDataList.Add(tcd);
                count++;
            }
            // Duplicate empty slots in the list, as ExtractArrayFromListT returns full list
            if (0 < count && count < listSize)
            {
                var tcd = tileChangeDataList[count - 1];
                for (int i = count; i < listSize; ++i)
                {
                    tileChangeDataList.Add(tcd);
                }
            }

            for (int i = 0; i < tileChangeDataList.Count; i++)
            {
                TileChangeData data = tileChangeDataList[i];

                if (data.tile is not TerrainTile terrainTile)
                {
                    continue;
                }

                if (terrainTile is not SandPaddingTile paddingTile)
                {
                    SetTile(ref data, terrainTile.TileLayerType);
                    continue;
                }
                if (terrainTile.TileLayerType != TileLayerType.Ground)
                {
                    Debug.LogError(terrainTile.name + " is only Ground Layer", terrainTile);
                    continue;
                }

                SetTile(ref data, terrainTile.TileLayerType);

                data.tile = sandTile;

                var pos = data.position;
                for (int x = 0; x < paddingTile.Size.x; x++)
                {
                    for (int y = 0; y < paddingTile.Size.y; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }
                        data.position = new(pos.x + x, pos.y + y, pos.z);
                        SetTile(ref data, sandTile.TileLayerType);

                    }
                }

            }
        }
        void SetTile(ref TileChangeData data, TileLayerType paintLayer)
        {
            var obj = GroundTileMap.GetInstantiatedObject(data.position);
            if (obj != null && obj.TryGetComponent<Sand>(out var connectedSand) && connectedSand.Target != null)
            {
                var targetPos = connectedSand.Target.TilePos;
                if (GroundTileMap.GetTile((Vector3Int)targetPos) is SandPaddingTile paddingTile)
                {
                    for (int x = 0; x < paddingTile.Size.x; x++)
                    {
                        for (int y = 0; y < paddingTile.Size.y; y++)
                        {
                            var pos = new Vector3Int(targetPos.x + x, targetPos.y + y);
                            if (pos.x == data.position.x && pos.y == data.position.y)
                            {
                                continue;
                            }
                            GroundTileMap.SetTile(pos, sandTile);
                        }
                    }
                }
            }

            tilemapLayerDic[paintLayer].SetTile(data, false);

            var tile = data.tile;
            data.tile = null;

            foreach (var layer in tilemapLayerDic.Keys)
            {
                if (layer != paintLayer)
                    tilemapLayerDic[layer].SetTile(data, false);
            }

            data.tile = tile;
        }

        void ClearZNotZeroTile()
        {
            if (!UpdateTilemapDic())
                return;

            List<Vector3Int> posList = new();
            foreach (var tilemap in tilemapLayerDic.Values)
            {
                posList.Clear();
                foreach (var pos in tilemap.cellBounds.allPositionsWithin)
                {
                    if (pos.z == 0) continue;

                    posList.Add(pos);
                }
                tilemap.SetTiles(posList.ToArray(), new TileBase[posList.Count]);
            }
            EditorSceneManager.MarkSceneDirty(beforeGridLayout.gameObject.scene);
        }


        [MenuItem("Assets/Create/Tile/Brush/LayerFixedBrush")]
        public static void CreateLayerFixedBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save LayerFixedBrush", "New LayerFixedBrush", "Asset", "Save LayerFixedBrush");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LayerFixedBrush>(), path);
        }

        [CustomEditor(typeof(LayerFixedBrush))]
        public class LayerFixedBrushEditor : GridBrushEditor
        {
            public override bool canChangeZPosition => false;

            public override void OnPaintInspectorGUI()
            {
                var brush = target as LayerFixedBrush;

                if (GUILayout.Button("zが0じゃないものをクリア   （何故か通れない場所があったときに押す）"))
                {
                    brush.ClearZNotZeroTile();
                }

            }
        }
    }
}
