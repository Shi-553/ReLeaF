using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public struct ChangeStatusinfo
    {
        public float duration;
        public Sprite sprite;

        public ChangeStatusinfo(float duration, Sprite sprite)
        {
            this.duration = duration;
            this.sprite = sprite;
        }
    }
    public class StatusChange : MonoBehaviour
    {
        [SerializeField]
        Image[] sprites;

        [SerializeField]
        RectTransform mask;

        ChangeStatusinfo info;
        public void Init(ChangeStatusinfo info)
        {
            this.info = info;
            sprites.ForEach(s => s.sprite = info.sprite);
            StartCoroutine(Wait());
        }
        IEnumerator Wait()
        {
            float timer = 0;

            var children = mask.GetChildren().ToArray();
            while (true)
            {
                yield return null;
                timer += Time.deltaTime;

                var rate = 1 - (timer / info.duration);
                var pos = mask.localPosition;
                pos.y = rate * mask.sizeDelta.y;

                mask.localPosition = pos;

                pos.y = -rate * mask.sizeDelta.y;
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].localPosition = pos;
                }
                if (timer > info.duration)
                    break;

            }
            StatusChangeManager.Singleton.RemoveStatus(this);
        }
    }
}
