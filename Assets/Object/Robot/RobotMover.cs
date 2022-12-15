using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class RobotMover : SingletonBase<RobotMover>
    {
        public override bool DontDestroyOnLoad => false;
        [SerializeField, Rename("スピード(nマス/秒)")]
        float speed = 8.0f;
        public float Speed => UseManualOperation ? manualSpeed : speed;

        [SerializeField, Rename("それ以上近づかない距離(nマス)")]
        float nearRange = 1.0f;

        public Vector2 Move { get; set; }
        public bool IsMove => Move != Vector2.zero;
        public bool IsLeft { get; private set; }
        public bool IsDash { get; set; }

        Rigidbody2DMover mover;

        public bool UseManualOperation { get; private set; } = false;

        bool canUseDash = true;
        float manualSpeed;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            TryGetComponent(out mover);
        }

        Vector3 target;
        Vector3 toTarget;
        Vector3 dir;
        float distance;
        void UpdateTarget(Vector3 target)
        {
            this.target = target;
            toTarget = target - transform.position;
            distance = toTarget.magnitude;
            dir = toTarget / distance;
        }

        public void UpdateManualOperation(Vector3 target, float speed, bool useDash)
        {
            if (speed == 0)
                return;
            UpdateTarget(target);
            UseManualOperation = true;
            this.canUseDash = useDash;
            manualSpeed = speed;
        }

        void LateUpdate()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;

            if (!UseManualOperation)
            {
                UpdateTarget(PlayerCore.Singleton.transform.position);

                if (distance * DungeonManager.CELL_SIZE < nearRange)
                {
                    if (PlayerCore.Singleton.Mover.Dir != Vector2.zero)
                    {
                        var dot = Vector2.Dot(PlayerCore.Singleton.Mover.Dir, -dir);
                        var cross = PlayerCore.Singleton.Mover.Dir.Cross(-dir);
                        if (dot > 0)
                        {
                            if (cross < 0)
                                cross = Mathf.Min(cross, -0.5f);
                            else
                                cross = Mathf.Max(cross, 0.5f);
                        }
                        dir = Quaternion.Euler(0, 0, 30 * cross) * dir;
                    }
                    UpdateTarget(target - dir * nearRange);
                }
            }
            else
            {
                UpdateTarget(target);
            }

            var distanceMaxOne = Mathf.Min(distance, 1);
            if (distanceMaxOne > 0.1f)
            {
                Move = DungeonManager.CELL_SIZE * distanceMaxOne * Speed * dir;
                mover.MoveDelta(Move);
            }
            else
            {
                UseManualOperation = false;
                canUseDash = true;
            }

            if (Move.x != 0)
                IsLeft = Move.x < 0;

            IsDash = canUseDash && distanceMaxOne == 1;
        }
    }
}
