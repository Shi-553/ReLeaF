using Animancer;
using System.Collections;
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

        [SerializeField]
        AnimationClip toStageSelect;

        void Start()
        {
            startButton.onClick.AddListener(() => StartStageSelect());
        }
        void StartStageSelect()
        {
            startButton.enabled = false;
            GetComponentInParent<AnimancerComponent>().Play(toStageSelect);
            toActiveObj.ForEach(x => x.SetActive(true));
            StartCoroutine(WaitAnimation());
        }
        IEnumerator WaitAnimation()
        {
            yield return new WaitForSeconds(toStageSelect.length);
            gameObject.SetActive(false);
        }
    }
}
