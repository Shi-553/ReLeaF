using Animancer;
using System.Collections;
using TMPro;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DamageValueEffect : PoolableMonoBehaviour
    {
        protected override void InitImpl()
        {
            if (!IsInitialized)
            {
                TryGetComponent(out animancer);
                text = GetComponentInChildren<TextMeshPro>(true);
                wait = new WaitForSeconds(info.InitAnimation.length);
            }
            animancer.Play(info.InitAnimation);
            StartCoroutine(WaitAnimation());
        }

        protected override void UninitImpl()
        {
        }

        TextMeshPro text;

        int damage;
        bool IsHighDamage => damage > info.HighDamageThreshold;


        [SerializeField]
        DamageValueEffectInfo info;

        AnimancerComponent animancer;
        WaitForSeconds wait;

        public void ShowDamageValue(int damage, Vector3 pos)
        {
            this.damage = damage;
            transform.position = pos + info.Offset + Vector3.Lerp(-info.RandomOffsetMax, info.RandomOffsetMax, Random.value);

            var damageStr = damage.ToString();
            var damageText = "";
            foreach (var damageChar in damageStr)
            {
                damageText += $"<sprite index={damageChar}>";
            }

            text.text = damageText;
            text.fontSize = MathExtension.Map(damage, 0, info.MaxDamage, info.MinSize, info.MaxSize, true);

            text.sortingOrder = 100 + damage;
            text.spriteAsset = IsHighDamage ? info.HighDamageSpriteAsset : info.NormalDamageSpriteAsset;
        }

        IEnumerator WaitAnimation()
        {
            yield return wait;
            Release();
        }

    }
}
