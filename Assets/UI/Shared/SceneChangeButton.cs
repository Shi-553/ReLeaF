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

        [SerializeField]
        AudioInfo seMenuOpen;
        [SerializeField]
        AudioInfo seMenuClose;
        [SerializeField]
        AudioInfo seMenuClick;

        void Start()
        {
            SEManager.Singleton.Play(seMenuOpen, transform.position);
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            if (changedTarget == ChangedTarget.UnPause)
            {
                SEManager.Singleton.Play(seMenuClose, transform.position);
                SceneLoader.Singleton.UnoverrideScene();
                return;
            }
            else
            {
                SEManager.Singleton.Play(seMenuClick, transform.position);
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
