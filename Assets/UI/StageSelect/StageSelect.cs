using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField]
        Transform icons;
        [SerializeField]
        StageInfo[] stageInfos;

        void Awake()
        {
            for (int i = 0; i < icons.childCount; i++)
            {
                var child = icons.GetChild(i);

                var button = child.GetComponent<Button>();

                var info = stageInfos[i];
                button.interactable = true;
                child.Find("Sprite").GetComponent<Image>().sprite = info.Activebutton;

                button.onClick.AddListener(() =>
                {
                    StageManager.Singleton.Current = info;
                    SceneLoader.Singleton.LoadScene(info.Scene, 3.0f);
                });
            }
        }
    }
}
