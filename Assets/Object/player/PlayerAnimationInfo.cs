using UnityEngine;

namespace ReLeaf
{
    public enum PlayerAnimationType
    {
        Standby,
        Walk
    }
    [CreateAssetMenu(menuName ="Player/AnimationInfo")]
    public class PlayerAnimationInfo : AnimationInfoBase<PlayerAnimationType>
    {

    }
}
