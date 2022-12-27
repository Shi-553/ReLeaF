using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility.Definition;

namespace Utility
{
    public class SceneLoader : SingletonBase<SceneLoader>
    {

        public override bool DontDestroyOnLoad => true;
        // アクティブなシーン
        public Scene Current { get; private set; }
#if DEFINE_SCENE_TYPE_ENUM
        public SceneType CurrentType => Current.GetSceneType();

        // オーバーライドされて後ろにいったシーン
        public Scene? Background { get; private set; } = null;
        public SceneType? BackgroundType => Background?.GetSceneType();

        // オーバーライドされてるときは必ずポーズする
        public bool IsPause => Background.HasValue;
        public bool IsOverride => Background.HasValue;


        Coroutine changeing;

        [SerializeField]
        GameObject loading;
        [SerializeField]
        AudioListener audioListener;

        Image fadeImage;



#endif
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            Current = SceneManager.GetActiveScene();
            if (Current.name == "" && Current.path == "")
            {
                Debug.LogWarning("ビルド設定にないので上手く遷移しないかも");
            }
            fadeImage = loading.GetComponentInChildren<Image>();
        }

#if DEFINE_SCENE_TYPE_ENUM
        public void LoadScene(SceneType scene, float fadeoutTime = 0, float fadeinTime = 0)
        {
            if (changeing == null)
            {
                changeing = StartCoroutine(LoadSceneAsync(scene, fadeoutTime, fadeinTime));
            }
        }

        IEnumerator LoadSceneAsync(SceneType type, float fadeoutTime = 0, float fadeinTime = 0)
        {
            audioListener.enabled = false;
            loading.SetActive(true);
            if (fadeoutTime > 0)
            {
                float counter = 0;
                while (true)
                {
                    fadeImage.color = new Color(0, 0, 0, (counter / fadeoutTime) * (counter / fadeoutTime));
                    if (fadeoutTime <= counter)
                        break;
                    counter += Time.deltaTime;

                    yield return null;
                }
            }
            fadeImage.color = new Color(0, 0, 0, 1);

            Time.timeScale = 0;

            BGMManager.Singleton.StopAll();
            SEManager.Singleton.StopAll();

            var definitionSingletonBases = FindObjectsOfType<DefinitionSingletonBase>();
            definitionSingletonBases.ForEach(s => s.UninitBeforeSceneUnloadDefinition());

            // とりあえずマネージャーシーンをアクティブに
            SceneManager.SetActiveScene(gameObject.scene);

            yield return SceneManager.UnloadSceneAsync(Current);

            if (IsPause)
            {
                yield return SceneManager.UnloadSceneAsync(Background.Value.buildIndex);
                Background = null;
            }

            definitionSingletonBases.ForEach(s => s.UninitAfterSceneUnloadDefinition());
            definitionSingletonBases.ForEach(s => s.TryDestroy());
            // audioListener.enabled = true;

            yield return Resources.UnloadUnusedAssets();

            audioListener.enabled = false;
            yield return SceneManager.LoadSceneAsync(type.GetBuildIndex(), LoadSceneMode.Additive);


            Current = SceneManager.GetSceneByBuildIndex(type.GetBuildIndex());
            SceneManager.SetActiveScene(Current);

            Debug.Log($"Changed to <b>{CurrentType}</b>");

            Time.timeScale = 1;

            if (fadeinTime > 0)
            {
                float counter = 0;
                while (true)
                {
                    fadeImage.color = new Color(0, 0, 0, (1 - (counter / fadeinTime)) * (1 - (counter / fadeinTime)));
                    if (fadeinTime <= counter)
                        break;
                    counter += Time.deltaTime;

                    yield return null;
                }
            }
            fadeImage.color = new Color(0, 0, 0, 0);

            loading.SetActive(false);
            changeing = null;
        }

        public void OverrideScene(SceneType scene)
        {
            if (changeing == null && !IsPause)
            {
                changeing = StartCoroutine(OverrideAsync(scene));
            }
        }
        public void UnoverrideScene()
        {
            if (changeing == null && IsPause)
            {
                changeing = StartCoroutine(UnoverrideAsync());
            }
        }

        IEnumerator OverrideAsync(SceneType scene)
        {
            Time.timeScale = 0;

            yield return SceneManager.LoadSceneAsync(scene.GetBuildIndex(), LoadSceneMode.Additive);


            Background = Current;
            Current = SceneManager.GetSceneByBuildIndex(scene.GetBuildIndex());
            SceneManager.SetActiveScene(Current);

            Debug.Log($"Override to <b>{CurrentType}</b>");

            changeing = null;
        }

        IEnumerator UnoverrideAsync()
        {
            Time.timeScale = 0;

            yield return SceneManager.UnloadSceneAsync(Current.buildIndex);

            Debug.Log($"Unoverride to <b>{CurrentType}</b>");

            Current = Background.Value;
            Background = null;
            SceneManager.SetActiveScene(Current);


            Time.timeScale = 1;

            changeing = null;
        }

#endif
    }
}
