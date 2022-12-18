using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum SeaUrhinAnimationType
    {
        BeforeAttack,
        Attack,
        AfterAttack,
        Death
    }
    [CreateAssetMenu(menuName = "Enemy/SeaUrchin/SeaUrhinAnimationInfo")]
    class SeaUrhinAnimationInfo : AnimationInfoBase<SeaUrhinAnimationType>
    {
    }
}
