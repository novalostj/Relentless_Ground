using Enemy.AI.Combat;
using JetBrains.Annotations;
using Particles;
using UnityEngine;

namespace Enemy.Animation.Particle
{
    public class GoblinBeastParticle : BaseParticle
    {
        [SerializeField] private ParticleEnum 
            rangedAttackParticle = ParticleEnum.HitBOrange, 
            meleeAttackParticle = ParticleEnum.HitCWhite;
        [SerializeField] private Transform leftGun, rightGun, attackPosition;
        [SerializeField] private float forwardZPercent = 0.7f;

        private Transform WhichGun => gunSwitch ? rightGun : leftGun;

        private bool gunSwitch;
        private GoblinBeastCombat goblinBeastCombat;
        protected override BaseCombat BaseCombat => goblinBeastCombat;
        

        protected override void Awake()
        {
            goblinBeastCombat = GetComponent<GoblinBeastCombat>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            goblinBeastCombat.onSlam.AddListener(OnAttack1);
            goblinBeastCombat.onBite.AddListener(OnAttack2);
            goblinBeastCombat.onLaunchProjectile.AddListener(OnAttack3);
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            goblinBeastCombat.onSlam.RemoveListener(OnAttack1);
            goblinBeastCombat.onBite.RemoveListener(OnAttack2);
            goblinBeastCombat.onLaunchProjectile.RemoveListener(OnAttack3);
        }

        private void OnAttack3()
        {
            Vector3 gunForward = WhichGun.forward;
            Vector3 newForward = new(gunForward.x, gunForward.y, gunForward.z * forwardZPercent);
            
            GlobalParticleManager.play?.Invoke(rangedAttackParticle, WhichGun.position + newForward);
            gunSwitch = !gunSwitch;
        }

        private void OnAttack1()
        {
            Vector3 atk1Forward = attackPosition.forward;
            Vector3 newForward = new Vector3(atk1Forward.x, atk1Forward.y,atk1Forward.z * forwardZPercent);
            GlobalParticleManager.play?.Invoke(meleeAttackParticle, attackPosition.position + newForward, goblinBeastCombat.Attack1Scale);
        }

        private void OnAttack2()
        {
            Vector3 atk1Forward = attackPosition.forward;
            Vector3 newForward = new Vector3(atk1Forward.x, atk1Forward.y,atk1Forward.z * forwardZPercent);
            GlobalParticleManager.play?.Invoke(meleeAttackParticle, attackPosition.position + newForward, goblinBeastCombat.Attack2Scale);
        }
    }
}