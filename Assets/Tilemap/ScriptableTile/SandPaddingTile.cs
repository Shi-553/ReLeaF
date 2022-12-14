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

    public class SandPaddingTile : SelectTile
    {
        [SerializeField]
        Vector2Int size = Vector2Int.one;
        [SerializeField]
        bool isPreview;
        [SerializeField]
        bool isScaleWithSize;
        [SerializeField]
        Vector2 previewOffset;


        public virtual Vector2Int Size => size;

        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
#if UNITY_EDITOR

            if (!Application.isPlaying && go != null && tilemap == null)
            {
                foreach (var map in tm.GetComponent<Transform>().parent.GetComponentsInChildren<Tilemap>())
                {
                    if (Enum.TryParse<TileLayerType>(map.name, out var mapType) && mapType == TileLayerType.Ground)
                    {
                        tilemap = map;
                        break;
                    }
                }
            }
#endif
            var result = base.StartUp(position, tm, go);
            if (result)
            {
                GlobalCoroutine.Singleton.StartCoroutine(Connect((Vector2Int)position, createdObject));
            }

#if UNITY_EDITOR

            if (!Application.isPlaying && go != null)
            {

                if (createdObject != null)
                {
                    EditorCoroutineUtility.StartCoroutine(ConnectInEditor(tilemap, position, createdObject), this);
                }

            }

#endif
            return result;
        }
        IEnumerator Connect(Vector2Int position, TileObject obj)
        {
            yield return null;
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if (DungeonManager.Singleton.TryGetTile<Sand>(new Vector2Int(position.x + x, position.y + y), out var connectedSand))
                    {
                        connectedSand.Target = obj;
                    }
                }
            }
        }
#if UNITY_EDITOR
        IEnumerator ConnectInEditor(Tilemap tm, Vector3Int position, TileObject tile)
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
                        var connectedSand = obj.GetComponentInChildren<Sand>();
                        if (connectedSand != null)
                            connectedSand.Target = tile;
                    }
                }
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            if (!Application.isPlaying && isPreview)
            {
                tileData.sprite = m_Sprite;

                var tm = tilemap.GetComponent<Tilemap>();

                if (!tm.gameObject.CompareTag("EditorOnly") && (currentTileObject != null))
                {
                    var pos = tm.CellToWorld((Vector3Int)(Size + new Vector2Int(-1, -1))) / 2.0f + (Vector3)previewOffset;
                    tileData.transform = Matrix4x4.TRS(pos, Quaternion.identity, isScaleWithSize ? new Vector3(Size.x, Size.y) : Vector3.one);

                    tileData.flags = TileFlags.LockTransform;
                }
            }
        }

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
}
