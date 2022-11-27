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
        public GameRuleState State { get; protected set; }

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


        protected override void Init()
        {
        }

        protected virtual IEnumerator Start()
        {
            gameReadyText.SetActive(true);
            yield return new WaitForSeconds(1);
            gameReadyText.SetActive(false);
            gamestartText.SetActive(true);

            State = GameRuleState.Playing;

            yield return new WaitForSeconds(1);
            gamestartText.SetActive(false);
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
