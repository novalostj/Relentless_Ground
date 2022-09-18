using Enemy.AI.Combat;
using Particles;
using UnityEngine;

namespace Enemy.Animation.Particle
{
    public class RangedParticle : BaseParticle
    {
        [SerializeField] protected ParticleEnum onLaunchProjectileParticle;
        [SerializeField] protected float launchForwardZPercent = 0.7f;
        
        
        private BaseCombat baseCombat;
        protected override BaseCombat BaseCombat => baseCombat;
        

        protected override void Awake()
        {
            baseCombat ??= GetComponent<BaseCombat>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BaseCombat.onLaunchProjectile.AddListener(OnLaunchProjectile);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BaseCombat.onLaunchProjectile.RemoveListener(OnLaunchProjectile);
        }

        private void OnLaunchProjectile()
        {
            Vector3 localForward = transform.forward;
            Vector3 newForward = new(localForward.x, localForward.y, localForward.z * launchForwardZPercent);
            
            GlobalParticleManager.play?.Invoke(onLaunchProjectileParticle, bloodLocalPosition + transform.position + newForward);
        }
    }
}