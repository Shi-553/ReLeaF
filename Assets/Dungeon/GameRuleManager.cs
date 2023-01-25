using System;
using System.Collections;
using UnityEngine;
using Utility;
namespace ReLeaf
{
    public enum GameRuleState
    {
        None,
        Prepare,
        Playing,
        GameClear,
        GameOver,
        Pause
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

        GameRuleState pausedState;
        public void Pause()
        {
            if (State == GameRuleState.Pause)
                return;

            pausedState = State;
            State = GameRuleState.Pause;
        }
        public void UnPause()
        {
            if (State == GameRuleState.Pause)
                State = pausedState;
        }

        public bool isWaitFinish;
        public bool IsWaitFinish
        {
            get => isWaitFinish;
            set
            {
                isWaitFinish = value;

                if (!isWaitFinish && waitState != GameRuleState.None)
                {
                    Finish(waitState);
                    waitState = GameRuleState.None;
                }
            }
        }
        GameRuleState waitState = GameRuleState.None;


        [SerializeField]
        GameObject gameclearObj;
        [SerializeField]
        GameObject gameoverObj;

        [SerializeField]
        GameObject playingUIRoot;

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
        [SerializeField]
        AudioInfo gameOverBGM;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            State = GameRuleState.Prepare;
        }
        protected override void UninitBeforeSceneUnload(bool isDestroy)
        {
            SceneLoader.Singleton.OnFinishFadein -= Ready;
        }
        protected virtual void Start()
        {
            SceneLoader.Singleton.OnFinishFadein += Ready;
        }
        void Ready()
        {
            StartCoroutine(WaitReady());
        }
        IEnumerator WaitReady()
        {
            yield return new WaitForSeconds(0.5f);

            SEManager.Singleton.Play(seReady);
            yield return NotificationUI.Singleton.Notice(NotificationUI.NotificationType.GameReady, 1.0f);

            var co = NotificationUI.Singleton.Notice(NotificationUI.NotificationType.GameStart, 1.0f);

            yield return new WaitForSeconds(0.1f);
            SEManager.Singleton.Play(seStart);

            State = GameRuleState.Playing;

            yield return co;

            BGMManager.Singleton.Play(bgmStage1);
        }

        public void Finish(bool isGameClear)
        {
            if (!IsPlaying)
                return;
            Finish(isGameClear ? GameRuleState.GameClear : GameRuleState.GameOver);
        }
        void Finish(GameRuleState state)
        {
            if (IsWaitFinish)
            {
                waitState = state;
                return;
            }
            if (!IsPlaying)
                return;
            State = state;

            if (state == GameRuleState.GameClear)
            {
                StartCoroutine(WaitClearSound());
                StartCoroutine(WaitGreening());
            }
            else
            {
                BGMManager.Singleton.Play(gameOverBGM);
                gameoverObj.SetActive(true);
                Finish();

            }
        }
        protected IEnumerator WaitClearSound()
        {
            BGMManager.Singleton.Stop();
            SEManager.Singleton.Play(stageClear1);
            yield return new WaitForSeconds(stageClear1.clip.length - 0.2f);
            SEManager.Singleton.Play(stageClear2);

        }
        IEnumerator WaitGreening()
        {
            PlayerController.Singleton.ChangeToUI();

            yield return StartCoroutine(AllGreening.Singleton.StartGreeningWithPlayer());
            gameclearObj.SetActive(true);
            SEManager.Singleton.Play(clearBGM);
            Finish();
        }

        /// <summary>
        /// 終了時の共通処理
        /// </summary>
        void Finish()
        {
            PlayerCore.Singleton.gameObject.SetActive(false);
            RobotMover.Singleton.gameObject.SetActive(false);
            ItemDescription.Singleton.ResetItemDescription();

            foreach (var item in FindObjectsOfType<ItemBase>())
            {
                item.gameObject.SetActive(false);
            }

            playingUIRoot.SetActive(false);
        }
    }
}
