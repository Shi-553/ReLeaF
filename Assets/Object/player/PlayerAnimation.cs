using Animancer;
using UnityEngine;

namespace ReLeaf
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        PlayerAnimationInfo info;

        AnimancerComponent animancerComponent;
        PlayerMover mover;

        private void Awake()
        {
            TryGetComponent(out mover);
            animancerComponent = GetComponentInChildren<AnimancerComponent>();
        }
        public void AnimationPlay(AnimationClip clip)
        {
            if (!animancerComponent.IsPlayingClip(clip))
                animancerComponent.Play(clip);
        }

        private void Update()
        {
            if (mover.IsDash)
            {
                AnimationPlay(info.GetPair(PlayerAnimationType.Run).GetClip(mover.IsLeft));
                return;
            }
            if (mover.Move != Vector2.zero)
            {
                AnimationPlay(info.GetPair(PlayerAnimationType.Walk).GetClip(mover.IsLeft));
                return;
            }

            AnimationPlay(info.GetPair(PlayerAnimationType.Standby).GetClip(mover.IsLeft));
            return;
        }
    }
}
