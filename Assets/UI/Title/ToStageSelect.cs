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

        [SerializeField]
        AudioInfo seClick;

        AnimancerComponent animancer;
        void Start()
        {
            animancer = GetComponentInParent<AnimancerComponent>();

            if (TitleState.Singleton.IsSkipTitle)
            {
                toStageSelect.SampleAnimation(animancer.gameObject, toStageSelect.length);

                toActiveObj.ForEach(x => x.SetActive(true));
                gameObject.SetActive(false);
            }
            else
                startButton.onClick.AddListener(() => StartStageSelect());
        }
        void StartStageSelect()
        {
            SEManager.Singleton.Play(seClick, transform.position);
            TitleState.Singleton.IsSkipTitle = true;
            animancer.Play(toStageSelect);
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
