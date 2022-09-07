using UnityEngine;

namespace Enemy.AI.NewCombat
{
    [System.Serializable]
    public class BerserkAttack : MeleeAttack
    {
        public float leapStrength = 10f;
        public float leapOnTime = 0.3f;
        public bool isLeaping;
    }
    
    public class ReBerserkCombat : ReBaseCombat
    {
        [SerializeField] private BerserkAttack berserkAttack;

        protected override Attack CurrentAttack => berserkAttack;

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
            transform.Translate(Vector3.forward * (berserkAttack.leapStrength * Time.fixedDeltaTime));
        }

        public override void ReceiveDamage(float value)
        {
            if (!Vulnerable || baseAI.IsDead) return;
            
            base.ReceiveDamage(value);
            
            berserkAttack.isLeaping = false;
        }
    }
}