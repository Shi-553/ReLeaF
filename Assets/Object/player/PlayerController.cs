using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace ReLeaf
{
    public class PlayerController : SingletonBase<PlayerController>, ReLeafInputAction.IPlayerActions
    {
        PlayerInput playerInput;
        ReLeafInputAction reLeafInputAction;

        ItemManager itemManager;

        PlayerMover mover;

        [SerializeField]
        CinemachineVirtualCamera cinemachine;
        CinemachineFramingTransposer cinemachineFramingTransposer;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (callByAwake)
            {
                TryGetComponent(out mover);

                reLeafInputAction = new ReLeafInputAction();
                TryGetComponent(out playerInput);
                playerInput.defaultActionMap = reLeafInputAction.Player.Get().name;
                playerInput.actions = reLeafInputAction.asset;
                reLeafInputAction.Player.SetCallbacks(this);

                itemManager = GetComponentInChildren<ItemManager>();

                cinemachineFramingTransposer = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();

            }
        }
        private void Start()
        {
            SceneLoader.Singleton.OnChangePause += OnChangePause;
        }
        private void OnChangePause(bool sw)
        {
            if (sw)
                reLeafInputAction.Disable();
            else
                reLeafInputAction.Enable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (SceneLoader.Singleton != null)
                SceneLoader.Singleton.OnChangePause -= OnChangePause;
            reLeafInputAction?.Disable();
        }

        void SetItemDir(Vector2 value)
        {
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
            if (!context.performed)
                return;
            Vector3 mouseScreenPos = context.ReadValue<Vector2>();
            mouseScreenPos.z = cinemachineFramingTransposer.m_CameraDistance;
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

    }
}