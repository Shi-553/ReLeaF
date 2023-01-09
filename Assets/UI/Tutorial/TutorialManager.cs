using DebugLogExtension;
using Pickle;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
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

        readonly static string ACTION_TEXT_COLOR = "<color=#1A4FF3>";
        readonly static string NORMAL_TEXT_COLOR = "<color=black>";

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
                var actionString = GetActionSpriteTag(PlayerController.Singleton.ReLeafInputAction.Player.Move);
                text.text = $"まずは{actionString}で歩いて緑化しよう！";
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Move);
            }

            {
                text.text = "いいね！！";
                yield return new WaitForSeconds(niceWaitTime);
            }

            {
                text.text = "歩くと緑化エネルギー(EP)が溜まるよ！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                var actionString = GetActionSpriteTag(PlayerController.Singleton.ReLeafInputAction.Player.Dash);

                text.text = $"移動中に{actionString}でEPを使ってダッシュ！";
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

            itemManager.CanUse = false;
            {
                text.text = "ナイス！！落としたアイテムを拾おう！";

                float time = Time.time;
                yield return new WaitUntil(() => itemManager.ItemCount != 0);
                var duration = Time.time - time;

                // 取るのにかかった時間の分短くする
                yield return new WaitForSeconds(autoNextWaitTime - duration);
            }


            itemManager.CanUse = true;
            while (true)
            {

                var useActionString = GetActionSpriteTag(PlayerController.Singleton.ReLeafInputAction.Player.UseItem);
                var aimActionString = PlayerController.Singleton.PlayerInput.currentControlScheme == "Gamepad" ?
                    GetActionSpriteTag(PlayerController.Singleton.ReLeafInputAction.Player.Look) :
                    $"{ACTION_TEXT_COLOR}マウス{NORMAL_TEXT_COLOR}";

                text.text = $"{aimActionString.DebugLog()}で狙って{useActionString}で湖を緑化しよう！";

                yield return new WaitUntil(() => itemManager.ItemCount == 0);
                yield return new WaitForSeconds(1);

                if (lake.IsGreening)
                    break;

                text.text = "もう一度！湖を全て緑化しよう！";

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

        public string GetActionSpriteTag(InputAction action)
        {
            var displayString = "";

            var currentControlScheme = PlayerController.Singleton.PlayerInput.currentControlScheme;


            var bindingMask = new InputBinding(groups: currentControlScheme, path: null);

            for (var i = 0; i < action.bindings.Count; i++)
            {
                if (bindingMask.Matches(action.bindings[i]))
                {
                    action.GetBindingDisplayString(i, out string device, out var add);

                    displayString += add.ToUpper();
                }
            }

            if (displayString.Length == 4 && new Regex("^[WASD]{4}$", RegexOptions.IgnoreCase).IsMatch(displayString))
                displayString = "WASD";

            return $"<sprite name={displayString}>";
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
