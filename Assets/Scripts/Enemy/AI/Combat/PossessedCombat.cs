using Enemy.Animation.Particle;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.AI.Combat
{
    [System.Serializable]
    public class MeleeAttack : Attack
    {
        public float radius = 3f;
        public LayerMask targetLayer;
    }
    
    [RequireComponent(typeof(PossessedParticle))]
    public class PossessedCombat : BaseCombat
    {
        [FormerlySerializedAs("attack")]
        [Header("Possessed Variables")]
        [SerializeField] private MeleeAttack meleeAttack;

        protected override Attack CurrentAttack => meleeAttack;
        public float Attack1Scale => meleeAttack.radius;
        public Vector3 LocalAttackPosition => CurrentAttack.localAttackPosition;
        
        protected override void Update()
        {
            if (baseAI.IsDead) return;

            if (IsAttacking || !CanAttack) return;
            
            if (meleeAttack.canPerform && TargetInRangeCheck(meleeAttack.distanceToAttack))
                meleeAttack.attackCoroutine = StartCoroutine(AttackingEvent1(meleeAttack));
        }

    }
}