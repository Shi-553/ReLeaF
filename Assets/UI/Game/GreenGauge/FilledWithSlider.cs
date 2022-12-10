using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class FilledWithSlider : MonoBehaviour
    {
        [SerializeField]
        Slider target;
        Image image;
        void Start()
        {
            TryGetComponent(out image);
            target.onValueChanged.AddListener(OnValueChanged);
        }
        void OnValueChanged(float value)
        {
            image.fillAmount = value;
        }
    }
}
