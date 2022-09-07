using UnityEngine;

namespace Enemy.AI.NewCombat
{
    [System.Serializable]
    public class RangedAttack : Attack
    {
        public GameObject projectile;
    }
    
    public class ReRangedCombat : ReBaseCombat
    {
        [Header("Ranged Variables")]
        [SerializeField] private RangedAttack rangedAttack;

        protected override Attack CurrentAttack => rangedAttack;
        
        protected override void Update()
        {
            if (baseAI.IsDead) return;

            if (IsAttacking || !CanAttack) return;
            
            if (rangedAttack.canPerform && TargetInRangeCheck(rangedAttack.distanceToAttack))
                rangedAttack.attackCoroutine = StartCoroutine(AttackingEvent1(rangedAttack));
        }

    }
}