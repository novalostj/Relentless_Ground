using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Enemy.AI.NewCombat
{
    [Serializable]
    public class BeastAttackBite : MeleeAttack
    {
        public float heal = 1f;
    }

    [Serializable]
    public class BeastAttackStagger : MeleeAttack
    {
        public float time = 0.4f;
    }
    
    
    public class GoblinBeastCombat : BossCombat
    {
        [SerializeField] private BeastAttackStagger beastAttackSlam;
        [SerializeField] private BeastAttackBite beastAttackBite;
        [SerializeField] private RangedAttack beastAttackRanged;
        

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var tra = transform;
            Vector3 position = tra.position;
            Vector3 forward = tra.forward;
            
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position + forward + beastAttackSlam.localAttackPosition, beastAttackSlam.radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position + forward + beastAttackBite.localAttackPosition, beastAttackBite.radius);
        }
#endif

        protected override Attack CurrentAttack => null;

        protected override void Update()
        {
            if (baseAI.IsDead) return;

            if (IsAttacking || !CanAttack) return;
            
            if (beastAttackSlam.canPerform && TargetInRangeCheck(beastAttackSlam.distanceToAttack))
                beastAttackSlam.attackCoroutine = StartCoroutine(AttackingEvent1());
            else if (beastAttackBite.canPerform && TargetInRangeCheck(beastAttackBite.distanceToAttack))
                beastAttackBite.attackCoroutine = StartCoroutine(AttackingEvent2());
            else if (beastAttackRanged.canPerform && TargetInRangeCheck(beastAttackRanged.distanceToAttack))
                beastAttackRanged.attackCoroutine = StartCoroutine(AttackingEvent3());
        }

        protected override IEnumerator AttackingEvent1()
        {
            beastAttackSlam.cooldownCoroutine = StartCoroutine(Cooldown(beastAttackSlam.AttackCooldown, beastAttackSlam));
            IsAttacking = true;
            onAttack?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(beastAttackSlam.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            beastAttackSlam.applyAttackCoroutine = StartCoroutine(SphereAttack(beastAttackSlam));
            
            //Attack Is Over
            yield return new WaitForSeconds(beastAttackSlam.howLongIsAttack);
            StartCoroutine(AttackInterval(pauseTime));
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }

        protected override IEnumerator AttackingEvent2()
        {
            beastAttackBite.cooldownCoroutine = StartCoroutine(Cooldown(beastAttackBite.AttackCooldown, beastAttackBite));
            IsAttacking = true;
            onAttack2?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(beastAttackSlam.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            beastAttackBite.applyAttackCoroutine = StartCoroutine(SphereAttack(beastAttackBite));
            
            //Attack Is Over
            yield return new WaitForSeconds(beastAttackBite.howLongIsAttack);
            StartCoroutine(AttackInterval(pauseTime));
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }

        protected override IEnumerator AttackingEvent3()
        {
            beastAttackRanged.cooldownCoroutine = StartCoroutine(Cooldown(beastAttackRanged.AttackCooldown, beastAttackRanged));
            IsAttacking = true;
            onAttack3?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(beastAttackRanged.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            beastAttackRanged.applyAttackCoroutine = StartCoroutine(ProjectileAttack(beastAttackRanged));
            
            //Attack Is Over
            yield return new WaitForSeconds(beastAttackRanged.howLongIsAttack);
            StartCoroutine(AttackInterval(pauseTime));
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }
        
        public override void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;
            
            base.ReceiveDamage(value);
            
            StopAttack(beastAttackSlam);
            StopAttack(beastAttackBite);
            StopAttack(beastAttackRanged);
        }

        protected virtual IEnumerator SphereAttack(BeastAttackBite meleeBite)
        {
            meleeBite.UsedTime = 0;
            foreach (var newTime in meleeBite.applyDamageOn.Select(time => time - meleeBite.UsedTime))
            {
                yield return new WaitForSeconds(newTime);
                meleeBite.UsedTime += newTime;
                AttackSphereBite(meleeBite.localAttackPosition, meleeBite.radius, meleeBite.damage, meleeBite.heal, meleeBite.targetLayer);
            }
            rotateToTarget = false;
        }
        
        protected virtual IEnumerator SphereAttack(BeastAttackStagger meleeStagger)
        {
            meleeStagger.UsedTime = 0;
            foreach (var newTime in meleeStagger.applyDamageOn.Select(time => time - meleeStagger.UsedTime))
            {
                yield return new WaitForSeconds(newTime);
                meleeStagger.UsedTime += newTime;
                StaggerAttackSphere(meleeStagger.localAttackPosition, meleeStagger.radius, meleeStagger.damage, meleeStagger.time, meleeStagger.targetLayer);
            }
            rotateToTarget = false;
        }
        
        protected virtual void AttackSphereBite(Vector3 position, float radius, float damage, float heal, LayerMask targetLayer)
        {
            var foundTargets = GetTargets(position, radius, targetLayer);

            foreach (var iTarget in foundTargets)
            {
                iTarget.ReceiveDamage(damage);
                baseAI.EnemyStatus.AddHealth(heal);
            }
        }

        protected virtual void StaggerAttackSphere(Vector3 position, float radius, float damage, float time, LayerMask targetLayer)
        {
            var foundTargets = GetTargets(position, radius, targetLayer);

            foreach (var iTarget in foundTargets)
            {
                iTarget.DisableMovement(time);
                iTarget.ReceiveDamage(damage);
            }
        }
        
        
    }
}