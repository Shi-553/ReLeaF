using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    [CustomGridBrush(false, true, false, "Layer Fixed Brush")]
    public class LayerFixedBrush : GridBrush
    {
        [SerializeField]
        TerrainTile sandTile;
        [SerializeField]
        TerrainTile connectedSeedTile;

        [SerializeField]
        [HideInInspector]
        List<Tilemap> tilemaps = new();
        [SerializeField]
        [HideInInspector]
        Dictionary<TileLayerType, Tilemap> tilemapLayerMaps = new();

        [SerializeField]
        [HideInInspector]
        private List<TileChangeData> tileChangeDataList;

        Tilemap GroundTileMap => tilemapLayerMaps[TileLayerType.Ground];

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            gridLayout.GetComponentsInChildren(tilemaps);

            foreach (var tilemap in tilemaps)
            {
                base.BoxErase(gridLayout, tilemap.gameObject, position);
            }
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null)
                return;

            gridLayout.GetComponentsInChildren(tilemaps);

            tilemapLayerMaps.Clear();
            foreach (var map in tilemaps)
            {
                if (Enum.TryParse<TileLayerType>(map.name, out var mapType))
                    tilemapLayerMaps.Add(mapType, map);
            }

            if (tilemapLayerMaps.Count == 0)
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

                data.tile = connectedSeedTile;

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
                        SetTile(ref data, connectedSeedTile.TileLayerType);

                    }
                }

            }
        }
        void SetTile(ref TileChangeData data, TileLayerType paintLayer)
        {
            var obj = GroundTileMap.GetInstantiatedObject(data.position);
            if (obj != null && obj.TryGetComponent<ConnectedSand>(out var connectedSand) && connectedSand.Target != null)
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

            tilemapLayerMaps[paintLayer].SetTile(data, false);

            var tile = data.tile;
            data.tile = null;

            foreach (var layer in tilemapLayerMaps.Keys)
            {
                if (layer != paintLayer)
                    tilemapLayerMaps[layer].SetTile(data, false);
            }

            data.tile = tile;
        }

#if UNITY_EDITOR

        [MenuItem("Assets/Create/Tile/Brush/LayerFixedBrush")]
        public static void CreateLayerFixedBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save LayerFixedBrush", "New LayerFixedBrush", "Asset", "Save LayerFixedBrush");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LayerFixedBrush>(), path);
        }
#endif

        [CustomEditor(typeof(LayerFixedBrush))]
        public class LayerFixedBrushEditor : GridBrushEditor
        {
            public override bool canChangeZPosition => false;
        }
    }
}
