using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class DamageValueEffect : MonoBehaviour, IPoolableSelfRelease
    {
        public void Init(bool isCreated)
        {
            if (isCreated)
            {
                TryGetComponent(out animation);
                text = GetComponentInChildren<Text>();
                textRect = text.GetComponent<RectTransform>();
            }
            animation.Play();
            StartCoroutine(WaitAnimation());
        }

        public void Uninit()
        {
        }

        Text text;
        RectTransform textRect;
        new Animation animation;

        int damage;

        [SerializeField]
        DamageValueEffectInfo info;


        IPool IPoolableSelfRelease.Pool { get; set; }

        public void ShowDamageValue(int damage, Vector3 pos)
        {
            this.damage = damage;
            transform.position = pos;
            text.text = damage.ToString();
            var size = textRect.sizeDelta;
            size.y = MathExtension.Map(damage, 0, info.MaxDamage, info.MinSize, info.MaxSize, true);
            textRect.sizeDelta = size;


            text.color = damage <= 4 ? Color.white : Color.green;
        }

        IEnumerator WaitAnimation()
        {
            yield return new WaitForSeconds(animation.clip.length);
            this.StaticCast<IPoolableSelfRelease>().Release();
        }

    }
}
