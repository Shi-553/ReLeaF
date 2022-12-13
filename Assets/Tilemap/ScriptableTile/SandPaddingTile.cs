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
        Vector2Int size;
        [SerializeField]
        bool isPreview;
        [SerializeField]
        Vector2 previewOffset;

        public virtual Vector2Int Size => size;

        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
            var result = base.StartUp(position, tm, go);
            if (result)
            {
                GlobalCoroutine.Singleton.StartCoroutine(Connect((Vector2Int)position, createdObject));
            }

#if UNITY_EDITOR

            if (!Application.isPlaying && go != null)
            {
                var tileObject = go.GetComponentInChildren<TileObject>();

                if (tileObject != null)
                {
                    tileObject.TilePos = (Vector2Int)position;
                    EditorCoroutineUtility.StartCoroutine(ConnectInEditor(tm.GetComponent<Tilemap>(), position, tileObject), this);
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
                    if (x == 0 && y == 0) continue;

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
                    if (x == 0 && y == 0) continue;

                    var obj = tm.GetInstantiatedObject(new Vector3Int(position.x + x, position.y + y));
                    if (obj != null && obj.TryGetComponent<Sand>(out var connectedSand))
                    {
                        connectedSand.Target = tile;
                    }
                }
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            if (!Application.isPlaying)
            {
                tileData.sprite = m_Sprite;

                var tm = tilemap.GetComponent<Tilemap>();

                if (isPreview && !tm.gameObject.CompareTag("EditorOnly") && (currentTileObject != null))
                {
                    var pos = tm.CellToWorld((Vector3Int)(Size + new Vector2Int(-1, -2))) / 2.0f + (Vector3)previewOffset;
                    tileData.transform = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

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
