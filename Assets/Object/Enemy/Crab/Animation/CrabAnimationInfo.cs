using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum CrabAnimationType
    {
        Move,
        BeforeAttack,
        Attack,
        Death
    }
    [CreateAssetMenu(menuName = "Enemy/Crab/CrabAnimationInfo")]
    public class CrabAnimationInfo : AnimationInfoBase<CrabAnimationType>
    {
    }
}
