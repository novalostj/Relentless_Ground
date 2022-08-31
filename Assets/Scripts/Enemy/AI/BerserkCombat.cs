using System;
using System.Collections;
using UnityEngine;

namespace Enemy.AI
{
    public class BerserkCombat : BaseCombat
    {
        [SerializeField] private float leapStrength = 10f;
        [SerializeField] private float leapOnTime = 0.3f;
        
        private bool isLeaping;

        private void FixedUpdate()
        {
            if (isLeaping)
            {
                Leap();
            }
        }

        protected override IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(howLongIsAttack));
            rotateToTarget = true;

            yield return new WaitForSeconds(leapOnTime);
            transform.LookAt(eyeSight.Target);
            isLeaping = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(applyDamageOn - leapOnTime);
            targets.ApplyDamage(20);
            isLeaping = false;
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(howLongIsAttack - applyDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }

        private void Leap()
        {
            transform.Translate(Vector3.forward * (leapStrength * Time.fixedDeltaTime));
        }

        public override void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;
            base.ReceiveDamage(value);
            isLeaping = false;
        }

        protected override void Dead()
        {
            CanAttack = false;
            Vulnerable = false;
            IsAttacking = false;
            StopAllCoroutines();
        }
    }
}