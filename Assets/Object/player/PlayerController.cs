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

                SetCursorLock(true);
            }
        }
        private void Start()
        {
            SceneLoader.Singleton.OnChangePause += OnChangePause;
        }
        private void OnChangePause(bool sw)
        {
            SetCursorLock(!sw);

            if (sw)
            {
                ReLeafInputAction.Disable();
            }
            else
            {
                ReLeafInputAction.Enable();
            }
        }

        void SetCursorLock(bool sw)
        {
            Cursor.visible = !sw;
            if (sw)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public void OnDisable()
        {
            SetCursorLock(false);
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

        [SerializeField]
        float maxMouseSameDelta = 20;
        Vector2 sameDelta = Vector2.zero;

        public void OnAim(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<Vector2>();
            delta.Scale(new(1920.0f / Screen.width, 1080.0f / Screen.height));

            sameDelta += delta;

            if (sameDelta.sqrMagnitude > maxMouseSameDelta * maxMouseSameDelta)
            {
                sameDelta = sameDelta.normalized * maxMouseSameDelta;
                SetItemDir(sameDelta);
            }
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