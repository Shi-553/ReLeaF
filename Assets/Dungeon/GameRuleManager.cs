using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum GameRuleState
    {
        Prepare,
        Playing,
        GameClear,
        GameOver
    }
    public class GameRuleManager : SingletonBase<GameRuleManager>
    {
        public override bool DontDestroyOnLoad => false;

        public GameRuleState State { get; protected set; } = GameRuleState.Prepare;

        public bool IsPlaying => State == GameRuleState.Playing;
        public bool IsPrepare => State == GameRuleState.Prepare;
        public bool IsFinished => State == GameRuleState.GameClear || State == GameRuleState.GameOver;
        public bool IsGameClear => State == GameRuleState.GameClear;
        public bool IsGameOver => State == GameRuleState.GameOver;

        [SerializeField]
        AllGreening allGreening;


        [SerializeField]
        GameObject gameReadyText;
        [SerializeField]
        GameObject gamestartText;
        [SerializeField]
        GameObject gameclearText;

        [SerializeField]
        AudioInfo bgmStage1;

        protected override void Init()
        {
            State = GameRuleState.Prepare;
        }

        protected virtual IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            gameReadyText.SetActive(true);
            yield return new WaitForSeconds(1);
            gameReadyText.SetActive(false);
            gamestartText.SetActive(true);

            State = GameRuleState.Playing;

            yield return new WaitForSeconds(1);
            gamestartText.SetActive(false);

            BGMManager.Singleton.Play(bgmStage1);
        }

        public void Finish(bool isGameClear)
        {
            if (!IsPlaying)
                return;
            State = isGameClear ? GameRuleState.GameClear : GameRuleState.GameOver;

            if (isGameClear)
                StartCoroutine(WaitGreening());
        }
        IEnumerator WaitGreening()
        {
            yield return StartCoroutine(allGreening.StartGreeningWithPlayer());
            gameclearText.SetActive(true);
        }

    }
}
