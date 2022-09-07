using System;
using _2._5D_Objects;
using Enemy.AI;
using Enemy.AI.Combat;
using Enemy.AI.NewCombat;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Animation
{
    public class BaseAnimation : MonoBehaviour
    {
        private ReBaseCombat reBaseCombat;
        private SpriteRotation spriteRotation;
        private Animator animator;
        private NavMeshAgent agent;
        private BaseAI baseAI;
        
        private static readonly int Front = Animator.StringToHash("Front");
        private static readonly int Side = Animator.StringToHash("Side");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Hit = Animator.StringToHash("OnHit");
        private static readonly int HitOver = Animator.StringToHash("OnHitOver");
        private static readonly int Attack = Animator.StringToHash("OnAttack");
        private static readonly int AttackFinish = Animator.StringToHash("OnAttackFinish");
        private static readonly int Death = Animator.StringToHash("OnDeath");
        private static readonly int Attack2 = Animator.StringToHash("OnAttack2");
        private static readonly int Attack3 = Animator.StringToHash("OnAttack3");

        protected virtual void OnEnable()
        {
            reBaseCombat ??= GetComponent<ReBaseCombat>();
            animator ??= GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            spriteRotation ??= GetComponentInChildren<SpriteRotation>();
            baseAI ??= GetComponent<BaseAI>();
            
            reBaseCombat.hit.AddListener(OnHit);
            reBaseCombat.hitOver.AddListener(OnHitOver);
            reBaseCombat.onAttack.AddListener(OnAttack);
            reBaseCombat.onAttack2.AddListener(OnAttack2);
            reBaseCombat.onAttack3.AddListener(OnAttack3);
            reBaseCombat.onAttackFinish.AddListener(OnAttackFinish);
            baseAI.onDeath.AddListener(OnDeath);    
        }

        protected virtual void OnDisable()
        {
            reBaseCombat.hit.RemoveListener(OnHit);
            reBaseCombat.hitOver.RemoveListener(OnHitOver);
            reBaseCombat.onAttack.RemoveListener(OnAttack);
            reBaseCombat.onAttack2.RemoveListener(OnAttack2);
            reBaseCombat.onAttack3.AddListener(OnAttack3);
            reBaseCombat.onAttackFinish.RemoveListener(OnAttackFinish);
            baseAI.onDeath.RemoveListener(OnDeath);
        }
        
        private void Update()
        {
            SetDirections();
            
            animator.SetFloat(Speed, agent.velocity.magnitude);
        }
        
        private void SetDirections()
        {
            animator.SetFloat(Front, spriteRotation.Front);
            animator.SetFloat(Side, spriteRotation.Side);
        }
        

        private void OnHit() => animator.SetTrigger(Hit);
        private void OnHitOver() => animator.SetTrigger(HitOver);
        private void OnAttack() => animator.SetTrigger(Attack);
        private void OnAttackFinish() => animator.SetTrigger(AttackFinish);
        private void OnDeath() => animator.SetTrigger(Death);
        private void OnAttack2() => animator.SetTrigger(Attack2);
        private void OnAttack3() => animator.SetTrigger(Attack3);
    }
}
