using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace ReLeaf
{
    public class PlayerController : SingletonBase<PlayerController>, ReLeafInputAction.IPlayerActions
    {
        PlayerInput playerInput;
        public PlayerInput PlayerInput => playerInput;


        public ReLeafInputAction ReLeafInputAction { get; private set; }


        ItemManager itemManager;

        PlayerMover mover;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (callByAwake)
            {
                TryGetComponent(out mover);

                ReLeafInputAction = new ReLeafInputAction();
                TryGetComponent(out playerInput);
                PlayerInput.defaultActionMap = ReLeafInputAction.Player.Get().name;
                PlayerInput.actions = ReLeafInputAction.asset;
                ReLeafInputAction.Player.SetCallbacks(this);

                itemManager = GetComponentInChildren<ItemManager>();

            }
        }
        private void Start()
        {
            SceneLoader.Singleton.OnChangePause += OnChangePause;
        }
        private void OnChangePause(bool sw)
        {
            if (sw)
                ReLeafInputAction.Disable();
            else
                ReLeafInputAction.Enable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (SceneLoader.Singleton != null)
                SceneLoader.Singleton.OnChangePause -= OnChangePause;
            ReLeafInputAction?.Disable();
        }

        void SetItemDir(Vector2 value)
        {
            if (value == Vector2.zero)
                return;
            itemManager.ItemDir = value.ClampOneMagnitude();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            mover.Dir = context.ReadValue<Vector2>().normalized;
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.started || context.canceled)
                mover.IsDash = context.ReadValue<float>() != 0;
        }

        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (context.ReadValue<float>() != 0)
            {
                StartCoroutine(itemManager.UseItem());
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
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

    }
}