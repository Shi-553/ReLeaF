using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace ReLeaf
{
    public class PlayerController : MonoBehaviour, ReLeafInputAction.IPlayerActions
    {

        [SerializeField]
        float moveSpeed = 5;
        [SerializeField]
        float shotMoveSpeed = 2;
        [SerializeField]
        float dashSpeedMagnification = 2;
        [SerializeField]
        float shotSpeed = 3;
        float shotTimeCounter = 0;


        Transform footTransform;

        [SerializeField]
        float knockBackDampingRate = 0.9f;

        [SerializeField]
        FruitContainer fruitContainer;
        public FruitContainer FruitContainer => fruitContainer;

        [SerializeField]
        float dashConsumeStamina = 0.1f;

        [SerializeField]
        ValueGaugeManager hpGauge;
        [SerializeField]
        ValueGaugeManager staminaGauge;

        Rigidbody2DMover mover;

        Vector3Int FootTilePos => DungeonManager.Instance.WorldToTilePos(footTransform.position);
        PlayerInput playerInput;
        ReLeafInputAction reLeafInputAction;

        [SerializeField]
        SelectSeed selectSeed;

        private void Awake()
        {
            TryGetComponent(out mover);
            reLeafInputAction = new ReLeafInputAction();
            TryGetComponent(out playerInput);
            playerInput.defaultActionMap = reLeafInputAction.Player.Get().name;
            playerInput.actions = reLeafInputAction.asset;
            reLeafInputAction.Player.SetCallbacks(this);

        }
        void OnEnable()
        {
            playerInput.ActivateInput();
        }

        void Start()
        {
            footTransform = transform.Find("Foot");
        }

        Vector2 move;
        Vector2Int sowSeedMove;
        Vector2 fireDir;
        bool onFire;
        bool onDash;

        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>().normalized;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            onFire = context.ReadValue<float>() != 0;
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            onDash = context.ReadValue<float>() != 0;
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            Vector3 mouseScreenPos = context.ReadValue<Vector2>();
            mouseScreenPos.z = 10.0f;
            fireDir = Camera.main.ScreenToWorldPoint(mouseScreenPos) - transform.position;
            fireDir.Normalize();
        }

        public void OnSowSeedMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            if (move == Vector2.zero)
            {
                sowSeedMove = Vector2Int.zero;
                return;
            }
            if (Mathf.Abs(move.x) < Mathf.Abs(move.y))
            {
                sowSeedMove = new Vector2Int(0, move.y < 0 ? -1 : 1);
            }
            else
            {
                sowSeedMove = new Vector2Int(move.x < 0 ? -1 : 1, 0);
            }
        }

        public void OnSelectSeed(InputAction.CallbackContext context)
        {
            selectSeed.MoveSelect(context.ReadValue<float>());
        }

        public void OnHarvest(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                StartCoroutine(DroneManager.Instance.BeginSowRoute(transform.position));
            }
            if (context.canceled)
            {
                DroneManager.Instance.EndSowRoute();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            fireDir = context.ReadValue<Vector2>().normalized;
        }

        void Update()
        {
            if (Keyboard.current.escapeKey.isPressed)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
            }
            if (Keyboard.current.f1Key.isPressed)
            {
                SceneManager.LoadScene(0);
            }
            if (hpGauge.Value == 0)
            {
                return;
            }

            if (DroneManager.Instance.IsSowRouting)
            {
                DroneManager.Instance.MoveSowRoute(Vector2Int.CeilToInt(sowSeedMove));
                return;
            }



            var speed = fruitContainer.IsEmpty() ? moveSpeed : shotMoveSpeed;

            if (onDash && staminaGauge.ConsumeValue(dashConsumeStamina * Time.deltaTime))
            {
                speed *= dashSpeedMagnification;
            }

            mover.Move(speed * move);

            DungeonManager.Instance.SowSeed(FootTilePos, PlantType.Foundation);



            if (shotTimeCounter > 0.0f)
            {
                shotTimeCounter -= Time.deltaTime * shotSpeed;
            }
            else
            {
                if (!fruitContainer.IsEmpty() && onFire)
                {
                    if (fruitContainer.Pop(out var f))
                    {
                        f.position = transform.position;

                        shotTimeCounter = f.GetComponent<Fruit>().Shot(fireDir);
                    }
                }
            }
        }

        public void Damaged(float damage, Vector3 impulse)
        {
            if (hpGauge.ConsumeValue(damage))
            {
                StartCoroutine(KnockBack(impulse));

                if (hpGauge.Value == 0)
                {
                    StartCoroutine(Death());
                }
            }
        }
        IEnumerator KnockBack(Vector3 impulse)
        {
            while (true)
            {
                mover.Move(impulse);


                impulse *= knockBackDampingRate;

                if (impulse.sqrMagnitude < 0.01f)
                {
                    yield break;
                }
                yield return null;
            }
        }
        IEnumerator Death()
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            SceneManager.LoadScene(0);
        }

        public void EnterRoom()
        {
            fruitContainer.Clear();
            DroneManager.Instance.Cancel();
        }

    }
}