using System;
using _2._5D_Objects;
using Enemy.AI;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Animation
{
    public class BaseAnimation : MonoBehaviour
    {
        public BaseCombat baseCombat;
        public SpriteRotation spriteRotation;
        public Animator animator;
        public NavMeshAgent agent;
        public BaseAI baseAI;
        
        private static readonly int Front = Animator.StringToHash("Front");
        private static readonly int Side = Animator.StringToHash("Side");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Hit = Animator.StringToHash("OnHit");
        private static readonly int HitOver = Animator.StringToHash("OnHitOver");
        private static readonly int Attack = Animator.StringToHash("OnAttack");
        private static readonly int AttackFinish = Animator.StringToHash("OnAttackFinish");
        private static readonly int Death = Animator.StringToHash("OnDeath");

        private void OnEnable()
        {
            baseCombat.hit.AddListener(OnHit);  
            baseCombat.hitOver.AddListener(OnHitOver);
            baseCombat.onAttack.AddListener(OnAttack);
            baseCombat.onAttackFinish.AddListener(OnAttackFinish);
            baseAI.onDeath.AddListener(OnDeath);
        }

        private void OnDisable()
        {
            baseCombat.hit.RemoveListener(OnHit);
            baseCombat.hitOver.RemoveListener(OnHitOver);
            baseCombat.onAttack.RemoveListener(OnAttack);
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
    }
}
