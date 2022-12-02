using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/SeaUrchin/SeaUrhinAnimationInfo")]
    class SeaUrhinAnimationInfo : ScriptableObject
    {
        [SerializeField]
        AnimationClip beforeAttack;
        public AnimationClip BeforeAttack => beforeAttack;
        [SerializeField]
        AnimationClip attack;
        public AnimationClip Attack => attack;
        [SerializeField]
        AnimationClip afterAttack;
        public AnimationClip AfterAttack => afterAttack;
    }
}
