using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/EnemyMoverInfo")]
    public class EnemyMoverInfo : ScriptableObject
    {
        [SerializeField]
         float speed = 4;
        public float Speed => speed;

    }
}
