using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ReLeaf
{
    public class Fruit : MonoBehaviour
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

        [SerializeField]
        float diffusion = 10;
        [SerializeField]
        float shotInterval = 1;

        public bool IsAttack { get; private set; }

        Vector2 dir;
        Rigidbody2DMover mover;

        private void Awake()
        {
            TryGetComponent(out mover);
            IsAttack = false;
            lifeTimeCounter = 0;
        }

        public void SteppedOn()
        {
            Destroy(gameObject);
        }
        public void Highlight(bool sw)
        {
            transform.GetChild(0).gameObject.SetActive(sw);
        }
        public float Shot(Vector2 dir)
        {
            this.dir = Quaternion.Euler(0, 0, Random.Range(-diffusion, diffusion)) * dir;
            IsAttack = true;
            return shotInterval;
        }

        void Update()
        {
            if (!IsAttack)
            {
                return;
            }

            lifeTimeCounter += Time.deltaTime;

            if (lifeTimeCounter >= lifeTime)
            {
                Destroy(gameObject);
            }

            mover.Move(speed * dir);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsAttack)
            {
                return;
            }
            if (collision.CompareTag("Enemy"))
            {
                if (collision.TryGetComponent<cactus>(out var c))
                {
                    c.Damaged(atk);
                    Destroy(gameObject);
                }
                if (collision.TryGetComponent<Scorpion>(out var s))
                {
                    s.Damaged(atk, dir * attackKnockBackPower);
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