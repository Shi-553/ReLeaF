using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class CrabAttacker : MonoBehaviour, IEnemyAttacker
    {
        public AttackTransition Transition { get; set; }
        public EnemyAttackInfo EnemyAttackInfo => crabAttackInfo;

        [SerializeField]
        CrabAttackInfo crabAttackInfo;


        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool isDamagableOnly)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            throw new System.NotImplementedException();
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}
