﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

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
        AttackTransition Transition { get; set; }
        bool IsAttackDamageing => Transition == AttackTransition.Damageing;
        bool IsAttack => Transition != AttackTransition.None;
        EnemyAttackInfo EnemyAttackInfo { get; }

        IEnumerable<Vector2Int> GetAttackRange(Vector2Int pos, Vector2Int dir, bool isDamagableOnly);

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