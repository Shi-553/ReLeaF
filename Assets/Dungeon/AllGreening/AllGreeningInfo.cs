using UnityEngine;
using Utility;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Dungeon/AllGreeningInfo")]
    public class AllGreeningInfo : ScriptableObject
    {
        [SerializeField, Rename("次のマスを緑化するまでの時間")]
        float greeningTime = 0.1f;
        public float GreeningTime => greeningTime;

    }
}
