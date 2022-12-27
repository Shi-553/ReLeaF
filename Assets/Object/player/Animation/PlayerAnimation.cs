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

        bool isDamagedAnimation = false;

        private IEnumerator OnDamaged()
        {
            isDamagedAnimation = true;
            yield return animancerComponent.Play(info.GetClip(PlayerAnimationType.Damaged, mover.IsLeft));
            isDamagedAnimation = false;
        }

        private void Update()
        {
            if (isDamagedAnimation)
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
