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
        [SerializeField] private float attackDelay = 1.1f;
        [SerializeField] private float attackCooldown = 2f;

        private BaseAI baseAI;
        private EyeSight eyeSight;
        
        [HideInInspector]
        public UnityEvent hit, hitOver, onAttack, onAttackFinish;

        private Coroutine cAttack, cHalt;
        
        public bool CanAttack { get; private set; } = true;
        public bool IsAttacking { get; private set; }
        public bool Vulnerable { get; private set; } = true;

        private void Start()
        {
            baseAI = GetComponent<BaseAI>();
            eyeSight = GetComponentInChildren<EyeSight>();
        }

        private void Update()
        {
            TargetInRangeCheck();
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
            
            yield return new WaitForSeconds(attackDelay);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(attackCooldown);
            CanAttack = true;
        }

        public void ReceiveDamage()
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
            yield return new WaitForSeconds(hitInvulnerableTime);
            CanAttack = true;
            Vulnerable = true;
            hitOver?.Invoke();
        }
    }
}
