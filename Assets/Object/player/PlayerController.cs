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
        float dashSpeedMagnification = 2;


        Transform footTransform;

        [SerializeField]
        float knockBackDampingRate = 0.9f;

        [SerializeField]
        float dashConsumeEnergy = 0.1f;

        [SerializeField]
        ValueGaugeManager hpGauge;
        [SerializeField]
        ValueGaugeManager energyGauge;

        Rigidbody2DMover mover;

        Vector3Int FootTilePos => DungeonManager.Instance.WorldToTilePos(footTransform.position);
        PlayerInput playerInput;
        ReLeafInputAction reLeafInputAction;


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
        bool onDash;

        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>().normalized;
        }


        public void OnDash(InputAction.CallbackContext context)
        {
            onDash = context.ReadValue<float>() != 0;
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


            var speed = moveSpeed;

            if (onDash && energyGauge.ConsumeValue(dashConsumeEnergy * Time.deltaTime))
            {
                speed *= dashSpeedMagnification;
            }

            mover.Move(speed * move);

            DungeonManager.Instance.SowSeed(FootTilePos, PlantType.Foundation);

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


    }
}