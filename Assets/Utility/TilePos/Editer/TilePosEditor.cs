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
        Vector2Int centerTileSize = Vector2Int.one;

        Vector2Int lineCount;
        Vector2Int centerTilePos;
        Vector2 centerPos;
        Vector2 gridOffset;
        Vector2 gridOffsetLimited;

        Vector2 dragOffset;
        Direction centerDirection;

        int boldGridDistance = 5;

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



        static public void Open(SerializedProperty tiles, Direction centerDirection)
        {
            var window = GetWindow<TilePosEditor>();
            window.titleContent = new GUIContent("TilePosEditor");
            if (!window.TrySetTiles(tiles))
            {
                Debug.LogWarning("Array Not Found.");
                return;
            }
            window.centerDirection = centerDirection;
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
                    if (TrySetTiles(serializedProperty))
                    {
                        centerDirection = att.direction;
                        Repaint();
                        return;
                    }
                }
            }
        }

        void SetXBoldGridColor(float x)
        {
            x += 1;
            var tilePos = ScreenPosToTilePos(new Vector2(x, 0));
            var pos = tilePos.x;
            if ((pos < 0 && 0 == (pos % boldGridDistance)) || (pos > 0 && 0 == ((pos - centerTileSize.x) % boldGridDistance)) || pos == 0)
            {
                Handles.color = Color.white;
            }
            else
                Handles.color = Color.gray;
        }
        void SetYBoldGridColor(float y)
        {
            y += 1;
            var tilePos = ScreenPosToTilePos(new Vector2(0, y));
            var pos = tilePos.y;
            if ((pos > 0 && 0 == ((pos - centerTileSize.y + 1) % boldGridDistance)) || (pos < 0 && 0 == ((pos + 1) % boldGridDistance)) || pos == centerTileSize.y - 1)
            {
                Handles.color = Color.white;
            }
            else
                Handles.color = Color.gray;
        }

        void DrawGrid()
        {
            Handles.color = Color.gray;

            for (int i = 0; i <= lineCount.x; i++)
            {
                var x = i * cellSize + gridOffsetLimited.x;
                SetXBoldGridColor(x);
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, position.height));
            }
            for (int i = 0; i <= lineCount.y; i++)
            {
                var y = i * cellSize + gridOffsetLimited.y;
                SetYBoldGridColor(y);
                Handles.DrawLine(new Vector3(0, y), new Vector3(position.width, y));
            }
        }
        void DrawCenter()
        {

            Handles.color = Color.white;

            for (int x = 0; x < centerTileSize.x; x++)
            {
                for (int y = 0; y < centerTileSize.y; y++)
                {
                    var centerPos = new Vector2Int(x, y);
                    var pos = TilePosToScreenPos(centerPos);

                    if (centerDirection == Direction.NONE)
                    {
                        Handles.DrawSolidDisc(pos, Vector3.forward, cellSize / 2);
                        continue;
                    }

                    var cellHeight = new Vector2(0, cellSize / 2);
                    var cellWidth = new Vector2(cellSize / 2, 0);

                    if (centerDirection == Direction.UP || centerDirection == Direction.DOWN)
                    {
                        Handles.DrawLine(pos + cellHeight, pos - cellHeight);
                    }
                    else
                    {
                        Handles.DrawLine(pos + cellWidth, pos - cellWidth);
                    }

                    switch (centerDirection)
                    {
                        case Direction.UP:
                            Handles.DrawLine(pos - cellHeight, pos + cellWidth / 2);
                            Handles.DrawLine(pos - cellHeight, pos - cellWidth / 2);
                            break;
                        case Direction.LEFT:
                            Handles.DrawLine(pos - cellWidth, pos + cellHeight / 2);
                            Handles.DrawLine(pos - cellWidth, pos - cellHeight / 2);
                            break;
                        case Direction.DOWN:
                            Handles.DrawLine(pos + cellHeight, pos + cellWidth / 2);
                            Handles.DrawLine(pos + cellHeight, pos - cellWidth / 2);
                            break;
                        case Direction.RIGHT:
                            Handles.DrawLine(pos + cellWidth, pos + cellHeight / 2);
                            Handles.DrawLine(pos + cellWidth, pos - cellHeight / 2);
                            break;
                    }
                }
            }
        }
        void DrawTiles()
        {

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
            if (MathExtension.DuringExists(target, Vector2Int.zero, centerTileSize))
                return false;

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
            var guiArea = new Rect(10, 10, 200, 100);

            lineCount.x = Mathf.CeilToInt(position.width / cellSize);
            lineCount.y = Mathf.CeilToInt(position.height / cellSize);

            UpdateCenter();

            DrawCenter();

            if (tiles == null)
            {
                return;
            }

            try
            {
                DrawTiles();

                var e = Event.current;
                var mousePos = Event.current.mousePosition;
                if (!guiArea.Contains(mousePos))
                {
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
            }
            catch
            {
                // null判定できない・・・
            }
            DrawGrid();


            GUILayout.BeginArea(guiArea);
            centerTileSize = EditorGUILayout.Vector2IntField("中心サイズ", centerTileSize, GUILayout.MaxWidth(200));
            GUILayout.EndArea();
        }
    }
}
