using System.Collections;
using Enemy.AI.NewCombat;
using UnityEngine;

namespace Enemy.AI.Combat
{
    public class BerserkCombat : OldBaseCombat
    {
        [System.Serializable]
        public class BerserkAttack : Attack
        {
            public float leapStrength = 10f;
            public float leapOnTime = 0.3f;
        }


        [Header("Berserk Variables"), SerializeField]
        private BerserkAttack berserkAttack;
        
        
        private bool isLeaping;

        /*
        protected override IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(berserkAttack.howLongIsAttack));
            rotateToTarget = true;

            yield return new WaitForSeconds(berserkAttack.leapOnTime);
            transform.LookAt(eyeSight.Target);
            isLeaping = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(berserkAttack.applyDamageOn - berserkAttack.leapOnTime);
            targets.ApplyDamage(20);
            isLeaping = false;
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(berserkAttack.howLongIsAttack - berserkAttack.applyDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }
        */

        private void Leap()
        {
            transform.Translate(Vector3.forward * (berserkAttack.leapStrength * Time.fixedDeltaTime));
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