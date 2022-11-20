using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("敵({asset.dirname})の移動パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/EnemyMoverInfo")]
    public class EnemyMoverInfo : ScriptableObject
    {
        [SerializeField,Rename("移動スピード(nマス/秒)")]
         float speed = 4;
        public float Speed => speed;

    }
}
