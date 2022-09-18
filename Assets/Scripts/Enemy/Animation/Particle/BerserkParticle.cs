using Enemy.AI.Combat;
using Particles;
using UnityEngine;

namespace Enemy.Animation.Particle
{
    public class BerserkParticle : BaseParticle
    {
        private BerserkCombat berserkCombat;

        [SerializeField] private ParticleEnum 
            onLeapParticle = ParticleEnum.DashSmoke,
            onAttackParticle = ParticleEnum.HitCWhite;
        [SerializeField] private float forwardPercentZ = 1f;
        [SerializeField] private float leapScale = 1;

        protected override BaseCombat BaseCombat => berserkCombat;

        protected override void Awake()
        {
            berserkCombat = GetComponent<BerserkCombat>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            berserkCombat.onLeap.AddListener(OnLeap);
            berserkCombat.onMeleeAttackApply.AddListener(OnAttack1);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            berserkCombat.onLeap.RemoveListener(OnLeap);
            berserkCombat.onMeleeAttackApply.RemoveListener(OnAttack1);
        }

        private void OnLeap()
        {
            GlobalParticleManager.play?.Invoke(onLeapParticle, transform.position, leapScale);
        }

        private void OnAttack1()
        {
            var tra = transform;
            GlobalParticleManager.play?.Invoke(onAttackParticle, tra.position + tra.forward + berserkCombat.LocalAttackPosition, berserkCombat.Attack1Scale);
        }
    }
}