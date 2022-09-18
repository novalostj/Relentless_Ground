using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

namespace Enemy.AI.Combat
{
    [Serializable]
    public class Attack
    {
        public float distanceToAttack = 2f;
        public List<float> applyDamageOn;
        public float howLongIsAttack = 1.1f;
        public float attackCooldownMinimum = 2f;
        public float attackCooldownMaximum = 4f;
        public float damage = 20f;
        [FormerlySerializedAs("attackPosition")] public Vector3 localAttackPosition = Vector3.zero;

        public float AttackCooldown => Random.Range(attackCooldownMinimum, attackCooldownMaximum);
        public Coroutine attackCoroutine, cooldownCoroutine, applyAttackCoroutine;
        public bool canPerform = true;
        public float UsedTime { get; set; }

        public float TotalTimeUsed()
        {
            float totalTime = 0;
            
            foreach (var newTime in applyDamageOn.Select(time => time - totalTime))
            {
                totalTime += newTime;
            }

            return totalTime;
        }
    }
    
    public abstract class BaseCombat : MonoBehaviour, ITargetable
    {
        [Header("Base Variables")]
        [SerializeField] protected float hitInvulnerableTime = 0.4f;
        [SerializeField] protected float attackRotateSpeed = 2f;

        protected BaseAI baseAI;
        protected EyeSight eyeSight;

        [HideInInspector] public UnityEvent hit;
        [HideInInspector] public UnityEvent hitOver, onAttack1, onAttack2, onAttack3, onAttackFinish, onLaunchProjectile, onMeleeAttackApply, onLeap;
        
        protected Coroutine haltCoroutine;
        
        protected bool CanAttack { get; set; } = true;
        protected bool IsAttacking { get; set; }
        protected bool Vulnerable { get; set; } = true;
        protected bool IsDead { get; set; }

        protected bool rotateToTarget;

        public float HitInvulnerableTime => hitInvulnerableTime;


        protected abstract Attack CurrentAttack { get; }


        protected virtual void Start()
        {
            baseAI = GetComponent<BaseAI>();
            eyeSight = GetComponentInChildren<EyeSight>();
            baseAI.onDeath.AddListener(Dead);
        }
        protected virtual void Dead()
        {
            StopAllCoroutines();
            IsDead = true;
            CanAttack = false;
            Vulnerable = false;
            IsAttacking = false;
        }

        protected abstract void Update();

        private void LateUpdate()
        {
            RotateToTarget(eyeSight.Target);
        }

        public virtual void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;
            
            ForceReceiveDamage(value);
        }
        public void ForceReceiveDamage(float value)
        {
            if (IsDead) return;
            
            hit?.Invoke();

            if (CurrentAttack != null) StopAttack(CurrentAttack);
            
            if (eyeSight.AgroCoroutine != null) eyeSight.StopCoroutine(eyeSight.AgroCoroutine);
            if (haltCoroutine != null) StopCoroutine(haltCoroutine);
            
            eyeSight.AgroCoroutine = eyeSight.StartCoroutine(eyeSight.AgroTimer());
            
            rotateToTarget = false;
            IsAttacking = false;
            CanAttack = false;
            Vulnerable = false;

            baseAI.SetAgentState(false);
            baseAI.ReceiveDamage(value);
            
            StartCoroutine(HitIsOverTimer());
        }

        public void DisableMovement(float time)
        {
            throw new NotImplementedException();
        }

        public void ForceDisableMovement(float time)
        {
            if (IsDead) return;
        }

        public IEnumerator DisableMovementCoroutine(float time)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerator HitIsOverTimer()
        {
            yield return new WaitForSeconds(hitInvulnerableTime);
            
            CanAttack = true;
            Vulnerable = true;
            hitOver?.Invoke();
        }
        protected virtual void RotateToTarget(Transform target)
        {
            if (!rotateToTarget || !target) return;

            var transform1 = transform;
            Vector3 fp = transform1.up;
            Vector3 sp = target.position - transform1.position;

            float angle = Vector3.SignedAngle(fp, sp, transform1.forward);

            transform.Rotate(0, -angle * attackRotateSpeed * Time.deltaTime,0);
        }
        protected virtual bool TargetInRangeCheck(float distance)
        {
            if (!eyeSight.Target) return false;
            
            return CanAttack && !IsAttacking &&
                   !(eyeSight.TargetDistance > distance) && eyeSight.isAgro;
        }

        protected virtual IEnumerator AttackingEvent1(MeleeAttack attack)
        {
            attack.cooldownCoroutine = StartCoroutine(Cooldown(attack.AttackCooldown, attack));
            IsAttacking = true;
            onAttack1?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(attack.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            attack.applyAttackCoroutine = StartCoroutine(SphereAttack(attack));
            
            //Attack Is Over
            yield return new WaitForSeconds(attack.howLongIsAttack);
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }
        
        protected virtual IEnumerator AttackingEvent1(BerserkAttack attack)
        {
            attack.cooldownCoroutine = StartCoroutine(Cooldown(attack.AttackCooldown, attack));
            IsAttacking = true;
            onAttack1?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(attack.howLongIsAttack));
            rotateToTarget = true;

            
            // Leap
            yield return new WaitForSeconds(attack.leapOnTime);
            transform.LookAt(eyeSight.Target);
            attack.isLeaping = true;
            
            var applyDmgOn = attack.TotalTimeUsed() - attack.leapOnTime;
            var tra = transform;
            var targetDistance = Vector3.Distance(attack.localAttackPosition + tra.forward + tra.position, eyeSight.Target.position);
            attack.FinalVelocity = 
                Mathf.Clamp(MotionInOneDirection.FindFinalVelocity(targetDistance, 0, applyDmgOn), 0, attack.maximumVelocity);
            onLeap?.Invoke();
            
            //ApplyDamage
            attack.applyAttackCoroutine = StartCoroutine(SphereAttack(attack, attack.leapOnTime));
            yield return new WaitForSeconds(applyDmgOn);
            attack.isLeaping = false;
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(attack.howLongIsAttack - (attack.TotalTimeUsed() - attack.leapOnTime));
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }
        
        protected virtual IEnumerator AttackingEvent1(RangedAttack attack)
        {
            attack.cooldownCoroutine = StartCoroutine(Cooldown(attack.AttackCooldown, attack));
            IsAttacking = true;
            onAttack1?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(attack.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            attack.applyAttackCoroutine = StartCoroutine(ProjectileAttack(attack));

            //Attack Is Over
            yield return new WaitForSeconds(attack.howLongIsAttack);
            onAttackFinish?.Invoke();
            IsAttacking = false;
        }
        
        protected void LaunchProjectile(GameObject obj, Vector3 position)
        {
            GameObject instance = Instantiate(obj, position, transform.rotation);

            Destroy(instance, 5f);
        }

        protected IEnumerator Cooldown(float cooldown, Attack attack)
        {
            attack.canPerform = false;
            yield return new WaitForSeconds(cooldown);
            attack.canPerform = true;
        }

        protected virtual IEnumerator SphereAttack(MeleeAttack meleeAttack, float startTime = 0)
        {
            meleeAttack.UsedTime = startTime;
            foreach (var newTime in meleeAttack.applyDamageOn.Select(time => time - meleeAttack.UsedTime))
            {
                yield return new WaitForSeconds(newTime);
                meleeAttack.UsedTime += newTime;
                
                var tra = transform;
                onMeleeAttackApply?.Invoke();
                CombatCollider.AttackSphere(tra.position + tra.forward + meleeAttack.localAttackPosition, meleeAttack.radius, meleeAttack.damage, meleeAttack.targetLayer);
            }
            rotateToTarget = false;
        }

        protected virtual IEnumerator ProjectileAttack(RangedAttack rangedAttack)
        {
            rangedAttack.UsedTime = 0;
            foreach (var newTime in rangedAttack.applyDamageOn.Select(time => time - rangedAttack.UsedTime))
            {
                yield return new WaitForSeconds(newTime);
                rangedAttack.UsedTime += newTime;
                onLaunchProjectile?.Invoke();
                LaunchProjectile(rangedAttack.projectile, transform.position + rangedAttack.localAttackPosition);
            }
            rotateToTarget = false;
        }

        protected void StopAttack(Attack attack)
        {
            if (attack.attackCoroutine != null) StopCoroutine(attack.attackCoroutine);
            if (attack.applyAttackCoroutine != null) StopCoroutine(attack.applyAttackCoroutine);
        }
    }
}