using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace ReLeaf
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        float moveSpeed = 5;
        [SerializeField]
        float shotMoveSpeed = 2;
        [SerializeField]
        float shotSpeed = 3;
        float shotTimeCounter = 0;


        Transform footTransform;

        [SerializeField]
        int hpMax = 10;
        [SerializeField]
        int hp = 10;
        [SerializeField]
        float knockBackDampingRate = 0.9f;

        [SerializeField]
        FruitContainer fruitContainer;
        public FruitContainer FruitContainer => fruitContainer;

        [SerializeField]
        Text text;


        Rigidbody2DMover mover;
        private void Awake()
        {
            TryGetComponent(out mover);
            hp = hpMax;
        }
        void Start()
        {
            footTransform = transform.Find("Foot");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadScene(0);
            }
            if (hp == 0)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                DroneManager.Instance.BeginSowRoute(transform.position);
            }
            if (Input.GetMouseButtonUp(1))
            {
                DroneManager.Instance.EndSowRoute();
            }

            Vector2 add = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                add += Vector2.up;
            }
            if (Input.GetKey(KeyCode.A))
            {
                add += Vector2.left;
            }
            if (Input.GetKey(KeyCode.S))
            {
                add += Vector2.down;
            }
            if (Input.GetKey(KeyCode.D))
            {
                add += Vector2.right;
            }
            if (add != Vector2.zero)
            {
                if (DroneManager.Instance.IsSowRouting)
                {
                    DroneManager.Instance.MoveSowRoute(Vector2Int.CeilToInt(add));
                    return;
                }

                add.Normalize();


                var speed = fruitContainer.IsEmpty() ? moveSpeed : shotMoveSpeed;


                mover.Move(speed * add);

                DungeonManager.Instance.SowSeed(footTransform.position, PlantType.Foundation);
            }


            if (shotTimeCounter > 0.0f)
            {
                shotTimeCounter -= Time.deltaTime * shotSpeed;
            }
            else
            {
                if (!fruitContainer.IsEmpty() && Input.GetMouseButton(0))
                {
                    if (fruitContainer.Pop(out var f))
                    {
                        f.position = transform.position;
                        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                        dir.Normalize();

                        shotTimeCounter = f.GetComponent<Fruit>().Shot(dir);
                    }
                }
            }
            text.text = fruitContainer.FruitCount().ToString();
        }

        public void Damaged(int damage, Vector3 impulse)
        {
            if (hp == 0)
                return;

            hp -= damage;
            StartCoroutine(KnockBack(impulse));
            if (hp <= 0)
            {
                hp = 0;
                StartCoroutine(Death());
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