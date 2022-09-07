using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.AI.NewCombat
{
    [System.Serializable]
    public class MeleeAttack : Attack
    {
        public float radius = 3f;
        public LayerMask targetLayer;
    }
    
    public class RePossessedCombat : ReBaseCombat
    {
        [FormerlySerializedAs("attack")]
        [Header("Possessed Variables")]
        [SerializeField] private MeleeAttack meleeAttack;

        protected override Attack CurrentAttack => meleeAttack;
        
        protected override void Update()
        {
            if (baseAI.IsDead) return;

            if (IsAttacking || !CanAttack) return;
            
            if (meleeAttack.canPerform && TargetInRangeCheck(meleeAttack.distanceToAttack))
                meleeAttack.attackCoroutine = StartCoroutine(AttackingEvent1(meleeAttack));
        }

    }
}