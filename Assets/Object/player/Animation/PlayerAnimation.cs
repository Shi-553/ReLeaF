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
                AnimationPlay(info.GetClip(PlayerAnimationType.Run, mover.IsLeft));
                return;
            }
            if (mover.IsMove)
            {
                AnimationPlay(info.GetClip(PlayerAnimationType.Walk, mover.IsLeft));
                return;
            }

            AnimationPlay(info.GetClip(PlayerAnimationType.Standby, mover.IsLeft));
            return;
        }
    }
}
