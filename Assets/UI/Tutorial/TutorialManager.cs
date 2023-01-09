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
                text.text = "���{�b�g�Ƌ��͂��č�����Ή����悤�I";
                yield return WaitClick();
            }

            {
                text.text = "�Ή��Q�[�W�𖞃^���ɂ���ƃN���A�I";
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

                text.text += $" {NOMAL_COLOR}�܂��͕����ėΉ����悤�I";
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Move);
            }

            {
                text.text = "�����ˁI�I";
                yield return new WaitForSeconds(niceWaitTime);
            }

            {
                text.text = $"�����ƗΉ��G�l���M�[(EP)�����܂��I";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                text.text = ACTION_DISP_COLOR;

                if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.Dash, out var dashDisplayString))
                {
                    text.text += $"�ړ��{{dashDisplayString}";
                }
                text.text += $" {NOMAL_COLOR}EP���g���ă_�b�V���I";
                PlayerMover.Singleton.IsDash = false;
                yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Dash);
            }

            {
                text.text = "�ō��I�I�I�I";
                yield return new WaitForSeconds(niceWaitTime);
            }


            {
                wallRemover.RemoveWall();
                text.text = "���̂܂܏�ɐi�����I";
                yield return WaitOnEnter();
            }



            var enemys = lake.SpawnAllNow();

            GameRuleManager.Singleton.Pause();

            {
                text.text = "�΂���G���N����������I";
                yield return WaitClick();
            }
            {
                text.text = "�Ԃ��U���}�X�͔����悤�I";
                yield return WaitClick();
            }

            GameRuleManager.Singleton.UnPause();
            {
                text.text = "���F����_�}�X�𓥂�œ|�����I";
                yield return new WaitUntil(() => enemys.All(e => e == null || e.gameObject == null));
            }

            var itemManager = PlayerController.Singleton.GetComponentInChildren<ItemManager>();

            {
                text.text = "�i�C�X�I�I���Ƃ����A�C�e�����E�����I";
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
                text.text += $" {NOMAL_COLOR}�΂ɃA�C�e�����g���Ă݂悤�I";

                yield return new WaitUntil(() => itemManager.ItemCount == 0);
                yield return new WaitForSeconds(1);

                if (lake.IsGreening)
                    break;

                text.text = $"{NOMAL_COLOR}������x�I";

                yield return new WaitForSeconds(2);

                var item = Instantiate(itemPrefab);
                item.Fetch();
                itemManager.AddItem(item);
            }


            {
                text.text = "�u���{�[�I�I�G���N���Ȃ��Ȃ�����I";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            {
                text.text = "�`���[�g���A���͂���ŏI���I";
                yield return new WaitForSeconds(autoNextWaitTime);
            }
            {
                text.text = "�Ή��Q�[�W�𖞃^���ɂ��ďI�����悤�I";
                yield return new WaitForSeconds(autoNextWaitTime);
            }

            gameObject.SetActive(false);
            GameRuleManager.Singleton.IsWaitFinish = false;

        }

        public string ToReadableString(string actionString)
        {
            return actionString
                .Replace("LeftButton", "���N���b�N", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Left", "L ", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Right", "R ", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Trigger", "�g���K�[", System.StringComparison.OrdinalIgnoreCase)
                .Replace("Space", "�X�y�[�X�L�[", System.StringComparison.OrdinalIgnoreCase);
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
