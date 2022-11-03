using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace ReLeaf
{
    public class Spines : MonoBehaviour
    {
        [SerializeField]
        float lifeTime = 2.0f;
        float lifeTimeCounter;

        [SerializeField]
        float speed = 1.0f;
        [SerializeField]
        int atk = 1;
        [SerializeField]
        float attackKnockBackPower = 4.0f;


        Rigidbody2DMover mover;

        private void Awake()
        {
            TryGetComponent(out mover);
            lifeTimeCounter = 0;
        }
        void Update()
        {
            lifeTimeCounter += Time.deltaTime;

            if (lifeTimeCounter >= lifeTime)
            {
                Destroy(gameObject);
            }

            mover.Move(transform.up * speed);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
                {
                    player.Damaged(atk, transform.up * attackKnockBackPower);
                    Destroy(gameObject);
                }
            }
            if (collision.gameObject.CompareTag("Plant"))
            {
                if (collision.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    plant.Damaged(atk, DamageType.Shooting);

                    Destroy(gameObject);
                }
            }
            if (collision.gameObject.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}