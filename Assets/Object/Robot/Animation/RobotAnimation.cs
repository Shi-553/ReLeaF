using Animancer;
using System.Collections;
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
        bool isSpecial = false;
        public IEnumerator Thrust()
        {
            isSpecial = true;
            yield return animancerComponent.Play(info.GetClip(RobotAnimationType.Special, mover.IsLeft));
            isSpecial = false;
        }

        private void Update()
        {
            if (isSpecial)
                return;

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
