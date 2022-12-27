using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class SceneChangeButton : MonoBehaviour
    {
        public enum ChangedTarget
        {
            Title,
            StageSelect,
            NextStage,
            Retry,
            UnPause
        }

        [SerializeField]
        ChangedTarget changedTarget;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            if (changedTarget == ChangedTarget.UnPause)
            {
                SceneLoader.Singleton.UnoverrideScene();
                return;
            }

            if (changedTarget == ChangedTarget.StageSelect ||
                changedTarget == ChangedTarget.Title)
            {
                TitleState.Singleton.IsSkipTitle = changedTarget == ChangedTarget.StageSelect;
            }

            SceneLoader.Singleton.LoadScene(GetSceneType());
        }
        SceneType GetSceneType()
        {
            return changedTarget switch
            {
                ChangedTarget.Title => SceneType.Title,
                ChangedTarget.StageSelect => SceneType.Title,
                ChangedTarget.NextStage => SceneTypeExtension.GetSceneType(SceneLoader.Singleton.Current.buildIndex + 1),
                ChangedTarget.Retry => SceneLoader.Singleton.CurrentBaseType,
                _ => SceneType.Title,
            };
        }
    }
}
