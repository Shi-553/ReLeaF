using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("SeaUrchinÇÃçUåÇÉpÉâÉÅÅ[É^")]
    [CreateAssetMenu(menuName = "Enemy/SeaUrchinAttackInfo")]
    class SeaUrchinAttackInfo : EnemyAttackInfo
    {
        //[SerializeField]

    }
    public class SeaUrchinAttacker : MonoBehaviour, IEnemyAttacker
    {
        EnemyMover mover;
        List<Vector2Int> buffer = new();
        public AttackTransition Transition { get; set; }

        public EnemyAttackInfo EnemyAttackInfo { get; }

        public IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool isDamagableOnly)
        {
            mover.GetCheckPoss(pos, dir, buffer);
            return buffer;
        }

        IEnumerator IEnemyAttacker.OnStartDamageing()
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {
            TryGetComponent(out mover);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
