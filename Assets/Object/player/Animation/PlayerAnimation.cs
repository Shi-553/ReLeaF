using Animancer;
using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        PlayerAnimationInfo info;

        AnimancerComponent animancerComponent;
        PlayerMover mover;
        PlayerCore core;

        private void Awake()
        {
            TryGetComponent(out mover);
            TryGetComponent(out core);
            animancerComponent = GetComponentInChildren<AnimancerComponent>();

            core.OnDamaged += () => StartCoroutine(OnDamaged());
        }

        int lockAnimation = 0;

        private IEnumerator OnDamaged()
        {
            lockAnimation++;
            yield return animancerComponent.Play(info.GetClip(PlayerAnimationType.Damaged, mover.IsLeft));
            lockAnimation--;
        }

        public void Grasp(bool sw)
        {
            lockAnimation += sw ? 1 : -1;
            if (sw)
            {
                animancerComponent.Play(info.GetClip(PlayerAnimationType.Grasp, mover.IsLeft));
            }
        }

        private void Update()
        {
            if (lockAnimation > 0)
            {
                return;
            }
            if (mover.IsDash)
            {
                animancerComponent.Play(info.GetClip(PlayerAnimationType.Run, mover.IsLeft));
                return;
            }
            if (mover.IsMove)
            {
                animancerComponent.Play(info.GetClip(PlayerAnimationType.Walk, mover.IsLeft));
                return;
            }

            animancerComponent.Play(info.GetClip(PlayerAnimationType.Standby, mover.IsLeft));
            return;
        }
    }
}
