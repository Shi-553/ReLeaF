using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public static class ManagerSceneInitializer
    {
#if DEFINE_SCENE_TYPE_ENUM
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SingletonSceneInitialize()
        {
            if (Enum.TryParse<SceneType>("Manager", true, out var sceneType))
            {
                SceneManager.LoadScene(sceneType.GetBuildIndex(), LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning("Manager scene not found.");
            }
        }
#endif
    }
}
