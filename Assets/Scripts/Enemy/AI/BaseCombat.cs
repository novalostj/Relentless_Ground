    using System.Collections;
using Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.AI
{
    public class BaseCombat : MonoBehaviour, ITargetable
    {
        [SerializeField] private float hitInvulnerableTime = 0.4f;
        [SerializeField] private float distanceToAttack = 2f;
        [SerializeField] private float applyDamageOn = 0.9f;
        [SerializeField] private float attackDelay = 1.1f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float attackRotateSpeed = 2f;

        private BaseAI baseAI;
        private EyeSight eyeSight;
        
        [HideInInspector]
        public UnityEvent hit, hitOver, onAttack, onAttackFinish;

        private Targets targets;
        private Coroutine cAttack, cHalt, cooldown;
        
        public bool CanAttack { get; private set; } = true;
        public bool IsAttacking { get; private set; }
        public bool Vulnerable { get; private set; } = true;
        
        private bool rotateToTarget;
        
        

        private void Start()
        {
            baseAI = GetComponent<BaseAI>();
            eyeSight = GetComponentInChildren<EyeSight>();
            targets = GetComponentInChildren<Targets>();
        }

        private void Update()
        {
            TargetInRangeCheck();
            RotateToTarget(eyeSight.Target);
        }

        private void TargetInRangeCheck()
        {
            if (!CanAttack || IsAttacking || 
                (eyeSight.TargetDistance > distanceToAttack || !eyeSight.isAgro)) return;

            cAttack = StartCoroutine(AttackingEvent());
        }

        private IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(attackDelay));
            rotateToTarget = true;
            //ApplyDamage
            yield return new WaitForSeconds(applyDamageOn);
            targets.ApplyDamage(20);
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(attackDelay - applyDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }

        private void RotateToTarget(Transform target)
        {
            if (!rotateToTarget) return;
            
            Vector3 fp = transform.up;
            Vector3 sp = target.position - transform.position;

            float angle = Vector3.SignedAngle(fp, sp, transform.forward);

            transform.Rotate(0, -angle * attackRotateSpeed * Time.deltaTime,0);
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(attackCooldown);
            CanAttack = true;
        }

        public void ReceiveDamage(float value)
        {
            if (!Vulnerable) return;

            if (eyeSight.AgroCoroutine != null) 
                eyeSight.StopCoroutine(eyeSight.AgroCoroutine);
            eyeSight.AgroCoroutine = eyeSight.StartCoroutine(eyeSight.AgroTimer());
            
            IsAttacking = false;
            CanAttack = false;
            
            if (cAttack != null) StopCoroutine(cAttack);
            if (cHalt != null) StopCoroutine(cHalt);
            
            baseAI.SetAgentState(false);
            Vulnerable = false;
            hit?.Invoke();
            StartCoroutine(HitIsOverTimer());
        }

        private IEnumerator HitIsOverTimer()
        {
            if (cooldown != null) StopCoroutine(cooldown);
            
            yield return new WaitForSeconds(hitInvulnerableTime);
            CanAttack = true;
            Vulnerable = true;
            hitOver?.Invoke();
        }
    }
}
