using Enemy.Animation.Particle;
using UnityEngine;

namespace Enemy.AI.Combat
{
    
    [RequireComponent(typeof(RangedParticle))]
    public class RangedSlingCombat : BaseCombat
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