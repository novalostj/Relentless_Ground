using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemy.AI.NewCombat
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
    
    public abstract class ReBaseCombat : MonoBehaviour, ITargetable
    {
        [Header("Base Variables")]
        [SerializeField] protected float hitInvulnerableTime = 0.4f;
        [SerializeField] protected float attackRotateSpeed = 2f;

        protected BaseAI baseAI;
        protected EyeSight eyeSight;

        [HideInInspector] public UnityEvent hit;
        [HideInInspector] public UnityEvent hitOver, onAttack, onAttack2, onAttack3, onAttackFinish;
        
        protected Coroutine haltCoroutine;
        
        protected bool CanAttack { get; set; } = true;
        protected bool IsAttacking { get; set; }
        protected bool Vulnerable { get; set; } = true;
        
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
        public void ForceReceiveDamage(float value)
        {
            
        }

        public void DisableMovement(float time)
        {
            throw new NotImplementedException();
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
            if (!rotateToTarget) return;

            var transform1 = transform;
            Vector3 fp = transform1.up;
            Vector3 sp = target.position - transform1.position;

            float angle = Vector3.SignedAngle(fp, sp, transform1.forward);

            transform.Rotate(0, -angle * attackRotateSpeed * Time.deltaTime,0);
        }
        protected virtual bool TargetInRangeCheck(float distance)
        {
            return CanAttack && !IsAttacking &&
                   !(eyeSight.TargetDistance > distance) && eyeSight.isAgro;
        }
        protected List<ITargetable> GetTargets(Vector3 position, float radius, LayerMask targetLayer)
        {
            var tra = transform;
            Collider[] foundColliders = Physics.OverlapSphere(tra.position + tra.forward + position, radius, targetLayer);

            return (from foundCollider in foundColliders let tar = foundCollider.GetComponent<ITargetable>() where foundCollider.transform != transform && tar != null select tar).ToList();
        }
        protected virtual void AttackSphere(Vector3 position, float radius, float damage, LayerMask targetLayer)
        {
            var foundTargets = GetTargets(position, radius, targetLayer);

            foreach (var iTarget in foundTargets)
                iTarget.ReceiveDamage(damage);
        }

        protected virtual IEnumerator AttackingEvent1(MeleeAttack attack)
        {
            attack.cooldownCoroutine = StartCoroutine(Cooldown(attack.AttackCooldown, attack));
            IsAttacking = true;
            onAttack?.Invoke();
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
            onAttack?.Invoke();
            haltCoroutine = StartCoroutine(baseAI.Halt(attack.howLongIsAttack));
            rotateToTarget = true;

            yield return new WaitForSeconds(attack.leapOnTime);
            transform.LookAt(eyeSight.Target);
            attack.isLeaping = true;
            
            //ApplyDamage
            attack.applyAttackCoroutine = StartCoroutine(SphereAttack(attack, attack.leapOnTime));
            yield return new WaitForSeconds(attack.TotalTimeUsed() - attack.leapOnTime);
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
            onAttack?.Invoke();
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

        protected virtual IEnumerator SphereAttack(MeleeAttack meleeAttack)
        {
            meleeAttack.UsedTime = 0;
            foreach (var time in meleeAttack.applyDamageOn)
            {
                float newTime = time - meleeAttack.UsedTime;
                yield return new WaitForSeconds(newTime);
                meleeAttack.UsedTime += newTime;
                AttackSphere(meleeAttack.localAttackPosition, meleeAttack.radius, meleeAttack.damage, meleeAttack.targetLayer);
            }
            rotateToTarget = false;
        }
        
        protected virtual IEnumerator SphereAttack(MeleeAttack meleeAttack, float startTime)
        {
            meleeAttack.UsedTime = startTime;
            foreach (var newTime in meleeAttack.applyDamageOn.Select(time => time - meleeAttack.UsedTime))
            {
                yield return new WaitForSeconds(newTime);
                meleeAttack.UsedTime += newTime;
                AttackSphere(meleeAttack.localAttackPosition, meleeAttack.radius, meleeAttack.damage, meleeAttack.targetLayer);
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