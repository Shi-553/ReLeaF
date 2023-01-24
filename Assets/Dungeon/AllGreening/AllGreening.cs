using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class AllGreening : SingletonBase<AllGreening>
    {
        [SerializeField]
        AllGreeningInfo info;

        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        CinemachineTargetGroup cinemachineTargetGroup;


        bool isStartGreening = false;
        bool useCamera = false;

        Coroutine co;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
        void Start()
        {
            isStartGreening = false;
            TryGetComponent(out cinemachineTargetGroup);
        }

        public IEnumerator StartGreening(Vector2Int tilePos, bool useCamera, bool isFouce = false)
        {

            yield return TileCulling.Singleton.StopCulling();

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
                virtualCamera.Priority = 30;
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

            var player = FindObjectOfType<PlayerMover>();
            cinemachineTargetGroup.AddMember(player.transform, 1, 1);

            yield return StartGreening(player.TilePos, true);
        }

        IEnumerator Greening(Vector2Int startPos)
        {
            // 負荷の低いうちにおおよそ確保しておく
            var worldStartPos = DungeonManager.Singleton.TilePosToWorld(startPos);

            HashSet<Vector2Int> greenMap = new(500);

            List<Vector2Int> target = new(100);
            List<Vector2Int> buffer = new(100);

            target.Add(startPos);

            int targetCount = target.Count;

            int cinemachineTargetGroupIndex = 0;

            var greeningWait = new WaitForSeconds(info.GreeningTime);

            // 緑化を試した数　低いうちは何があっても次のマスを探し続ける
            int tryGreeningCount = 0;

            while (targetCount > 0)
            {
                int bufferIndex = 0;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];

                    tryGreeningCount++;

                    if (!greenMap.Add(pos))
                    {
                        continue;
                    }

                    // 緑化
                    DungeonManager.Singleton.SowSeed(pos, true, true);

                    //緑化関係なくタイルを取得
                    if (DungeonManager.Singleton.TryGetTile(pos, out var tile))
                    {
                        if (useCamera && tile.CanOrAleadyGreening(true))
                        {
                            if (cinemachineTargetGroup.m_Targets.Length > 100)
                            {
                                // 最大100回
                                for (int j = 0; j < 100; j++)
                                {
                                    var currentDir = (Vector2)cinemachineTargetGroup.m_Targets[2 + cinemachineTargetGroupIndex].target.position - worldStartPos;
                                    var overrideDir = (Vector2)tile.transform.position - worldStartPos;

                                    if (Mathf.Sign(currentDir.x) == Mathf.Sign(overrideDir.x) &&
                                        Mathf.Sign(currentDir.y) == Mathf.Sign(overrideDir.y))
                                    {
                                        // 緑化を始めた地点とプレイヤーをカメラに残したいので、98で折り返す
                                        cinemachineTargetGroup.m_Targets[2 + cinemachineTargetGroupIndex].target = tile.transform;
                                        cinemachineTargetGroupIndex = (cinemachineTargetGroupIndex + 1) % 98;

                                        break;
                                    }
                                    cinemachineTargetGroupIndex = (cinemachineTargetGroupIndex + 1) % 98;
                                }
                            }
                            else
                            {
                                cinemachineTargetGroup.AddMember(tile.transform, 1, 1);
                            }
                        }
                    }
                    else
                    {
                        //タイルがなく、10回以上緑化しようとしていたらcontinue
                        if (tryGreeningCount > 10)
                            continue;
                    }

                    // 既に存在するときに上書きすることでClear()をしなくてよくなる最適化
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

                // ターゲットとバッファをスワップしつつループすることでメモリ節約
                (buffer, target) = (target, buffer);

                yield return greeningWait;
            }

            co = null;
        }

    }
}
