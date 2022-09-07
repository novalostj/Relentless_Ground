using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Enemy.AI.NewCombat;
using UnityEngine;
using UnityEngine.Events;
using Transform = UnityEngine.Transform;

namespace Enemy.AI.Combat
{
    public class OldBaseCombat : MonoBehaviour, ITargetable
    {
        [Header("Base Variables"), SerializeField] 
        private Attack baseAttack;
        
        [SerializeField] protected float hitInvulnerableTime = 0.4f;
        [SerializeField] protected float attackRotateSpeed = 2f;
        [SerializeField] protected LayerMask targetLayer;

        protected BaseAI baseAI;
        protected EyeSight eyeSight;
        
        [HideInInspector]
        public UnityEvent hit, hitOver, onAttack, onAttackFinish;

        protected Targets targets;
        protected Coroutine cAttack, cHalt, cooldown;
        
        protected bool CanAttack { get; set; } = true;
        protected bool IsAttacking { get; set; }
        protected bool Vulnerable { get; set; } = true;
        
        protected bool rotateToTarget;

        public float HitInvulnerableTime => hitInvulnerableTime;
        
        
        
        protected virtual void Start()
        {
            baseAI = GetComponent<BaseAI>();
            eyeSight = GetComponentInChildren<EyeSight>();
            targets = GetComponentInChildren<Targets>();
            baseAI.onDeath.AddListener(Dead);
        }

        protected virtual void Update()
        {
            if (baseAI.IsDead) return;
            
            TargetInRangeCheck();
            RotateToTarget(eyeSight.Target);
        }

        protected virtual void TargetInRangeCheck()
        {
            if (!CanAttack || IsAttacking || 
                (eyeSight.TargetDistance > baseAttack.distanceToAttack || !eyeSight.isAgro)) return;

            cAttack = StartCoroutine(AttackingEvent());
        }

        protected virtual IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(baseAttack.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(baseAttack.applyDamageOn[0]);
            targets.ApplyDamage(20);
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(baseAttack.howLongIsAttack - baseAttack.applyDamageOn[0]);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }
        
        protected virtual void RotateToTarget(Transform target)
        {
            if (!rotateToTarget) return;
            
            Vector3 fp = transform.up;
            Vector3 sp = target.position - transform.position;

            float angle = Vector3.SignedAngle(fp, sp, transform.forward);

            transform.Rotate(0, -angle * attackRotateSpeed * Time.deltaTime,0);
        }
        

        protected virtual IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(baseAttack.AttackCooldown - baseAttack.howLongIsAttack);
            CanAttack = true;
        }

        public virtual void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;

            if (eyeSight.AgroCoroutine != null) 
                eyeSight.StopCoroutine(eyeSight.AgroCoroutine);
            eyeSight.AgroCoroutine = eyeSight.StartCoroutine(eyeSight.AgroTimer());
            
            IsAttacking = false;
            CanAttack = false;
            
            if (cAttack != null) StopCoroutine(cAttack);
            if (cHalt != null) StopCoroutine(cHalt);
            
            baseAI.SetAgentState(false);
            Vulnerable = false;
            baseAI.ReceiveDamage(value);
            hit?.Invoke(); 
            if (!baseAI.IsDead) StartCoroutine(HitIsOverTimer());
        }

        public virtual void ForceReceiveDamage(float value)
        {
            throw new System.NotImplementedException();
        }

        public void DisableMovement(float time)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator DisableMovementCoroutine(float time)
        {
            throw new System.NotImplementedException();
        }

        protected virtual IEnumerator HitIsOverTimer()
        {
            if (cooldown != null) StopCoroutine(cooldown);
            
            yield return new WaitForSeconds(hitInvulnerableTime);

            CanAttack = true;
            Vulnerable = true;
            hitOver?.Invoke();
        }

        protected virtual void Dead()
        {
            StopAllCoroutines();
            CanAttack = false;
            Vulnerable = false;
            IsAttacking = false;
        }
        
        protected List<ITargetable> GetTargets(Vector3 position, float radius)
        {
            var foundColliders = Physics.OverlapSphere(position, radius, targetLayer);
            
            return (from foundCollider in foundColliders let tar = foundCollider.GetComponent<ITargetable>() where foundCollider.transform != transform && tar != null select tar).ToList();
        }

        protected void AttackSphere(Vector3 position, float radius, float damage)
        {
            var foundTargets = GetTargets(position, radius);

            foreach (var iTarget in foundTargets)
                iTarget.ReceiveDamage(damage);
        }
    }
}
