using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum SharkAnimationType
    {
        Move,
        BeforeAttack,
        Attack,
        Death
    }
    [CreateAssetMenu(menuName = "Enemy/Shark/SharkAnimationInfo")]
    public class SharkAnimationInfo : AnimationInfoBase<SharkAnimationType>
    {

    }
}
