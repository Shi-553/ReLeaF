using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public enum GameRuleState
    {
        Prepare,
        Playing,
        GameClear,
        GameOver
    }
    public class GameRuleManager : MonoBehaviour
    {
        public GameRuleState State { get; private set; }

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

        public static GameRuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        private IEnumerator Start()
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
            yield return StartCoroutine(allGreening.StartGreening());
            gameclearText.SetActive(true);
        }
    }
}
