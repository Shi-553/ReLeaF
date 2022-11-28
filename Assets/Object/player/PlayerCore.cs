using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class PlayerCore : MonoBehaviour
    {
        [SerializeField]
        ValueGaugeManager hpGauge;

        PlayerMover mover;
        SpriteRenderer spriteRenderer;

        [SerializeField, Rename("ダメージを受けたときの無敵時間")]
        public float damagedInvicibleTime = 3;
        [SerializeField, Rename("無敵時間の点滅間隔")]
        public float damagedFlashingInterval = 0.1f;
        [SerializeField, Rename("無敵時間の点滅の透明度")]
        public float damagedFlashingAlpha = 0.5f;

        public bool IsInvincible { get; set; }

        [SerializeField]
        AudioInfo seDamaged;


        private void Awake()
        {
            IsInvincible = false;
            TryGetComponent(out mover);

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }


        public void Damaged(float damage, Vector3 impulse)
        {
            if (!GameRuleManager.Singleton.IsPlaying)
                return;
            if (IsInvincible)
                return;

            if (hpGauge.ConsumeValue(damage))
            {
                StartCoroutine(mover.KnockBack(impulse));
                StartCoroutine(Damaged());
                SEManager.Singleton.Play(seDamaged, transform.position);

                if (hpGauge.Value == 0)
                {
                    Death();
                }
            }
        }
        IEnumerator Damaged()
        {
            IsInvincible = true;
            float time = 0;
            var color = spriteRenderer.color;
            var flashingColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, damagedFlashingAlpha);
            while (true)
            {
                bool isFlashing = (((int)(time / damagedFlashingInterval)) % 2) == 1;
                var currentColor = isFlashing ? color : flashingColor;

                if (currentColor != spriteRenderer.color)
                    spriteRenderer.color = currentColor;

                time += Time.deltaTime;
                if (time > damagedInvicibleTime)
                {
                    break;
                }
                yield return null;
            }
            spriteRenderer.enabled = true;

            IsInvincible = false;
        }
        void Death()
        {
            spriteRenderer.enabled = false;
            GameRuleManager.Singleton.Finish(false);
        }


    }
}
