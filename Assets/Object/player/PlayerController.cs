using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ReLeaf
{
    public class PlayerController : MonoBehaviour, ReLeafInputAction.IPlayerActions
    {


        [SerializeField]
        ValueGaugeManager hpGauge;


        PlayerInput playerInput;
        ReLeafInputAction reLeafInputAction;

        ItemManager itemManager;

        PlayerMover mover;

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
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            mover.IsDash = context.ReadValue<float>() != 0;
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
                FindObjectOfType<AllGreening>().StartGreening(mover.TilePos);
            }



        }

        public void Damaged(float damage, Vector3 impulse)
        {
            if (hpGauge.ConsumeValue(damage))
            {
                StartCoroutine(mover.KnockBack(impulse));

                if (hpGauge.Value == 0)
                {
                    StartCoroutine(Death());
                }
            }
        }
        IEnumerator Death()
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitUntil(() => Mouse.current.leftButton.wasPressedThisFrame);
            SceneManager.LoadScene(0);
        }


    }
}