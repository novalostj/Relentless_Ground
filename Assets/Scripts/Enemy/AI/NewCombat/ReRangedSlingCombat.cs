using UnityEngine;

namespace Enemy.AI.NewCombat
{
    
    public class ReRangedSlingCombat : ReBaseCombat
    {
        [SerializeField] private RangedAttack doubleRangedAttack;

        protected override Attack CurrentAttack => doubleRangedAttack;
        
        protected override void Update()
        {
            if (baseAI.IsDead) return;

            if (IsAttacking || !CanAttack) return;
            
            if (doubleRangedAttack.canPerform && TargetInRangeCheck(doubleRangedAttack.distanceToAttack))
                doubleRangedAttack.attackCoroutine = StartCoroutine(AttackingEvent1(doubleRangedAttack));
        }

    }
}