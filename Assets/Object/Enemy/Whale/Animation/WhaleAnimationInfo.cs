using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum WhaleAnimationType
    {
        Move,
        BeforeAttack,
        Attack,
        Death
    }
    [CreateAssetMenu(menuName = "Enemy/Whale/WhaleAnimationInfo")]
    public class WhaleAnimationInfo : AnimationInfoBase<WhaleAnimationType>
    {
    }
}
