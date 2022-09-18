using System;
using System.Collections;
using Particles;
using Player.Control;
using UnityEngine;

namespace Player.Animations
{
    
    public class PlayerParticle : MonoBehaviour
    {
        [SerializeField] private ParticleEnum
            onHit = ParticleEnum.Blood,
            meleeAttackParticle = ParticleEnum.HitCWhite,
            smokeParticle = ParticleEnum.DashSmoke;

        [SerializeField] private float
            slamRadius = 2f,
            dashSmokeRadius = 2f,
            jumpSmokeRadius = 2f,
            runSmokeInterval = 0.1f,
            runSmokeRadius = 1f,
            dashParticleInterval = 0.1f;

        private PlayerCombat playerCombat;
        private Movement movement;

        private void Awake()
        {
            playerCombat = GetComponent<PlayerCombat>();
            movement = GetComponent<Movement>();
        }

        private void OnEnable()
        {
            PlayerCombat.playerCombatV3 += OnDash;
            PlayerCombat.playerCombatV2 += OnSlam;
            PlayerCombat.onHit += OnHit;
            PlayerCombat.onSlashApply += ForwardAttackParticle;
            PlayerCombat.onSlamApply += OnSlamApply;
            Movement.onJump += OnJump;
        }
        
        private void OnDisable()
        {
            PlayerCombat.playerCombatV3 -= OnDash;
            PlayerCombat.playerCombatV2 -= OnSlam;
            PlayerCombat.onHit -= OnHit;
            PlayerCombat.onSlashApply -= ForwardAttackParticle;
            PlayerCombat.onSlamApply -= OnSlamApply;
            Movement.onJump -= OnJump;
        }

        private void OnHit()
        {
            Vector3 position = transform.position;
            Vector3 bloodParticlePosition =
                new Vector3(position.x, position.y + 1, position.z);
            GlobalParticleManager.play?.Invoke(onHit, bloodParticlePosition);
        }

        private void Start()
        {
            StartCoroutine(DashParticleEffects());
            StartCoroutine(RunParticleEffects());
        }

        private void OnDash()
        {
            PlaySmoke(dashSmokeRadius);
        }

        private void OnSlam()
        {
            GlobalParticleManager.play?.Invoke(ParticleEnum.HitMiscBGravity, transform.position, slamRadius);
        }

        private void ForwardAttackParticle()
        {
            var facingDirection = movement.FacingDirection;
            GlobalParticleManager.play?.Invoke(meleeAttackParticle, transform.position + facingDirection + playerCombat.ForwardCollider.localForwardPosition, playerCombat.ForwardCollider.radius);
        }
        
        private void OnSlamApply()
        {
            GlobalParticleManager.play?.Invoke(meleeAttackParticle, transform.position, playerCombat.SlamCollider.radius);
        }

        private void PlaySmoke(float radius)
        {
            GlobalParticleManager.play?.Invoke(smokeParticle, transform.position, radius);
        }

        private void OnJump()
        {
            PlaySmoke(jumpSmokeRadius);
        }

        private IEnumerator DashParticleEffects()
        {
            while (true)
            {
                yield return new WaitForSeconds(dashParticleInterval);
                if (!playerCombat.IsDashing) continue;

                ForwardAttackParticle();
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator RunParticleEffects()
        {
            while (true)
            {
                yield return new WaitForSeconds(runSmokeInterval);
                if (!movement.IsMoving || !movement.IsGrounded || movement.IsDashing || !movement.IsHoldingRun) continue;
                
                PlaySmoke(runSmokeRadius);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
    }
}