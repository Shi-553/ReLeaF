using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class FaceUpdater : MonoBehaviour
    {
        [Serializable]
        class FaceInfo
        {
            public float maxHPRate = 1.0f;
            public Sprite sprite;
        }

        [SerializeField]
        FaceInfo[] faces;

        [SerializeField]
        Slider hpSlider;

        Image image;
        void Start()
        {
            faces = faces.OrderBy(f => f.maxHPRate).ToArray();
            TryGetComponent(out image);
            hpSlider.onValueChanged.AddListener(OnValueChanged);
        }
        void OnValueChanged(float value)
        {
            foreach (var face in faces)
            {
                if (face.maxHPRate > value)
                {
                    image.sprite = face.sprite;
                    break;
                }
            }
        }
    }
}
