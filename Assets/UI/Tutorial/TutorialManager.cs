using Pickle;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility;

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
        AudioInfo seRight;

        [SerializeField]
        float autoNextWaitTime = 3;
        [SerializeField]
        float niceWaitTime = 2;

        [SerializeField]
        WallRemover firstWallRemover;
        [SerializeField]
        OnEnterChecker firstEnterChecker;

        [SerializeField]
        WallRemover secoundWallRemover;
        [SerializeField]
        OnEnterChecker secoundEnterChecker;

        [SerializeField]
        OnEnterChecker thirdEnterChecker;

        [SerializeField]
        WallRemover thirdWallRemover;

        [SerializeField, Pickle]
        ItemBase itemPrefab1;
        [SerializeField, Pickle]
        ItemBase itemPrefab2;
        [SerializeField, Pickle]
        ItemBase itemPrefab3;

        [SerializeField]
        SpawnLakeGroup crabSpawn;
        [SerializeField]
        SpawnLakeGroup sharkSpawn;

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
                GameRuleManager.Singleton.OnChangeState -= OnChangeState;

                gameObject.SetActive(true);
                StartCoroutine(TutorialMain());
            }
        }

        IEnumerator TutorialMain()
        {
            crabSpawn.CanSpawn = false;
            crabSpawn.Stop();

            GameRuleManager.Singleton.Pause();
            {
                text.text = "ロボットと協力して砂漠を緑化しよう！";
                yield return WaitButton();
            }

            {
                text.text = "緑化ゲージを満タンにするとクリア！";
                greeningRateMask.SetActive(true);
                yield return WaitButton();
                greeningRateMask.SetActive(false);
            }

            GameRuleManager.Singleton.UnPause();

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
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Dash);
            }

            {
                text.text = "最高！！！！";
                yield return new WaitForSeconds(niceWaitTime);
            }


            {
                firstWallRemover.RemoveWall();
                text.text = "そのまま上に進もう！";
                yield return WaitOnEnter(firstEnterChecker);
            }



            var enemys = crabSpawn.SpawnAllNow();

            GameRuleManager.Singleton.Pause();

            {
                text.text = "湖から敵が湧いちゃった！";
                yield return WaitButton();
            }
            {
                text.text = "攻撃マス<sprite name=ATTACK>は避けよう！";
                yield return WaitButton();
            }

            GameRuleManager.Singleton.UnPause();
            {
                text.text = "弱点マス<sprite name=WEAK>を踏んで倒そう！";
                yield return new WaitUntil(() => enemys.All(e => e == null || e.gameObject == null));
            }

            var itemManager = PlayerController.Singleton.GetComponentInChildren<ItemManager>();

            itemManager.CanUse = false;
            itemManager.CanThrow = false;
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

                text.text = $"{aimActionString}で狙って{useActionString}で湖を緑化しよう！";

                yield return new WaitUntil(() => itemManager.ItemCount == 0);
                yield return new WaitForSeconds(1);

                if (crabSpawn.IsGreening)
                    break;

                text.text = "もう一度！湖を全て緑化しよう！";

                yield return new WaitForSeconds(2);

                AddItem(itemPrefab1);
            }


            {
                text.text = "ブラボー！！敵が湧かなくなったよ！";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            secoundWallRemover.RemoveWall();
            {
                text.text = "右の通路に進もう！";
                yield return WaitOnEnter(secoundEnterChecker);
            }

            GameRuleManager.Singleton.Pause();

            itemManager.CanUse = false;
            itemManager.CanThrow = false;
            {
                AddItem(itemPrefab1);
                AddItem(itemPrefab2);
                AddItem(itemPrefab3);


                text.text = $"アイテムは５つまで持てるよ！";
                yield return WaitButton();
            }

            itemManager.CanThrow = true;
            PlayerController.Singleton.ChangeToPlayer();
            {
                var selectActionString = PlayerController.Singleton.PlayerInput.currentControlScheme == "Gamepad" ?
                $"<sprite name=LEFTSHOLDER><sprite name=RIGHTSHOLDER>" :
                $"<sprite name=SCROLL_Y>";

                var throwActionString = GetActionSpriteTag(PlayerController.Singleton.ReLeafInputAction.Player.ThrowItem);

                text.text = $"{selectActionString}で切り替えて、{throwActionString}で捨ててみよう！";
                yield return new WaitUntil(() => itemManager.Index != 0 || itemManager.ItemCount < 3);
                yield return new WaitForSeconds(3);
            }
            itemManager.CanThrow = false;
            GameRuleManager.Singleton.UnPause();

            {
                text.text = $"OK！！さらに進んでルームに入ろう！";
                yield return WaitOnEnter(thirdEnterChecker);
            }

            PlayerController.Singleton.ChangeToUI();
            {
                text.text = "ルームではブラストゲージが出てくるよ！";
                yield return WaitButton();
            }
            var blastRate = PlayerMover.Singleton.Room.GetComponent<RoomBlastRate>();

            {
                text.text = "緑化してブラストゲージをためて…";
                yield return WaitButton();
                PlayerMover.Singleton.Dir = Vector2.right;
                yield return new WaitUntil(() => blastRate.ValueRate >= 1.0f);
                PlayerMover.Singleton.Dir = Vector2.zero;
            }
            {
                text.text = $"MAXにすると{ACTION_TEXT_COLOR}ルームブラスト{NORMAL_TEXT_COLOR}が発動！";
                yield return WaitButton();
            }

            if (PlayerMover.Singleton.Room.IsRoomBlastNow)
            {
                text.transform.parent.gameObject.SetActive(false);
                yield return new WaitUntil(() => !PlayerMover.Singleton.Room.IsRoomBlastNow);
                text.transform.parent.gameObject.SetActive(true);
            }

            {
                text.text = "積極的に狙ってみよう！";
                yield return WaitButton();
            }
            {
                text.text = "チュートリアルはここまで！";
                yield return WaitButton();
            }
            {
                text.text = "緑化ゲージを満タンにしてクリアしよう！";
                yield return WaitButton();
            }
            itemManager.CanUse = true;
            itemManager.CanThrow = true;


            PlayerController.Singleton.ChangeToPlayer();

            sharkSpawn.Targets.ForEach(t => t.StartSpawnInterval());
            thirdWallRemover.RemoveWall();

            gameObject.SetActive(false);
            GameRuleManager.Singleton.IsWaitFinish = false;

        }
        void AddItem(ItemBase itemPrefab)
        {
            var itemManager = PlayerController.Singleton.GetComponentInChildren<ItemManager>();

            var item = Instantiate(itemPrefab);
            item.Fetch();
            itemManager.AddItem(item);
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

                    displayString = add.ToUpper();
                    break;
                }
            }
            return $"<sprite name={displayString}>";
        }


        IEnumerator WaitOnEnter(OnEnterChecker onEnterChecker)
        {
            onEnterChecker.OnEnter += OnEnter;

            var nowCount = eventCount;

            yield return new WaitUntil(() => eventCount != nowCount);

            onEnterChecker.OnEnter -= OnEnter;
        }
        private void OnEnter(Collider2D obj)
        {
            if (obj.TryGetComponent<PlayerController>(out var playerController))
                eventCount++;
        }



        IEnumerator WaitButton()
        {
            nextButton.gameObject.SetActive(true);

            PlayerController.Singleton.ReLeafInputAction.UI.Submit.started += Submit;
            PlayerController.Singleton.ReLeafInputAction.UI.LeftClick.performed += Submit;
            var nowCount = eventCount;

            yield return null;

            if (EventSystem.current.currentInputModule != null)
            {
                EventSystem.current.currentInputModule.enabled = false;
                EventSystem.current.currentInputModule.enabled = true;
            }
            while (true)
            {
                if (eventCount != nowCount)
                    break;
                yield return null;
            }

            PlayerController.Singleton.ReLeafInputAction.UI.Submit.started -= Submit;
            PlayerController.Singleton.ReLeafInputAction.UI.LeftClick.performed -= Submit;

            nextButton.gameObject.SetActive(false);
        }

        private void Submit(InputAction.CallbackContext obj)
        {
            if (SceneLoader.Singleton.IsPause || obj.ReadValue<float>() == 0)
                return;

            nextButton.OnPointerClick(new(EventSystem.current));
            eventCount++;
        }

        IEnumerator WaitAction(InputAction action)
        {
            yield return new WaitUntil(() => action.IsPressed());

            yield return new WaitForSeconds(2);
        }
    }
}
