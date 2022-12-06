using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum SharkAnimationType
    {
        Move,
        BeforeAttack,
        Attack,
    }
    [CreateAssetMenu(menuName = "Enemy/Shark/SharkAnimationInfo")]
    public class SharkAnimationInfo : AnimationInfoBase<SharkAnimationType>
    {

    }
}
