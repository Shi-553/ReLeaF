using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class Scorpion : MonoBehaviour, IRoomEnemy
    {
        [SerializeField]
        int hpMax = 3;
        [SerializeField]
        int hp = 3;
        [SerializeField]
        float speed = 2.0f;
        [SerializeField]
        int atk = 1;
        [SerializeField]
        float attackCoolTime = 1.0f;
        float attackCoolTimeCounter = 0;
        [SerializeField]
        float attackAimTime = 1.0f;
        [SerializeField]
        float attackDuration = 1.0f;
        [SerializeField]
        float attackSpeed = 4.0f;
        [SerializeField]
        float attackKnockBackPower = 4.0f;

        Vision searchVision;
        Vision attackVision;

        [SerializeField]
        float knockBackDampingRate = 0.9f;
        bool isAttack = false;
        bool isAttackDamageNow = false;
        Vector3 attackDir;


        HashSet<Vector3Int> attackedTilePos = new HashSet<Vector3Int>();

        public bool CanAttackPlayer { get; set; }

        Rigidbody2DMover mover;
        private void Awake()
        {
            TryGetComponent(out mover);
        }

        void Start()
        {
            hp = hpMax;
            isAttack = false;
            transform.Find("AttackVision").TryGetComponent(out attackVision);
            transform.Find("SearchVision").TryGetComponent(out searchVision);
            attackedTilePos.Clear();
        }
        void Update()
        {
            if (!CanAttackPlayer)
            {
                return;
            }
            if (attackCoolTimeCounter > 0)
            {
                attackCoolTimeCounter -= Time.deltaTime;
                return;
            }

            if (isAttack)
                return;

            if (attackVision.ShouldFoundTarget)
            {
                isAttack = true;
                StartCoroutine(Attack());
                return;
            }

            if (!searchVision.ShouldFoundTarget)
            {
                return;
            }

            var dir = (searchVision.LastTarget.position - transform.position).normalized;

            mover.Move(speed * dir);
        }

        IEnumerator Attack()
        {
            float counter = 0;
            while (attackAimTime > counter)
            {
                transform.localScale = Vector3.one * MathExtension.LerpPairs(
                        new SortedList<float, float> { { 0, 1 }, { 1, 0.8f } }, counter / attackAimTime);

                counter += Time.deltaTime;
                yield return null;
            }
            counter = 0;


            attackDir = (searchVision.LastTarget.position - transform.position).normalized;
            isAttackDamageNow = true;
            attackedTilePos.Clear();

            while (attackDuration > counter && isAttackDamageNow)
            {
                if (!isAttack)
                {
                    yield break;
                }
                transform.localScale = Vector3.one * MathExtension.LerpPairs(
                        new SortedList<float, float> { { 0, 0.8f }, { 0.1f, 1.0f }, { 1, 1 } }, counter / attackDuration);

                mover.Move(attackSpeed * attackDir);

                counter += Time.deltaTime;
                yield return null;
            }
            isAttackDamageNow = false;
            isAttack = false;
            attackCoolTimeCounter = attackCoolTime;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!isAttackDamageNow)
            {
                return;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
                {
                    player.Damaged(atk, attackDir * attackKnockBackPower);
                    isAttackDamageNow = false;
                }
            }
            if (collision.gameObject.CompareTag("Plant"))
            {
                if (collision.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    plant.Damaged(atk, DamageType.Direct);
                }
            }
        }
        public void Damaged(int damage, Vector3 impulse)
        {
            hp -= damage;
            StartCoroutine(KnockBack(impulse));
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
        IEnumerator KnockBack(Vector3 impulse)
        {
            while (true)
            {
                mover.Move(impulse);

                impulse *= knockBackDampingRate * Time.deltaTime * 60.0f;

                if (impulse.sqrMagnitude < 0.01f)
                {
                    yield break;
                }
                yield return null;
            }
        }
    }
}