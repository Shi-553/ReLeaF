using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class RobotMover : SingletonBase<RobotMover>
    {
        public override bool DontDestroyOnLoad => false;
        [SerializeField, Rename("スピード(nマス/秒)")]
        float speed = 8.0f;
        [SerializeField, Rename("それ以上近づかない距離(nマス)")]
        float nearRange = 1.0f;

        public Vector2 Move { get; set; }
        public bool IsMove => Move != Vector2.zero;
        public bool IsLeft { get; private set; }
        public bool IsDash { get; set; }

        Rigidbody2DMover mover;

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


        void LateUpdate()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;

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

            if (distance > 0.01f)
            {
                Move = DungeonManager.CELL_SIZE * Mathf.Min(distance, 1) * speed * dir;
                mover.MoveDelta(Move);
            }

        }
    }
}
