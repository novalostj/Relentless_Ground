using System;
using _2._5D_Objects;
using Enemy.AI;
using Enemy.AI.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Animation
{
    public class BaseAnimation : MonoBehaviour
    {
        private BaseCombat baseCombat;
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
            baseCombat ??= GetComponent<BaseCombat>();
            animator ??= GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            spriteRotation ??= GetComponentInChildren<SpriteRotation>();
            baseAI ??= GetComponent<BaseAI>();
            
            baseCombat.hit.AddListener(OnHit);
            baseCombat.hitOver.AddListener(OnHitOver);
            baseCombat.onAttack1.AddListener(OnAttack);
            baseCombat.onAttack2.AddListener(OnAttack2);
            baseCombat.onAttack3.AddListener(OnAttack3);
            baseCombat.onAttackFinish.AddListener(OnAttackFinish);
            baseAI.onDeath.AddListener(OnDeath);    
        }

        protected virtual void OnDisable()
        {
            baseCombat.hit.RemoveListener(OnHit);
            baseCombat.hitOver.RemoveListener(OnHitOver);
            baseCombat.onAttack1.RemoveListener(OnAttack);
            baseCombat.onAttack2.RemoveListener(OnAttack2);
            baseCombat.onAttack3.AddListener(OnAttack3);
            baseCombat.onAttackFinish.RemoveListener(OnAttackFinish);
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
