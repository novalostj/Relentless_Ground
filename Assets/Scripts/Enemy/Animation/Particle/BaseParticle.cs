using Enemy.AI.Combat;
using Particles;
using UnityEngine;

namespace Enemy.Animation.Particle
{
    public abstract class BaseParticle : MonoBehaviour
    {
        [SerializeField] protected ParticleEnum onHitParticleEnum = ParticleEnum.Blood;
        
        [SerializeField] protected Vector3 bloodLocalPosition = new(0, 1, 0);
        [SerializeField] protected float bloodScale = 2;

        protected abstract BaseCombat BaseCombat { get; }

        protected abstract void Awake();

        protected virtual  void OnEnable()
        {
            BaseCombat.hit.AddListener(OnHit);
        }

        protected virtual void OnDisable()
        {
            BaseCombat.hit.RemoveListener(OnHit);
        }
        
        private void OnHit()
        {
            GlobalParticleManager.play?.Invoke(onHitParticleEnum, bloodLocalPosition + transform.position, bloodScale);
        }

    }
}