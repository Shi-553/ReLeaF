using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum PlayerAnimationType
    {
        Standby,
        Walk,
        Run
    }
    [CreateAssetMenu(menuName ="Player/AnimationInfo")]
    public class PlayerAnimationInfo : AnimationInfoBase<PlayerAnimationType>
    {

    }
}
