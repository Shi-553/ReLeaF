using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace ReLeaf
{
    public class PauseMenuInputReceiver : MonoBehaviour
    {
        ReLeafInputAction inputAction;
        private void OnEnable()
        {
            inputAction = new();
            inputAction.UI.Pause.started += Pause;
            inputAction.UI.Enable();
        }

        private void Pause(InputAction.CallbackContext obj)
        {
            if (GameRuleManager.Singleton.IsFinished)
                return;
            var loader = SceneLoader.Singleton;
            if (loader.IsPause)
                loader.UnoverrideScene();
            else
                loader.OverrideScene(SceneType.Menu);
        }
    }
}
