using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;

namespace ReLeaf
{
    public class PlayerController : MonoBehaviour, ReLeafInputAction.IPlayerActions
    {
        PlayerInput playerInput;
        ReLeafInputAction reLeafInputAction;

        ItemManager itemManager;

        PlayerMover mover;


        [SerializeField]
        AudioClip sePlayerSandWalk;
        [SerializeField]
        AudioClip sePlayerSandDash;

        AudioSource asPlayerSandWalk;
        AudioSource asPlayerSandDash;

        private void Awake()
        {
            TryGetComponent(out mover);

            reLeafInputAction = new ReLeafInputAction();
            TryGetComponent(out playerInput);
            playerInput.defaultActionMap = reLeafInputAction.Player.Get().name;
            playerInput.actions = reLeafInputAction.asset;
            reLeafInputAction.Player.SetCallbacks(this);

            itemManager = GetComponentInChildren<ItemManager>();

        }

        void OnDisable()
        {
            reLeafInputAction.Disable();
        }
        void SetItemDir(Vector2 value)
        {
            itemManager.ItemDir = value.ClampOneMagnitude();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            mover.Move = context.ReadValue<Vector2>().normalized;
            if (context.started == true)
            {
                asPlayerSandWalk = SEManager.Singleton.PlayLoop(sePlayerSandWalk, transform, 1.0f);
            }
            if (context.canceled == true)
            {
                asPlayerSandWalk.Stop();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            mover.IsDash = context.ReadValue<float>() != 0;
            if (context.started == true)
            {
                asPlayerSandDash = SEManager.Singleton.PlayLoop(sePlayerSandDash, transform, 1.0f);
            }
            if (context.canceled == true)
            {
                asPlayerSandDash.Stop();
            }
        }

        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (context.ReadValue<float>() != 0)
            {
                itemManager.UseItem();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Vector3 mouseScreenPos = context.ReadValue<Vector2>();
            mouseScreenPos.z = 10.0f;
            SetItemDir(Camera.main.ScreenToWorldPoint(mouseScreenPos) - transform.position);
        }

        public void OnSelectItem(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            var selectMove = context.ReadValue<float>();
            if (selectMove < 0)
            {
                itemManager.SelectMoveLeft();
            }
            else
            {
                itemManager.SelectMoveRight();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            SetItemDir(context.ReadValue<Vector2>());
        }


        void Update()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
            }
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene(0);
            }
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                GameRuleManager.Singleton.Finish(true);
            }

        }
    }
}