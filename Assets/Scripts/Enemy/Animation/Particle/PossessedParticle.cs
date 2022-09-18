using Enemy.AI.Combat;
using Particles;
using UnityEngine;

namespace Enemy.Animation.Particle
{
    public class PossessedParticle : BaseParticle
    {
        [SerializeField] protected ParticleEnum meleeAttackParticle = ParticleEnum.HitCWhite;
        [SerializeField] protected float attackForwardZ = 0.7f;
        
        private PossessedCombat possessedCombat;
        protected override BaseCombat BaseCombat => possessedCombat;
        
        protected override void Awake()
        {
            possessedCombat = GetComponent<PossessedCombat>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            possessedCombat.onMeleeAttackApply.AddListener(OnAttack);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            possessedCombat.onMeleeAttackApply.RemoveListener(OnAttack);
        }

        private void OnAttack()
        {
            var tra = transform;
            
            GlobalParticleManager.play?.Invoke(meleeAttackParticle, tra.position + tra.forward + possessedCombat.LocalAttackPosition, possessedCombat.Attack1Scale);
        }
    }
}