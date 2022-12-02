using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SeaUrchinController : MonoBehaviour
    {
        IEnemyAttacker enemyAttacker;
        EnemyMover mover;
        void Start()
        {
            TryGetComponent(out enemyAttacker);
            TryGetComponent(out mover);
            mover.Dir = Vector2Int.right;
        }

        void Update()
        {
            if (!GameRuleManager.Singleton.IsPlaying)
                return;
            if (enemyAttacker.IsAttack)
                return;


            mover.Dir = (Quaternion.Euler(0, 0, 90 + mover.Dir.GetRotationZ()) * Vector2.up).ClampOneMagnitude();
            StartCoroutine(enemyAttacker.Attack());
        }
    }
}
