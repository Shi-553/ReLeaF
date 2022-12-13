using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum RobotAnimationType
    {
        Walk,
        Run,
        Special
    }
    [CreateAssetMenu(menuName = "Robot/AnimationInfo")]
    public class RobotAnimationInfo : AnimationInfoBase<RobotAnimationType>
    {

    }
}
