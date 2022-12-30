using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class ClickSE : MonoBehaviour
    {
        [SerializeField]
        AudioInfo seClick;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }
        void OnClick()
        {
            SEManager.Singleton.Play(seClick);
        }
    }
}
