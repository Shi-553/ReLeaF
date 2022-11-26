using Cinemachine;
using DebugLogExtension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class AllGreening : MonoBehaviour
    {
        [SerializeField, Rename("ŽŸ‚Ìƒ}ƒX‚ð—Î‰»‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
        float greeningTime = 0.1f;

        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        CinemachineTargetGroup targetGroup;

        [SerializeField]
        GameObject gameclearText;

        bool isStartGreening = false;

        void Start()
        {
            TryGetComponent(out targetGroup);
        }

        public void StartGreening(Vector2Int pos)
        {
            if (isStartGreening)
                return;

            isStartGreening = true;

            virtualCamera.LookAt = transform;
            virtualCamera.Follow = transform;
            StartCoroutine(Greening(pos));
        }

        IEnumerator Greening(Vector2Int startPos)
        {
            Dictionary<Vector2Int, bool> greenMap = new();

            List<Vector2Int> list1 = new(100);
            List<Vector2Int> list2 = new(100);

            var target = list1;
            var buffer = list2;

            target.Add(startPos);

            int targetCount = target.Count;
            int targetGroupIndex = 0;

            var greeningWait = new WaitForSeconds(greeningTime);

            while (targetCount > 0)
            {
                int bufferIndex = 0;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];

                    if (!greenMap.TryAdd(pos, true) || !DungeonManager.Instance.TryGetTile(pos, out var tile))
                    {
                        continue;
                    }

                    if (targetGroup.m_Targets.Length > 100)
                    {
                        targetGroup.m_Targets[targetGroupIndex].target = tile.transform;
                        targetGroupIndex = (targetGroupIndex + 1) % 100;
                    }
                    else
                    {
                        targetGroup.AddMember(tile.transform, 1, 1);
                    }

                    // —Î‰»‚Å‚«‚é
                    if (tile.TileType == TileType.Sand || tile.TileType == TileType.Messy)
                    {
                        DungeonManager.Instance.SowSeed(pos, PlantType.Foundation, true);
                    }


                    void BufferAdd(Vector2Int nextPos)
                    {
                        if (buffer.Count == bufferIndex)
                            buffer.Add(nextPos);
                        else
                            buffer[bufferIndex] = nextPos;

                        bufferIndex++;
                    }

                    BufferAdd(pos + Vector2Int.up);
                    BufferAdd(pos + Vector2Int.down);
                    BufferAdd(pos + Vector2Int.right);
                    BufferAdd(pos + Vector2Int.left);
                }

                targetCount = bufferIndex;
                (buffer, target) = (target, buffer);

                yield return greeningWait;
            }

            gameclearText.SetActive(true);
        }
    }
}
