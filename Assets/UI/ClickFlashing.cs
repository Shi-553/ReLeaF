using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class ClickFlashing : MonoBehaviour
    {
        List<Image> images = new();

        float flashingAlpha = 0.5f;
        float flashingTime = 0.5f;
        float flashingInterval = 0.1f;

        void Start()
        {
            GetComponentsInChildren(images);

            GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Flash()));
        }

        IEnumerator Flash()
        {
            float time = 0;

            while (true)
            {

                bool isFlashing = (((int)(time / flashingInterval)) % 2) == 1;

                foreach (var image in images)
                {
                    var currentColor = image.color;
                    currentColor.a = isFlashing ? 1 : flashingAlpha;

                    if (currentColor != image.color)
                        image.color = currentColor;
                }

                time += Time.unscaledDeltaTime;
                if (time > flashingTime)
                    break;
                yield return null;
            }

            foreach (var image in images)
            {
                var currentColor = image.color;
                currentColor.a = 1;
                image.color = currentColor;
            }
        }
    }
}
