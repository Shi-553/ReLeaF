using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class PlayerCore : SingletonBase<PlayerCore>
    {
        [SerializeField]
        ValueGaugeManager hpGauge;

        PlayerMover mover;
        public PlayerMover Mover => mover;
        SpriteRenderer spriteRenderer;

        [SerializeField, Rename("ダメージを受けたときの無敵時間")]
        float damagedInvicibleTime = 3;
        [SerializeField, Rename("無敵時間の点滅間隔")]
        float damagedFlashingInterval = 0.1f;
        [SerializeField, Rename("無敵時間の点滅の透明度")]
        float damagedFlashingAlpha = 0.5f;


        int isInvincible = 0;
        Coroutine InvincibleCo;
        public bool IsInvincible => isInvincible > 0;

        public void AddInvincible(bool useEffect = true)
        {
            isInvincible++;

            if (IsInvincible && useEffect && InvincibleCo == null)
                InvincibleCo = StartCoroutine(Flashing());
        }
        public void RemoveInvincible()
        {
            isInvincible--;

            if (!IsInvincible && InvincibleCo != null)
            {
                InvincibleCo = null;
            }
        }
        IEnumerator Flashing()
        {
            float time = 0;
            var color = spriteRenderer.color;
            var flashingColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, damagedFlashingAlpha);
            while (true)
            {
                bool isFlashing = (((int)(time / damagedFlashingInterval)) % 2) == 1;
                if (!IsInvincible)
                    isFlashing = false;
                var currentColor = isFlashing ? flashingColor : color;

                if (currentColor != spriteRenderer.color)
                    spriteRenderer.color = currentColor;

                time += Time.deltaTime;

                if (!IsInvincible)
                {
                    break;
                }

                yield return null;
            }
        }

        public event Action OnDamaged;

        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        AudioInfo seDamaged;
        [SerializeField]
        AudioInfo seRecovered;
        [SerializeField]
        AudioInfo seInvincible;

        float damagedTime;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            TryGetComponent(out mover);

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            hpGauge.Slider = PlayerStatusUI.Singleton.HPSlider;
        }


        public void Damaged(float damage, Vector3 impulse)
        {

            if (!GameRuleManager.Singleton.IsPlaying)
                return;
            if (IsInvincible)
            {
                var duration = Time.time - damagedTime;
                if (duration > 0.3)
                {// 0.3ミリ秒経ってたら鳴らす
                    SEManager.Singleton.Play(seInvincible);
                }
                return;
            }

            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Strong, 0.3f);
            damagedTime = Time.time;

            if (hpGauge.ConsumeValue(damage))
            {
                StartCoroutine(mover.KnockBack(impulse));
                StartCoroutine(Damaged());
                SEManager.Singleton.Play(seDamaged);

                if (hpGauge.Value == 0)
                {
                    Death();
                }
            }
        }
        IEnumerator Damaged()
        {
            AddInvincible(true);
            OnDamaged?.Invoke();

            yield return new WaitForSeconds(damagedInvicibleTime);

            RemoveInvincible();
        }
        void Death()
        {
            GameRuleManager.Singleton.Finish(false);
        }


        public void HPRecoverAll()
        {
            SEManager.Singleton.Play(seRecovered);
            hpGauge.RecoveryValue(hpGauge.ValueMax);
        }
    }
}
