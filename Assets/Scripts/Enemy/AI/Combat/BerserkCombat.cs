using Enemy.Animation.Particle;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.AI.Combat
{
    [System.Serializable]
    public class BerserkAttack : MeleeAttack
    {
        public float leapMultiplier = 0.01f;
        public float leapOnTime = 0.3f;
        public bool isLeaping;
        public float maximumVelocity = 40;
        public float FinalVelocity { get; set; }
    }
    
    [RequireComponent(typeof(BerserkParticle))]
    public class BerserkCombat : BaseCombat
    {
        [SerializeField] private BerserkAttack berserkAttack;
        
        protected override Attack CurrentAttack => berserkAttack;
        public float Attack1Scale => berserkAttack.radius;
        public Vector3 LocalAttackPosition => berserkAttack.localAttackPosition;

        protected override void Update()
        {
            if (baseAI.IsDead) return;
            
            if (IsAttacking || !CanAttack) return;
            
            if (berserkAttack.canPerform && TargetInRangeCheck(berserkAttack.distanceToAttack))
                berserkAttack.attackCoroutine = StartCoroutine(AttackingEvent1(berserkAttack));
        }
        
        private void FixedUpdate()
        {
            if (berserkAttack.isLeaping)
            {
                Leap();
            }
        }
        
        private void Leap()
        {
            transform.Translate(Vector3.forward * (berserkAttack.FinalVelocity * berserkAttack.leapMultiplier * Time.fixedDeltaTime));
        }   

        public override void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;
            
            base.ReceiveDamage(value);
            
            berserkAttack.isLeaping = false;
        }
    }
}