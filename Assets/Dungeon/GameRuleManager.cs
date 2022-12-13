using System;
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

        GameRuleState state = GameRuleState.Prepare;
        public GameRuleState State
        {
            get => state;
            protected set
            {
                state = value;
                OnChangeState?.Invoke(state);
            }
        }
        public event Action<GameRuleState> OnChangeState;

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
        [SerializeField]
        AudioInfo seReady;
        [SerializeField]
        AudioInfo seStart;
        [SerializeField]
        AudioInfo clearBGM;
        [SerializeField]
        AudioInfo stageClear1;
        [SerializeField]
        AudioInfo stageClear2;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            State = GameRuleState.Prepare;
        }

        protected virtual IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            gameReadyText.SetActive(true);
            SEManager.Singleton.Play(seReady);
            yield return new WaitForSeconds(1);
            gameReadyText.SetActive(false);
            gamestartText.SetActive(true);
            SEManager.Singleton.Play(seStart);

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
            {
                BGMManager.Singleton.Stop();
                SEManager.Singleton.Play(stageClear1);
                StartCoroutine(WaitClearSound());
                StartCoroutine(WaitGreening());
            }
        }
        IEnumerator WaitClearSound()
        {
            yield return new WaitForSeconds(stageClear1.clip.length);
            SEManager.Singleton.Play(stageClear2);

        }
        IEnumerator WaitGreening()
        {
            yield return StartCoroutine(allGreening.StartGreeningWithPlayer());
            gameclearText.SetActive(true);
            SEManager.Singleton.Play(clearBGM);
        }

    }
}
