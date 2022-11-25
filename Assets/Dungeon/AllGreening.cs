using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace ReLeaf
{
    public class AllGreening : MonoBehaviour
    {
        [SerializeField, Rename("ŽŸ‚Ìƒ}ƒX‚ð—Î‰»‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
        float greeningTime = 0.1f;
        WaitForSeconds greeningWait;
        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        CinemachineTargetGroup targetGroup;

        Vector2Int startPos;
        Dictionary<Vector2Int, bool> greenMap=new();

        void Start()
        {
            greeningWait = new WaitForSeconds(greeningTime);
            TryGetComponent(out targetGroup);
        }

        public void StartGreening(Vector2Int pos)
        {
            greenMap.Clear();
            startPos = pos;

           // virtualCamera.LookAt = transform;
            //virtualCamera.Follow = transform;
            StartCoroutine(Greening(pos));
        }
        IEnumerator Greening(Vector2Int pos)
        {
            if (greenMap.ContainsKey(pos)||!DungeonManager.Instance.TryGetTile(pos, out var tile))
            {
                yield break;
            }

           // targetGroup.AddMember(tile.transform, 1, 1);

            // —Î‰»‚Å‚«‚é
            if (tile.TileType == TileType.Sand || tile.TileType == TileType.Messy)
            {
                DungeonManager.Instance.SowSeed(pos, PlantType.Foundation, true);
            }
            greenMap.Add(pos, true);

            yield return greeningWait;

            StartCoroutine(Greening(pos + Vector2Int.up));
            StartCoroutine(Greening(pos + Vector2Int.down));
            StartCoroutine(Greening(pos + Vector2Int.left));
            StartCoroutine(Greening(pos + Vector2Int.right));
        }
    }
}
