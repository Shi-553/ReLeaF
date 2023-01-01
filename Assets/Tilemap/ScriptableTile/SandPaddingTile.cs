using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public interface ISizeableTile
    {
        public Vector2Int Size { get; }

    }
    public class SandPaddingTile : SelectTile
    {
        [SerializeField]
        public Vector2Int size = Vector2Int.one;
        [SerializeField]
        public bool isPreview;
        [SerializeField]
        public bool isScaleWithSize;
        [SerializeField]
        public Vector2 previewOffset;

        public virtual Vector2Int Size => size;


#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tile/SandPaddingTile")]
        public static void CreateSandPaddingTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save SandPaddingTile", "New SandPaddingTile", "Asset", "Save SandPaddingTile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SandPaddingTile>(), path);
        }

#endif
    }
    public abstract class RotatedSandPaddingTile : TileBase, ISizeableTile, ILayerFixedTile
    {
        public static RotatedSandPaddingTile CreateRotatedInstance(int angle)
        {
            return angle switch
            {
                0 => CreateInstance<Rotated0Tile>(),
                90 => CreateInstance<Rotated90Tile>(),
                180 => CreateInstance<Rotated180Tile>(),
                270 => CreateInstance<Rotated270Tile>(),
                _ => throw new ArgumentException("not suppert"),
            };
        }
        public SandPaddingTile tile;
        public Vector2Int Size => IsSizeReverseXY ? new Vector2Int(tile.Size.y, tile.Size.x) : tile.Size;

        public abstract int Angle { get; }
        public abstract bool IsSizeReverseXY { get; }

        Quaternion Rotation => Quaternion.Euler(0, 0, Angle);
        public TileLayerType TileLayerType => tile.TileLayerType;


        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
            var result = tile.StartUp(position, tm, go);
            if (result)
            {
                GlobalCoroutine.Singleton.StartCoroutine(Connect((Vector2Int)position, tile.createdObject));
            }

            if (tile.createdObject is IRotateable rotateable)
            {
                rotateable.Rotate(Rotation, (Vector2)(Size - Vector2Int.one) * 0.25f);
            }
            tile.createdObject.CreatedTile = this;
            tile.createdObject.InstancingTarget.CreatedTile = this;

#if UNITY_EDITOR

            if (!Application.isPlaying && go != null)
            {

                if (tile.createdObject != null)
                {
                    EditorCoroutineUtility.StartCoroutine(ConnectInEditor(tm.GetComponent<Tilemap>(), position, tile.createdObject), this);
                }

            }

#endif
            return result;
        }
        IEnumerator Connect(Vector2Int position, TileObject parent)
        {
            yield return null;
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if (DungeonManager.Singleton.TryGetTile(new Vector2Int(position.x + x, position.y + y), out var tile))
                    {
                        tile.Parent = parent;
                    }
                }
            }
        }
#if UNITY_EDITOR
        IEnumerator ConnectInEditor(Tilemap tm, Vector3Int position, TileObject parent)
        {
            yield return null;
            if (tm == null)
                yield break;

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    var obj = tm.GetInstantiatedObject(new Vector3Int(position.x + x, position.y + y));
                    if (obj != null)
                    {
                        foreach (var tile in obj.GetComponentsInChildren<TileObject>(true))
                        {
                            if (tile != parent)
                                tile.Parent = parent;
                        }
                    }
                }
            }
        }
#endif

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tile.GetTileData(position, tilemap, ref tileData);

            if (!Application.isPlaying && tile.isPreview)
            {
                tileData.sprite = tile.m_Sprite;

                var tm = tilemap.GetComponent<Tilemap>();

                if ((tile.CurrentTileObject != null))
                {
                    var pos = tm.CellToWorld((Vector3Int)(Size + new Vector2Int(-1, -1))) / 2.0f + (Vector3)tile.previewOffset;
                    tileData.transform = Matrix4x4.TRS(pos, Rotation, tile.isScaleWithSize ? new Vector3(tile.Size.x, tile.Size.y) : Vector3.one);
                }
            }
            tileData.flags = TileFlags.LockTransform;
        }
    }
}
