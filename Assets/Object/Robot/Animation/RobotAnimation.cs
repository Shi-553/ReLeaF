using Animancer;
using UnityEngine;

namespace ReLeaf
{
    public class RobotAnimation : MonoBehaviour
    {
        [SerializeField]
        RobotAnimationInfo info;

        AnimancerComponent animancerComponent;
        RobotMover mover;

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
                AnimationPlay(info.GetClip(RobotAnimationType.Run, mover.IsLeft));
                return;
            }
            AnimationPlay(info.GetClip(RobotAnimationType.Walk, mover.IsLeft));
            return;
        }
    }
}
