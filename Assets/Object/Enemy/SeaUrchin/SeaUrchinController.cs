using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SeaUrchinController : MonoBehaviour
    {
        EnemyAttacker enemyAttacker;
        EnemyMover mover;
        EnemyCore core;
        void Start()
        {
            TryGetComponent(out enemyAttacker);
            TryGetComponent(out mover);
            TryGetComponent(out core);
            mover.DirNotZero = Vector2Int.left;
        }

        void Update()
        {
            if (!GameRuleManager.Singleton.IsPlaying || !core.IsValid)
                return;
            if (enemyAttacker.IsAttack)
                return;


            mover.DirNotZero = (Quaternion.Euler(0, 0, -90 + mover.DirNotZero.GetRotationZ()) * Vector2.up).ClampOneMagnitude();
            enemyAttacker.Attack();
        }
    }
}
