using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class ClickToEnd : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
            });

        }
    }
}
