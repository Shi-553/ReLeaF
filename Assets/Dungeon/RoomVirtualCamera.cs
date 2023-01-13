using Cinemachine;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class RoomVirtualCamera : SingletonBase<RoomVirtualCamera>
    {
        CinemachineVirtualCamera virtualCamera;
        CinemachineTargetGroup cinemachineTarget;
        Transform minTrans, maxTrans;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;

            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineTarget = GetComponentInChildren<CinemachineTargetGroup>();
            minTrans = CreateTarget();
            maxTrans = CreateTarget();
            cinemachineTarget.AddMember(minTrans, 1, 1);
            cinemachineTarget.AddMember(maxTrans, 1, 1);
        }
        Transform CreateTarget()
        {
            var target = new GameObject("target").transform;
            target.parent = transform;
            return target;
        }
        public void BeginRoomBlast(Vector3 min, Vector3 max)
        {
            minTrans.position = min;
            maxTrans.position = max;
            virtualCamera.Priority = 20;
        }

        public void EndRoomBlast()
        {
            virtualCamera.Priority = 5;

        }
    }
}
