using System.Collections;
using UnityEngine;

namespace ReLeaf
{
    public enum AttackTransition
    {
        None,
        Aiming,
        Damageing,
        CoolTime
    }
    public interface IEnemyAttacker
    {
        AttackTransition Transition { get;  set; }
        bool IsAttackDamageing => Transition == AttackTransition.Damageing;
        bool IsAttack => Transition != AttackTransition.None;
        EnemyAttackInfo EnemyAttackInfo { get; }


         protected void OnStartAiming()
        {
        }
        protected IEnumerator OnStartDamageing();
         protected void OnStartCoolTime()
        {
        }
         protected void OnEndCoolTime()
        {
        }

        IEnumerator Attack()
        {
            Transition = AttackTransition.Aiming;
            OnStartAiming();

            yield return new WaitForSeconds(EnemyAttackInfo.AimTime);

            Transition = AttackTransition.Damageing;
            yield return GlobalCoroutine.StartCoroutine(OnStartDamageing());

            Transition = AttackTransition.CoolTime;
            yield return new WaitUntil(() => Transition == AttackTransition.CoolTime);

            OnStartCoolTime();

            yield return new WaitForSeconds(EnemyAttackInfo.CoolTime);

            Transition = AttackTransition.None;
            OnEndCoolTime();
        }
    }
}