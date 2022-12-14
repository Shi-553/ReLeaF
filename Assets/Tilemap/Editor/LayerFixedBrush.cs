using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private void OnEnable()
        {
            GridPaintingState.scenePaintTargetChanged += _ => GridPaintingState.gridBrush = this;
        }

        int angleDegree;

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, position, pickStart);
            angleDegree = 0;
            UpdateAngle();
        }



        [SerializeField]
        [HideInInspector]
        private List<TileChangeData> tileChangeDataList;



        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null)
                return;

            if (!brushTarget.TryGetComponent<Tilemap>(out var map))
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

                if (data.tile is not ILayerFixedTile layerFixedTile)
                {
                    continue;
                }

                if (layerFixedTile is not ISizeableTile paddingTile)
                {
                    SetTile(map, ref data);
                    continue;
                }

                SetTile(map, ref data);

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
                        SetTile(map, ref data);
                    }
                }
            }
        }
        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null)
                return;

            if (!brushTarget.TryGetComponent<Tilemap>(out var map))
                return;

            var identity = Matrix4x4.identity;
            var listSize = Math.Abs(position.size.x * position.size.y * position.size.z);
            if (tileChangeDataList == null || tileChangeDataList.Capacity != listSize)
                tileChangeDataList = new List<TileChangeData>(listSize);
            tileChangeDataList.Clear();
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                tileChangeDataList.Add(new TileChangeData { position = location, tile = null, transform = identity, color = Color.white });
            }

            for (int i = 0; i < tileChangeDataList.Count; i++)
            {
                TileChangeData data = tileChangeDataList[i];
                SetTile(map, ref data);
            }
        }

        void FillSizeableTile(Tilemap tilemap, ref TileChangeData data)
        {
            var obj = tilemap.GetInstantiatedObject(data.position);
            if (obj == null)
            {
                return;
            }
            var tile = obj.GetComponentsInChildren<TileObject>(true).FirstOrDefault(t => t.HasParent);

            if (tile == null || !tile.HasParent || tile.Parent.CreatedTile is not ISizeableTile sizeableTile)
            {
                return;
            }
            var targetPos = tile.Parent.TilePos;
            for (int x = 0; x < sizeableTile.Size.x; x++)
            {
                for (int y = 0; y < sizeableTile.Size.y; y++)
                {
                    var pos = new Vector3Int(targetPos.x + x, targetPos.y + y);
                    if (pos.x == data.position.x && pos.y == data.position.y)
                    {
                        continue;
                    }
                    tilemap.SetTile(pos, sandTile);
                }
            }
        }
        void SetTile(Tilemap tilemap, ref TileChangeData data)
        {
            FillSizeableTile(tilemap, ref data);
            tilemap.SetTile(data, false);
        }

        void ClearOverlappedTile()
        {
            var tilemap = Object.FindObjectOfType<Tilemap>();

            Undo.RecordObject(tilemap, "ClearOverlappedTile");

            HashSet<Vector3Int> allTiles = new();

            List<Vector3Int> posList = new();

            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos);
                if (tile == null)
                {
                    continue;
                }
                if (pos.z != 0 || !allTiles.Add(pos))
                {
                    posList.Add(pos);
                    continue;
                }
            }

            tilemap.SetTiles(posList.ToArray(), new TileBase[posList.Count]);
            tilemap.gameObject.SetActive(false);
            tilemap.gameObject.SetActive(true);
            EditorSceneManager.MarkSceneDirty(tilemap.gameObject.scene);
        }

        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            if (direction == RotationDirection.Clockwise)
            {
                angleDegree += 90;
            }
            else
            {
                angleDegree -= 90;
            }

            angleDegree += 360;
            angleDegree %= 360;
            UpdateAngle();
        }

        void UpdateAngle()
        {
            foreach (BrushCell cell in cells)
            {
                if (cell.tile is not SandPaddingTile sandPaddingTile)
                {
                    if (cell.tile is not RotatedSandPaddingTile rotatedSandPadding)
                        continue;
                    sandPaddingTile = rotatedSandPadding.tile;
                }
                var sandPaddingTilePath = AssetDatabase.GetAssetPath(sandPaddingTile);

                var parentPath = Path.Combine(Path.GetDirectoryName(sandPaddingTilePath), $"Rotated");

                var parentFullPath = Path.GetFullPath(parentPath);
                if (!Directory.Exists(parentFullPath))
                {
                    Directory.CreateDirectory(parentFullPath);
                    AssetDatabase.Refresh();
                }

                var rotatedPath = Path.Combine(parentPath, $"{sandPaddingTile.name}_{angleDegree}.asset");

                var asset = AssetDatabase.LoadAssetAtPath<RotatedSandPaddingTile>(rotatedPath);
                if (asset != null)
                {
                    cell.tile = asset;
                    continue;
                }

                var instanced = RotatedSandPaddingTile.CreateRotatedInstance(angleDegree);
                instanced.tile = sandPaddingTile;
                AssetDatabase.CreateAsset(instanced, rotatedPath);
                cell.tile = instanced;
                AssetDatabase.Refresh();
            }
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

                if (GUILayout.Button("?d?????????^?C?????N???A   ?i?????????????????????????????????????j"))
                {
                    brush.ClearOverlappedTile();
                }

            }
        }
    }
}
