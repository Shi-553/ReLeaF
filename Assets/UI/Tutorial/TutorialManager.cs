using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ReLeaf
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField]
        TMP_Text text;
        [SerializeField]
        Button nextButton;
        int eventCount = 0;

        readonly static string ACTION_DISP_COLOR = "<color=#0000FF>";
        readonly static string NOMAL_COLOR = "<color=black>";

        [SerializeField]
        GameObject greeningRateMask;

        [SerializeField]
        float autoNextWaitTime = 3;
        [SerializeField]
        float niceWaitTime = 2;

        [SerializeField]
        WallRemover wallRemover;

        [SerializeField]
        OnEnterChecker enterChecker;
        [SerializeField]
        EnemyCore enemy;

        void Start()
        {
            eventCount = 0;
            GameRuleManager.Singleton.OnChangeState += OnChangeState;
        }

        private void OnChangeState(GameRuleState obj)
        {
            if (obj == GameRuleState.Playing)
                StartCoroutine(TutorialMain());
        }

        IEnumerator TutorialMain()
        {
            var lake = SpawnLakeManager.Singleton.Groups.First();
            lake.CanSpawn = false;

            PlayerController.Singleton.PlayerInput.enabled = false;
            {
                text.text = "ロボットと協力して砂漠を緑化しよう！";
                yield return WaitClick();
            }

            {
                text.text = "緑化ゲージを満タンにするとクリア！";
                greeningRateMask.SetActive(true);
                yield return WaitClick();
                greeningRateMask.SetActive(false);
            }

            PlayerController.Singleton.PlayerInput.enabled = true;

            {
                text.text = ACTION_DISP_COLOR;

                if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.Move, out var moveDisplayString))
                {
                    text.text += $"{moveDisplayString}";
                }

                text.text += $" {NOMAL_COLOR}まずは歩いて緑化しよう！";
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Move);
            }

            {
                text.text = "いいね！！";
                yield return new WaitForSeconds(niceWaitTime);
            }

            {
                text.text = $"歩くと緑化エネルギー(EP)が溜まるよ！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                text.text = ACTION_DISP_COLOR;

                if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.Dash, out var dashDisplayString))
                {
                    text.text += $"移動＋{dashDisplayString}";
                }
                text.text += $" {NOMAL_COLOR}EPを使ってダッシュ！";
                PlayerMover.Singleton.IsDash = false;
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Dash);
            }

            {
                text.text = "最高！！！！";
                yield return new WaitForSeconds(niceWaitTime);
            }


            {
                wallRemover.RemoveWall();
                text.text = "そのまま上に進もう！";
                yield return WaitOnEnter();
            }

            {
                text.text = "敵がいるよ！赤い攻撃マスは避けよう！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                text.text = "黄色い弱点マスを踏んで倒そう！";
                yield return new WaitUntil(() => enemy == null || enemy.gameObject == null);
            }


            {
                text.text = "ナイス！！落としたアイテムを拾おう！";
                yield return new WaitForSeconds(niceWaitTime);
            }

            {
                text.text = ACTION_DISP_COLOR;

                if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.UseItem, out var displayString))
                {
                    text.text += $"{displayString}";
                }
                text.text += $" {NOMAL_COLOR}アイテムを使ってみよう！";
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.UseItem);
            }

            var enemys = lake.SpawnAllNow();


            {
                text.text = "湖から敵が湧いちゃった！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            while (!lake.IsGreening)
            {
                text.text = "敵を倒して、アイテムを使って湖を緑化しよう！";
                yield return new WaitUntil(() => lake.IsGreening || enemys.All(e => e == null || e.gameObject == null));

                if (!lake.IsGreening)
                {
                    yield return new WaitForSeconds(autoNextWaitTime);
                    enemys = lake.SpawnAllNow();
                }
            }


            {
                text.text = "ブラボー！！！！！！！";
                yield return new WaitForSeconds(niceWaitTime);
            }

            {
                text.text = "これでチュートリアルは完了！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }
            {
                text.text = "緑化ゲージを満タンにして終了しよう！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            gameObject.SetActive(false);
        }

        public bool TryGetDisplayString(InputAction action, out string displayString)
        {
            displayString = "";

            var currentControlScheme = PlayerController.Singleton.PlayerInput.currentControlScheme;


            var bindingMask = new InputBinding(groups: currentControlScheme, path: null);

            for (var i = 0; i < action.bindings.Count; i++)
            {
                if (bindingMask.Matches(action.bindings[i]))
                {
                    action.GetBindingDisplayString(i, out string device, out var add);

                    displayString += $"{add.ToUpper()} ";
                }
            }

            return !string.IsNullOrEmpty(displayString);
        }


        IEnumerator WaitOnEnter()
        {
            enterChecker.OnEnter += OnEnter;

            var nowCount = eventCount;

            yield return new WaitUntil(() => eventCount != nowCount);

            enterChecker.OnEnter -= OnEnter;
        }
        private void OnEnter(Collider2D obj)
        {
            if (obj.TryGetComponent<PlayerController>(out var playerController))
                eventCount++;
        }


        IEnumerator WaitClick()
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.AddListener(OnClick);

            var nowCount = eventCount;

            yield return new WaitUntil(() => eventCount != nowCount);

            nextButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveListener(OnClick);
        }
        void OnClick()
        {
            eventCount++;
        }
        IEnumerator WaitAction(InputAction action)
        {
            if (action.WasPerformedThisFrame())
            {
                yield return new WaitUntil(() => action.WasReleasedThisFrame());
            }
            yield return new WaitUntil(() => action.WasPerformedThisFrame());

            yield return new WaitForSeconds(2);
        }
    }
}
