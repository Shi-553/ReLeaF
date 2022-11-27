using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class AllGreening : MonoBehaviour
    {
        [SerializeField, Rename("ŽŸ‚Ìƒ}ƒX‚ð—Î‰»‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
        float greeningTime = 0.1f;

        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        CinemachineTargetGroup targetGroup;


        bool isStartGreening = false;
        bool useCamera = false;

        Coroutine co;

        void Start()
        {
            TryGetComponent(out targetGroup);
        }

        public IEnumerator StartGreening(Vector2Int tilePos, bool useCamera, bool isFouce = false)
        {
            if (isStartGreening)
            {
                if (isFouce)
                {
                    isStartGreening = false;
                    if (co != null)
                        StopCoroutine(co);

                }
                else
                    yield break;
            }
            isStartGreening = true;

            this.useCamera = useCamera;
            if (useCamera)
            {
                virtualCamera.LookAt = transform;
                virtualCamera.Follow = transform;
            }
            co = StartCoroutine(Greening(tilePos));
            yield return co;
        }
        public IEnumerator StartGreeningWithPlayer()
        {
            if (isStartGreening)
                yield break;

            isStartGreening = true;

            var player = FindObjectOfType<PlayerMover>();
            targetGroup.AddMember(player.transform, 1, 1);

            yield return StartGreening(player.TilePos, true);
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

            int greeningCount = 0;

            while (targetCount > 0)
            {
                int bufferIndex = 0;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];

                    if (!greenMap.TryAdd(pos, true))
                    {
                        continue;
                    }
                    if (DungeonManager.Singleton.TryGetTile(pos, out var tile))
                    {
                        if (useCamera)
                        {
                            if (targetGroup.m_Targets.Length > 100)
                            {
                                targetGroup.m_Targets[targetGroupIndex].target = tile.transform;
                                targetGroupIndex = (targetGroupIndex + 1) % 99;
                            }
                            else
                            {
                                targetGroup.AddMember(tile.transform, 1, 1);
                            }
                        }

                        // —Î‰»‚Å‚«‚é
                        if (tile.TileType == TileType.Sand || tile.TileType == TileType.Messy)
                        {
                            DungeonManager.Singleton.SowSeed(pos, PlantType.Foundation, true);
                        }
                    }
                    else
                    {
                        if (greeningCount > 10)
                            continue;
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

                greeningCount++;
                targetCount = bufferIndex;
                (buffer, target) = (target, buffer);

                yield return greeningWait;
            }
            co = null;
        }
    }
}
