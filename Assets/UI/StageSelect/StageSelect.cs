using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField]
        StageInfo[] stageInfos;
        [SerializeField]
        float fadeoutTime = 2.0f;
        [SerializeField]
        float fadeinTime = 1.0f;

        void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var existsInfo = stageInfos.Length > i;

                var button = child.GetComponent<Button>();
                button.interactable = existsInfo;

                if (existsInfo)
                {
                    var info = stageInfos[i];
                    child.GetComponentInChildren<Text>().text = info.StageName;

                    button.onClick.AddListener(() =>
                    {
                        SceneLoader.Singleton.ChangeScene(info.Scene, fadeoutTime, fadeinTime);
                    });
                }
                else
                {
                    child.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }
    }
}
