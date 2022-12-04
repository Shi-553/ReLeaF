using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public class TilePosEditor : EditorWindow
    {
        SerializedProperty tiles;
        bool TrySetTiles(SerializedProperty newTiles)
        {
            do
            {
                if (newTiles.isArray)
                {
                    tiles = newTiles;
                    return true;
                }

            } while (newTiles.NextVisible(true));
            return false;
        }

        float cellSize = 50;

        Vector2Int lineCount;
        Vector2Int centerTilePos;
        Vector2 centerPos;
        Vector2 gridOffset;
        Vector2 gridOffsetLimited;

        Vector2 dragOffset;

        void UpdateCenter()
        {
            centerTilePos = new Vector2Int((lineCount.x + 1) / 2, (lineCount.y + 1) / 2);
            centerPos = position.size / 2 + dragOffset;
            gridOffset = centerPos - new Vector2((centerTilePos.x - 0.5f) * cellSize, (centerTilePos.y - 0.5f) * cellSize);
            gridOffsetLimited.x = gridOffset.x % cellSize;
            gridOffsetLimited.y = gridOffset.y % cellSize;
        }

        Vector2 TilePosToScreenPos(Vector2Int tilePos)
        {
            tilePos.y = -tilePos.y;

            var screenPos = centerPos + ((Vector2)tilePos * cellSize);
            return screenPos;
        }

        Vector2Int ScreenPosToTilePos(Vector2 screenPos)
        {

            var tilePos = Vector2Int.CeilToInt((screenPos - gridOffset) / cellSize) - centerTilePos;
            tilePos.y = -tilePos.y;
            return tilePos;
        }



        static public void Open(SerializedProperty tiles)
        {
            var window = GetWindow<TilePosEditor>();
            window.titleContent = new GUIContent("TilePosEditor");
            if (!window.TrySetTiles(tiles))
            {
                Debug.LogWarning("Array Not Found.");
            }
        }


        void OnSelectionChange()
        {
            TryChangeTiles();
        }

        private void OnEnable()
        {
            TryChangeTiles();
        }

        void TryChangeTiles()
        {
            var activeObject = Selection.activeObject;

            if (activeObject == null)
                return;

            var serializedObject = new SerializedObject(activeObject);
            var serializedProperty = serializedObject.GetIterator();

            while (serializedProperty.Next(true))
            {
                var att = serializedProperty.GetAttributes<EditTilePosAttribute>(false);
                if (att != null)
                {
                    TrySetTiles(serializedProperty);
                    Repaint();
                    return;
                }
            }
        }



        void DrawGrid()
        {
            Handles.color = Color.gray;

            for (int i = 0; i <= lineCount.x; i++)
            {
                var x = i * cellSize + gridOffsetLimited.x;
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, position.height));
            }
            for (int i = 0; i <= lineCount.y; i++)
            {
                var y = i * cellSize + gridOffsetLimited.y;
                Handles.DrawLine(new Vector3(0, y), new Vector3(position.width, y));
            }
        }

        void DrawTiles()
        {
            Handles.color = Color.white;

            Handles.DrawSolidDisc(centerPos, Vector3.forward, cellSize / 2);

            for (int i = 0; i < tiles.arraySize; i++)
            {
                var tilePos = tiles.GetArrayElementAtIndex(i).vector2IntValue;

                var pos = TilePosToScreenPos(tilePos);
                pos.x -= cellSize / 2;
                pos.y -= cellSize / 2;
                var rect = new Rect(pos, new Vector2(cellSize, cellSize));
                Handles.DrawSolidRectangleWithOutline(rect, Color.gray, Color.white);
            }
        }



        bool TryGetTilePosIndex(Vector2Int target, out int index)
        {
            for (int i = 0; i < tiles.arraySize; i++)
            {
                var tilePos = tiles.GetArrayElementAtIndex(i).vector2IntValue;
                if (target == tilePos)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        bool TryDeleteTilePos(Vector2Int target)
        {
            if (TryGetTilePosIndex(target, out var index))
            {
                tiles.DeleteArrayElementAtIndex(index);
                return true;
            }
            return false;
        }

        bool TryAddTilePos(Vector2Int target)
        {
            if (!TryGetTilePosIndex(target, out var _))
            {
                var insertIndex = tiles.arraySize;
                tiles.InsertArrayElementAtIndex(insertIndex);
                tiles.GetArrayElementAtIndex(insertIndex).vector2IntValue = target;
                return true;
            }
            return false;
        }

        HashSet<Vector2Int> changedTiles = new();
        bool isAdd = false;
        void ChangeTile(Vector2 mousePos, bool isReflesh)
        {
            if (isReflesh)
            {
                changedTiles.Clear();
            }

            var clickedTilePos = ScreenPosToTilePos(mousePos);

            if (clickedTilePos == Vector2Int.zero)
                return;

            if (changedTiles.Contains(clickedTilePos))
                return;


            if (isReflesh)
            {
                isAdd = !TryDeleteTilePos(clickedTilePos);
                if (isAdd)
                {
                    TryAddTilePos(clickedTilePos);
                }
            }
            else
            {
                if (isAdd)
                    TryAddTilePos(clickedTilePos);
                else
                    TryDeleteTilePos(clickedTilePos);
            }


            changedTiles.Add(clickedTilePos);

            tiles.serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        void ChangeCellSize(float delta, Vector2 mousePos)
        {

            var oldCellSize = cellSize;
            cellSize = Mathf.Clamp(cellSize - delta, 10, 100);

            // アフィン変換？
            var deltaPos = mousePos - centerPos;
            deltaPos -= deltaPos * cellSize / oldCellSize;

            ChangeDragOffset(deltaPos);
        }

        void ChangeDragOffset(Vector2 delta)
        {
            dragOffset += delta;
            dragOffset.x = Mathf.Clamp(dragOffset.x, -10 * cellSize, 10 * cellSize);
            dragOffset.y = Mathf.Clamp(dragOffset.y, -10 * cellSize, 10 * cellSize);

            Repaint();
        }

        private void OnGUI()
        {
            Handles.BeginGUI();

            lineCount.x = Mathf.CeilToInt(position.width / cellSize);
            lineCount.y = Mathf.CeilToInt(position.height / cellSize);

            UpdateCenter();

            DrawGrid();

            if (tiles == null)
            {
                Handles.EndGUI();
                return;
            }

            try
            {
                DrawTiles();

                var e = Event.current;
                var mousePos = Event.current.mousePosition;

                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (e.button == 0)
                        {
                            ChangeTile(mousePos, true);
                        }
                        break;

                    case EventType.MouseDrag:
                        if (e.button == 0)
                        {
                            ChangeTile(mousePos, false);
                        }
                        if (e.button == 2)
                        {
                            ChangeDragOffset(e.delta * 1.2f);
                        }
                        break;

                    case EventType.ScrollWheel:
                        ChangeCellSize(e.delta.y * 1.2f, mousePos);
                        break;

                }
            }
            catch
            {
                // null判定できない・・・
            }
            Handles.EndGUI();
        }
    }
}
