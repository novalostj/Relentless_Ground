    using System.Collections;
using Combat;
using UnityEngine;
using UnityEngine.Events;
    using UnityEngine.Serialization;

    namespace Enemy.AI
{
    
    public class BaseCombat : MonoBehaviour, ITargetable
    {
        [SerializeField] protected float hitInvulnerableTime = 0.4f;
        [SerializeField] protected float distanceToAttack = 2f;
        [SerializeField] protected float applyDamageOn = 0.9f;
        [FormerlySerializedAs("attackDelay")] [SerializeField] protected float howLongIsAttack = 1.1f;
        [SerializeField] protected float attackCooldownMinimum = 2f, attackCooldownMaximum = 4f;
        [SerializeField] protected float attackRotateSpeed = 2f;
        [SerializeField] protected LayerMask targetLayer;

        protected BaseAI baseAI;
        protected EyeSight eyeSight;
        
        [HideInInspector]
        public UnityEvent hit, hitOver, onAttack, onAttackFinish;

        protected Targets targets;
        protected Coroutine cAttack, cHalt, cooldown;
        protected float attackCooldown;
        
        protected bool CanAttack { get; set; } = true;
        protected bool IsAttacking { get; set; }
        protected bool Vulnerable { get; set; } = true;
        
        protected bool rotateToTarget;

        public float HitInvulnerableTime => hitInvulnerableTime;
        
        
        
        protected virtual void Start()
        {
            attackCooldown = Random.Range(attackCooldownMinimum, attackCooldownMaximum);
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
                (eyeSight.TargetDistance > distanceToAttack || !eyeSight.isAgro)) return;

            cAttack = StartCoroutine(AttackingEvent());
        }

        protected virtual IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(applyDamageOn);
            targets.ApplyDamage(20);
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(howLongIsAttack - applyDamageOn);
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
            yield return new WaitForSeconds(attackCooldown - howLongIsAttack);
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
    }
}
