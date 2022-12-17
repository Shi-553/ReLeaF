using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SeaUrchinController : MonoBehaviour
    {
        EnemyAttacker enemyAttacker;
        EnemyMover mover;
        void Start()
        {
            TryGetComponent(out enemyAttacker);
            TryGetComponent(out mover);
            mover.DirNotZero = Vector2Int.left;
        }

        void Update()
        {
            if (!GameRuleManager.Singleton.IsPlaying)
                return;
            if (enemyAttacker.IsAttack)
                return;


            mover.DirNotZero = (Quaternion.Euler(0, 0, -90 + mover.DirNotZero.GetRotationZ()) * Vector2.up).ClampOneMagnitude();
            enemyAttacker.Attack();
        }
    }
}
