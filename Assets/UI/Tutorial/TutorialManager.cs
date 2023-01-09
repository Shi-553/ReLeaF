using Pickle;
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

        [SerializeField, Pickle]
        ItemBase itemPrefab;

        void Start()
        {
            GameRuleManager.Singleton.IsWaitFinish = true;
            eventCount = 0;
            gameObject.SetActive(false);
            GameRuleManager.Singleton.OnChangeState += OnChangeState;
        }

        private void OnChangeState(GameRuleState obj)
        {
            if (obj == GameRuleState.Playing)
            {
                gameObject.SetActive(true);
                StartCoroutine(TutorialMain());
            }
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



            var enemys = lake.SpawnAllNow();

            GameRuleManager.Singleton.Pause();

            {
                text.text = "湖から敵が湧いちゃった！";
                yield return WaitClick();
            }
            {
                text.text = "赤い攻撃マスは避けよう！";
                yield return WaitClick();
            }

            GameRuleManager.Singleton.UnPause();
            {
                text.text = "黄色い弱点マスを踏んで倒そう！";
                yield return new WaitUntil(() => enemys.All(e => e == null || e.gameObject == null));
            }

            var itemManager = PlayerController.Singleton.GetComponentInChildren<ItemManager>();

            {
                text.text = "ナイス！！落としたアイテムを拾おう！";
                yield return new WaitForSeconds(autoNextWaitTime);
                yield return new WaitUntil(() => itemManager.ItemCount != 0);
            }


            while (true)
            {
                text.text = ACTION_DISP_COLOR;

                if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.UseItem, out var displayString))
                {
                    text.text += $"{displayString}";
                }
                text.text += $" {NOMAL_COLOR}湖にアイテムを使ってみよう！";

                yield return new WaitUntil(() => itemManager.ItemCount == 0);
                yield return new WaitForSeconds(1);

                if (lake.IsGreening)
                    break;

                text.text = $"{NOMAL_COLOR}もう一度！";

                yield return new WaitForSeconds(2);

                var item = Instantiate(itemPrefab);
                item.Fetch();
                itemManager.AddItem(item);
            }


            {
                text.text = "ブラボー！！敵が湧かなくなったよ！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                text.text = "チュートリアルはこれで終わり！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }
            {
                text.text = "緑化ゲージを満タンにして終了しよう！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            gameObject.SetActive(false);
            GameRuleManager.Singleton.IsWaitFinish = false;

        }

        public string ToReadableString(string actionString)
        {
            return actionString
                .Replace("LeftButton", "左クリック", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Left", "L ", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Right", "R ", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Trigger", "トリガー", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Space", "スペースキー", System.StringComparison.OrdinalIgnoreCase);
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

            displayString = ToReadableString(displayString);

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
