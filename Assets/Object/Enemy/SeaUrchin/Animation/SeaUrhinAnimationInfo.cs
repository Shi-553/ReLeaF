using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum SeaUrhinAnimationType
    {
        BeforeAttack,
        Attack,
        AfterAttack
    }
    [CreateAssetMenu(menuName = "Enemy/SeaUrchin/SeaUrhinAnimationInfo")]
    class SeaUrhinAnimationInfo : AnimationInfoBase<SeaUrhinAnimationType>
    {
    }
}
