using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using UnityEngine;

namespace Player.Control
{
    [Serializable]
    public class SphereColliders
    {
        public Vector3 localForwardPosition = Vector3.zero;
        public float radius = 1.5f;
        public LayerMask layerMask;

        public Collider[] GetColliders(Vector3 position, Vector3 forward) =>
            Physics.OverlapSphere(position + forward + localForwardPosition, radius, layerMask);

        public List<ITargetable> GetTargets(Vector3 position, Vector3 forward)
        {
            var colliders = GetColliders(position, forward);

            return (from collider in colliders select collider.GetComponent<ITargetable>()).ToList();
        }
    }
    
    public class PlayerCombat : MonoBehaviour, ITargetable
    {
        public delegate bool PlayerEnergyUse<in T>(T value);
        public delegate void PlayerCombatEvent();
        public delegate void PlayerCombatEvent<in T>(T value);
        
        public static PlayerCombatEvent playerCombatV1;
        public static PlayerCombatEvent playerCombatV2; 
        public static PlayerCombatEvent playerCombatV3;
        public static PlayerCombatEvent<float> playerCombatV3Float;
        public static PlayerCombatEvent onFinished;
        public static PlayerCombatEvent onHit;
        public static PlayerCombatEvent<float> onHitFloat;
        
        public static PlayerEnergyUse<float> energyConsumption;

        [SerializeField] private float onAttackTime = 0.7f;
        [SerializeField] private float hitTime = 0.4f;
        [SerializeField] private SphereColliders forwardCollider, slamCollider;
        [SerializeField] private Movement movement;
        
        [Header("Slash")] 
        [SerializeField] private float slashCooldown = 1f;
        [SerializeField] private float applySlashDamageOn = 0.2f;
        [SerializeField] private float slashCost = 10f;

        [Header("Slam")] 
        [SerializeField] private float slamCooldown = 3f;
        [SerializeField] private float slamWeight = -0.5f;
        [SerializeField] private float slamCost = 25f;
        [SerializeField] private float applySlamDamageOn = 0.4f;

        [Header("Dash")] 
        [SerializeField] private float dashCooldown = 3f;
        [SerializeField] private float onDashTime = 0.4f;
        [SerializeField] private float dashCost = 40f;
        
        private float timer, dashTimer;
        private Coroutine applyForwardDamage, vulnerableInBetween, hitCoroutine;

        public bool IsAttacking => timer > 0;
        public bool IsDashing => dashTimer > 0;
        public bool CanDash { get; private set; } = true;
        public bool CanSlam { get; private set; } = true;
        public bool CanSlash { get; private set; } = true;
        public bool Vulnerable { get; private set; } = true;
        public bool CrowdControl { get; private set; }
        private bool CanAttack => timer <= 0 && !CrowdControl;
        
        private bool CanAttackV1 => movement.IsGrounded && movement.CanMove && CanSlash;
        private bool CanAttackV2 => !movement.IsGrounded && movement.CanMove && CanSlam;
        private bool CanAttackV3 => movement.IsMoving && movement.IsHoldingRun && !movement.IsDashing && CanDash;

        private void OnEnable()
        {
            InputDetection.onAttack += Attack;
            InputDetection.onSecondAttack += SecondAttack;
        }

        private void OnDisable()
        {
            InputDetection.onAttack -= Attack;
            InputDetection.onSecondAttack -= SecondAttack;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            var facingDirection = movement.FacingDirection;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position + facingDirection + slamCollider.localForwardPosition, slamCollider.radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position + facingDirection + forwardCollider.localForwardPosition, forwardCollider.radius);
        }
#endif

        private void Update()
        {
            SetTimer();
        }

        private void FixedUpdate()
        {
            ApplyForwardDamage();
        }

        private void SetTimer()
        {
            if (timer == 0) return;
            
            timer = Mathf.Clamp(timer - Time.deltaTime, 0, 20);
            dashTimer = Mathf.Clamp(dashTimer - Time.deltaTime, 0, 20);
            
            if (timer == 0) onFinished?.Invoke();
        }

        private void Attack()
        {   
            if (!CanAttack) return;
            
            if (CanAttackV1)
            {
                bool? hasEnergy = energyConsumption?.Invoke(slashCost);
                
                if (hasEnergy is false) return; 
                
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = onAttackTime;
                playerCombatV1?.Invoke();
                applyForwardDamage = StartCoroutine(ApplyForwardDamageOn(applySlashDamageOn));
                StartCoroutine(SlashTimer(slashCooldown));
                StartCoroutine(InvulnerableTimer(onAttackTime));
            }
            else if (CanAttackV2)
            {
                bool? hasEnergy = energyConsumption?.Invoke(slamCost);
                
                if (hasEnergy is false) return; 
                
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = onAttackTime;
                movement.ApplyMotion(new Vector3(0, slamWeight, 0));
                playerCombatV2?.Invoke();
                StartCoroutine(ApplyCenterDamageOn(applySlamDamageOn));
                StartCoroutine(SlamTimer(slamCooldown));
                StartCoroutine(InvulnerableTimer(onAttackTime));
            }
        }

        private IEnumerator InvulnerableTimer(float time)
        {
            Vulnerable = false;
            yield return new WaitForSeconds(time);
            Vulnerable = true;
        }

        private void SecondAttack()
        {
            if (!CanAttack) return;
            
            if (CanAttackV3)
            {
                bool? hasEnergy = energyConsumption?.Invoke(dashCost);
                
                if (hasEnergy is false) return; 
                
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = onDashTime;
                dashTimer = onDashTime;
                playerCombatV3?.Invoke();
                playerCombatV3Float?.Invoke(onDashTime);
                StartCoroutine(DashCooldown(dashCooldown));
                StartCoroutine(InvulnerableTimer(onDashTime));
            }
        }

        private IEnumerator DashCooldown(float time)
        {
            CanDash = false;
            yield return new WaitForSeconds(time);
            CanDash = true;
        }

        private IEnumerator ApplyForwardDamageOn(float time)
        {
            yield return new WaitForSeconds(time);
            var targets = forwardCollider.GetTargets(transform.position, movement.FacingDirection);

            foreach (var target in targets)
                target.ReceiveDamage(1);
        }

        private IEnumerator ApplyCenterDamageOn(float time)
        {
            yield return new WaitForSeconds(time);
            var targets = slamCollider.GetTargets(transform.position, Vector3.zero);

            foreach (var target in targets)
                target.ReceiveDamage(1);
        }

        private IEnumerator SlamTimer(float time)
        {
            CanSlam = false;
            yield return new WaitForSeconds(time);
            CanSlam = true;
        }

        private IEnumerator SlashTimer(float time)
        {
            CanSlash = false;
            yield return new WaitForSeconds(time);
            CanSlash = true;
        }

        private void ApplyForwardDamage()
        {
            if (!IsDashing) return;
            
            var targets = forwardCollider.GetTargets(movement.transform.position, movement.MovementDirection);

            foreach (var target in targets)
                target.ReceiveDamage(1);
        }

        private IEnumerator HitIsOverTimer()
        {
            Vulnerable = false;

            yield return new WaitForSeconds(hitTime);

            onFinished?.Invoke();
            Vulnerable = true;
        }

        public void ReceiveDamage(float value)
        {
            if (!Vulnerable) return;
            
            if (applyForwardDamage != null) StopCoroutine(applyForwardDamage);
            if (movement.fallToStand != null) movement.StopCoroutine(movement.fallToStand);
            if (vulnerableInBetween != null) StopCoroutine(vulnerableInBetween);
            
            onHit?.Invoke();
            onHitFloat?.Invoke(value);
            hitCoroutine = StartCoroutine(HitIsOverTimer());
        }

        public void ForceReceiveDamage(float value)
        {
            
        }

        public void DisableMovement(float time)
        {
            if (!Vulnerable) return;
            StartCoroutine(DisableMovementCoroutine(time));
        }

        public IEnumerator DisableMovementCoroutine(float time)
        {
            CrowdControl = true;
            yield return new WaitForSeconds(time);
            CrowdControl = false;
        }
    }
}