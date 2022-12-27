using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class AllGreening : SingletonBase<AllGreening>
    {
        [SerializeField, Rename("���̃}�X��Ή�����܂ł̎���")]
        float greeningTime = 0.1f;

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

            var player = FindObjectOfType<PlayerMover>();
            cinemachineTargetGroup.AddMember(player.transform, 1, 1);

            yield return StartGreening(player.TilePos, true);
        }

        IEnumerator Greening(Vector2Int startPos)
        {
            // ���ׂ̒Ⴂ�����ɂ����悻�m�ۂ��Ă���
            var worldStartPos = DungeonManager.Singleton.TilePosToWorld(startPos);

            Dictionary<Vector2Int, bool> greenMap = new(500);

            var target = new List<Vector2Int>(100);
            var buffer = new List<Vector2Int>(100);

            target.Add(startPos);

            int targetCount = target.Count;

            int cinemachineTargetGroupIndex = 0;

            var greeningWait = new WaitForSeconds(greeningTime);

            // �Ή������������@�Ⴂ�����͉��������Ă����̃}�X��T��������
            int tryGreeningCount = 0;

            while (targetCount > 0)
            {
                int bufferIndex = 0;

                for (int i = 0; i < targetCount; i++)
                {
                    Vector2Int pos = target[i];

                    tryGreeningCount++;

                    if (!greenMap.TryAdd(pos, true))
                    {
                        continue;
                    }

                    // �Ή�
                    DungeonManager.Singleton.SowSeed(pos, true, true);

                    //�Ή��֌W�Ȃ��^�C�����擾
                    if (DungeonManager.Singleton.TryGetTile(pos, out var tile))
                    {
                        if (useCamera && tile.CanOrAleeadyGreening(true))
                        {
                            if (cinemachineTargetGroup.m_Targets.Length > 100)
                            {
                                // �ő�100��
                                for (int j = 0; j < 100; j++)
                                {
                                    var currentDir = (Vector2)cinemachineTargetGroup.m_Targets[2 + cinemachineTargetGroupIndex].target.position - worldStartPos;
                                    var overrideDir = (Vector2)tile.transform.position - worldStartPos;

                                    if (Mathf.Sign(currentDir.x) == Mathf.Sign(overrideDir.x) &&
                                        Mathf.Sign(currentDir.y) == Mathf.Sign(overrideDir.y))
                                    {
                                        // �Ή����n�߂��n�_�ƃv���C���[���J�����Ɏc�������̂ŁA98�Ő܂�Ԃ�
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
                        //�^�C�����Ȃ��A10��ȏ�Ή����悤�Ƃ��Ă�����continue
                        if (tryGreeningCount > 10)
                            continue;
                    }

                    // ���ɑ��݂���Ƃ��ɏ㏑�����邱�Ƃ�Clear()�����Ȃ��Ă悭�Ȃ�œK��
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

                // �^�[�Q�b�g�ƃo�b�t�@���X���b�v�����[�v���邱�ƂŃ������ߖ�
                (buffer, target) = (target, buffer);

                yield return greeningWait;
            }

            co = null;
        }

    }
}
