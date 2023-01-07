using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ReLeaf
{
    public class TutorialRuleManager : GameRuleManager
    {
        [SerializeField]
        TMP_Text text;
        [SerializeField]
        Button nextButton;
        int clickCount = 0;

        string COLOR_YELLOW = "<color=yellow>";

        [SerializeField]
        GameObject greeningRateMask;

        [SerializeField]
        float autoNextWaitTime = 3;
        [SerializeField]
        float niceWaitTime = 2;

        protected override void Start()
        {
            StartCoroutine(TutorialMain());

            clickCount = 0;
            nextButton.onClick.AddListener(OnClick);
        }

        IEnumerator TutorialMain()
        {
            text.text = "���{�b�g�Ƌ��͂��č�����Ή����悤�I";
            yield return WaitClick();


            text.text = "�Ή��Q�[�W�𖞃^���ɂ����\n�X�e�[�W�N���A�I";
            greeningRateMask.SetActive(true);
            yield return WaitClick();
            greeningRateMask.SetActive(false);



            text.text = $"�܂��͕����ėΉ����悤�I\n";
            if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.Move, out var moveDisplayString))
            {
                text.text += $"�ړ��F{COLOR_YELLOW}{moveDisplayString}";
            }
            yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Move);


            text.text = "�����ˁI�I";
            yield return new WaitForSeconds(niceWaitTime);




            text.text = $"�����ƗΉ��G�l���M�[�����܂��I";
            yield return new WaitForSeconds(autoNextWaitTime);


            text.text = $"�G�l���M�[���g���ă_�b�V�����悤�I\n";
            if (TryGetDisplayString(PlayerController.Singleton.ReLeafInputAction.Player.Dash, out var dashDisplayString))
            {
                text.text += $"�_�b�V���F�ړ��{{COLOR_YELLOW}{dashDisplayString}";
            }
            PlayerMover.Singleton.IsDash = false;
            yield return WaitAction(PlayerController.Singleton.ReLeafInputAction.Player.Dash);

            text.text = "�ō��I�I�I�I";
            yield return new WaitForSeconds(niceWaitTime);



            text.text = "���̂܂܏�ɐi�����I";

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


        IEnumerator WaitClick()
        {
            State = GameRuleState.Prepare;
            nextButton.gameObject.SetActive(true);

            var nowCount = clickCount;

            yield return new WaitUntil(() => clickCount != nowCount);

            nextButton.gameObject.SetActive(false);
            State = GameRuleState.Playing;
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
        void OnClick()
        {
            clickCount++;
        }
    }
}
