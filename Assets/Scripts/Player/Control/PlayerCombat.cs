using System.Collections;
using Combat;
using Stats;
using UnityEngine;
using Utility;

namespace Player.Control
{
    public class PlayerCombat : MonoBehaviour, ITargetable
    {
        public delegate bool PlayerEnergyUse<in T>(T value);
        public delegate void PlayerCombatEvent();
        public delegate void PlayerCombatEvent<in T>(T value);
        
        public static PlayerCombatEvent 
            onHit, 
            onSlashApply,
            onSlamApply,
            onFinished,
            playerCombatV1,
            playerCombatV2,
            playerCombatV3;
        public static PlayerCombatEvent<float> 
            playerCombatV3Float,
            onHitFloat;
        
        public static PlayerEnergyUse<float> energyConsumption;

        [SerializeField] private float onAttackTime = 0.7f;
        [SerializeField] private float hitTime = 0.4f;
        [SerializeField] private Movement movement;
        [SerializeField] private PlayerStatus playerStatus;

        private Attack Slash => playerStatus.Slash;
        private Slam Slam => playerStatus.Slam;
        private Dash Dash => playerStatus.Dash;
        private float timer, dashTimer;
        private Coroutine applyForwardDamage, vulnerableInBetween, hitCoroutine;
        private Coroutine disableMovementCoroutine;

        public bool IsAttacking => timer > 0;
        public bool IsDashing => dashTimer > 0;
        public bool CanDash { get; private set; } = true;
        public bool CanSlam { get; private set; } = true;
        public bool CanSlash { get; private set; } = true;
        public bool Vulnerable { get; private set; } = true;
        public bool CrowdControl { get; private set; }
        private bool CanAttack => timer <= 0 && !CrowdControl && !IsDead;
        private bool IsDead { get; set; }
        public SphereColliders ForwardCollider => playerStatus.ForwardCollider;
        public SphereColliders SlamCollider => playerStatus.SlamCollider;

        private bool CanAttackV1 => movement.IsGrounded && movement.CanMove && CanSlash;
        private bool CanAttackV2 => !movement.IsGrounded && movement.CanMove && CanSlam;
        private bool CanAttackV3 => movement.IsMoving && !movement.IsDashing && CanDash;

        private void OnEnable()
        {
            InputDetection.onAttack += Attack;
            InputDetection.onSecondAttack += SecondAttack;
            PlayerStatus.noHealth += PlayerIsDead;
        }

        private void OnDisable()
        {
            InputDetection.onAttack -= Attack;
            InputDetection.onSecondAttack -= SecondAttack;
            PlayerStatus.noHealth -= PlayerIsDead;
        }

#if UNITY_EDITOR
        public bool displayDebug;
        
        private void OnDrawGizmosSelected()
        {
            if (!displayDebug) return;
            
            var position = transform.position;
            var facingDirection = movement.FacingDirection;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position + facingDirection + SlamCollider.localForwardPosition, SlamCollider.radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position + facingDirection + ForwardCollider.localForwardPosition, ForwardCollider.radius);
        }
#endif

        private void Update()
        {
            SetTimer();
        }

        private void FixedUpdate()
        {
            ApplyDashDamage();
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
                bool? hasEnergy = energyConsumption?.Invoke(Slash.energyCost);
                
                if (hasEnergy is false) return; 
                
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = onAttackTime;
                playerCombatV1?.Invoke();
                applyForwardDamage = StartCoroutine(ApplyForwardDamageOn(Slash.applyDamageOn, Slash.damage));
                StartCoroutine(SlashTimer(Slash.cooldown));
                StartCoroutine(InvulnerableTimer(onAttackTime));
            }
            else if (CanAttackV2)
            {
                bool? hasEnergy = energyConsumption?.Invoke(Slam.energyCost);
                
                if (hasEnergy is false) return;

                var position = transform.position;
                Physics.Raycast(position, transform.TransformDirection(Vector3.down), out var hit, 99, movement.GroundLayer);
           
                float distance = (position - hit.point).magnitude;
                float finaVelocity = -MotionInOneDirection.FindFinalVelocity(distance, movement.Gravity, Slam.applyDamageOn);

                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = onAttackTime;
                movement.ApplyMotion(new Vector3(0, finaVelocity * Slam.voMultiplier, 0));
                playerCombatV2?.Invoke();
                
                StartCoroutine(ApplyCenterDamageOn(Slam.applyDamageOn, Slam.damage));
                StartCoroutine(SlamTimer(Slam.cooldown));
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
                bool? hasEnergy = energyConsumption?.Invoke(Dash.energyCost);
                
                if (hasEnergy is false) return; 
                
                if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                timer = Dash.time;
                dashTimer = Dash.time;
                playerCombatV3?.Invoke();
                playerCombatV3Float?.Invoke(Dash.time);
                StartCoroutine(DashCooldown(Dash.cooldown));
                StartCoroutine(InvulnerableTimer(Dash.time));
            }
        }

        private IEnumerator DashCooldown(float time)
        {
            CanDash = false;
            yield return new WaitForSeconds(time);
            CanDash = true;
        }

        private IEnumerator ApplyForwardDamageOn(float time, float damage)
        {
            yield return new WaitForSeconds(time);
            onSlashApply?.Invoke();
            var targets = CombatCollider.GetTargets(transform.position + movement.FacingDirection + ForwardCollider.localForwardPosition, ForwardCollider.radius, ForwardCollider.layerMask);

            foreach (var target in targets)
                target.ReceiveDamage(damage);
        }

        private IEnumerator ApplyCenterDamageOn(float time, float damage)
        {
            yield return new WaitForSeconds(time);
            onSlamApply?.Invoke();
            var targets = CombatCollider.GetTargets(transform.position, SlamCollider.radius, SlamCollider.layerMask);

            foreach (var target in targets)
                target.ReceiveDamage(damage);
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

        private void ApplyDashDamage()
        {
            if (!IsDashing) return;
            
            var targets = CombatCollider.GetTargets(transform.position + movement.MovementDirection, SlamCollider.radius, SlamCollider.layerMask);

            foreach (var target in targets)
                target.ReceiveDamage(Dash.damage);
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
            
            ForceReceiveDamage(value);
        }

        public void ForceReceiveDamage(float value)
        {
            if (IsDead) return;
            
            if (applyForwardDamage != null) StopCoroutine(applyForwardDamage);
            if (movement.fallToStand != null) movement.StopCoroutine(movement.fallToStand);
            if (vulnerableInBetween != null) StopCoroutine(vulnerableInBetween);
            
            onHit?.Invoke();
            onHitFloat?.Invoke(value);
            
            if (hitCoroutine != null) StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitIsOverTimer());
        }

        public void DisableMovement(float time)
        {
            if (!Vulnerable) return;

            ForceDisableMovement(time);
        }

        public void ForceDisableMovement(float time)
        {
            if (disableMovementCoroutine != null) StopCoroutine(disableMovementCoroutine);
            disableMovementCoroutine = StartCoroutine(DisableMovementCoroutine(time));
        }

        public IEnumerator DisableMovementCoroutine(float time)
        {
            CrowdControl = true;
            yield return new WaitForSeconds(time);
            CrowdControl = false;
        }

        private void PlayerIsDead()
        {
            IsDead = true;
        }
    }
}