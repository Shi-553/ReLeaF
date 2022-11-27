using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class ToStageSelect : MonoBehaviour
    {
        [SerializeField]
        Button startButton;
        [SerializeField]
        GameObject[] toActiveObj;


        void Start()
        {
            startButton.onClick.AddListener(() => StartStageSelect());
        }
        void StartStageSelect()
        {
            Camera.main.GetComponent<Animation>().Play();
            toActiveObj.ForEach(x => x.SetActive(true));

        }
    }
}
