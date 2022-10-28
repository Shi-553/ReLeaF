using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : MonoBehaviour
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

    Vector2 move;

    Rigidbody2D rigid;
    void Start()
    {
        hp = hpMax;
        isAttack = false;
        transform.Find("AttackVision").TryGetComponent(out attackVision);
        transform.Find("SearchVision").TryGetComponent(out searchVision);
        TryGetComponent(out rigid);
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position+ move);
        move = Vector2.zero;
    }
    void Update()
    {
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
        var dir = (searchVision.Target.position - transform.position).normalized;

        move += new Vector2(
            dir.x * speed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
            dir.y * speed * DungeonManager.CELL_SIZE.y * Time.deltaTime);
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


        attackDir = (searchVision.Target.position - transform.position).normalized;
        isAttackDamageNow = true;

        while (attackDuration > counter&& isAttackDamageNow)
        {
            if (!isAttack)
            {
                yield break;
            }
            transform.localScale = Vector3.one * MathExtension.LerpPairs(
                    new SortedList<float, float> { { 0, 0.8f }, { 0.1f, 1.0f }, { 1, 1 } }, counter / attackDuration);

            move += new Vector2(
                attackDir.x * attackSpeed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
                attackDir.y * attackSpeed * DungeonManager.CELL_SIZE.y * Time.deltaTime);

            counter += Time.deltaTime;
            yield return null;
        }
        isAttackDamageNow = false;
        isAttack = false;
        attackCoolTimeCounter = attackCoolTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttackDamageNow) {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerControler>(out var player))
            {
                player.Damaged(atk,attackDir*attackKnockBackPower);
                isAttackDamageNow = false;
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

            var pos = transform.position;

            pos.x += impulse.x * DungeonManager.CELL_SIZE.x * Time.deltaTime;
            pos.y += impulse.y * DungeonManager.CELL_SIZE.y * Time.deltaTime;

            transform.position = pos;

            impulse *= knockBackDampingRate;

            if (impulse.sqrMagnitude<0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }
}