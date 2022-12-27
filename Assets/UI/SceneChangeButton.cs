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
            Retry
        }

        [SerializeField]
        ChangedTarget changedTarget;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            SceneLoader.Singleton.LoadScene(GetSceneType(), 0.5f, 0.5f);
        }
        SceneType GetSceneType()
        {

            return changedTarget switch
            {
                ChangedTarget.Title => SceneType.Title,
                ChangedTarget.StageSelect => SceneType.Title,
                ChangedTarget.NextStage => SceneTypeExtension.GetSceneType(SceneLoader.Singleton.Current.buildIndex + 1),
                ChangedTarget.Retry => SceneLoader.Singleton.CurrentType,
                _ => SceneType.Title,
            };
        }
    }
}
