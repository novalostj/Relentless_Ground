using System.Collections;
using Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.Control
{
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

        public Targets forwardAttack, centerAttack;

        [SerializeField] private float onAttackTime = 0.7f;
        [SerializeField] private float hitTime = 0.4f;

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

        private Movement movement;
        private float timer, dashTimer;
        private Coroutine applyForwardDamage, vulnerableInBetween;

        public bool IsAttacking => timer > 0;
        public bool IsDashing => dashTimer > 0;
        public bool CanDash { get; private set; } = true;
        public bool CanSlam { get; private set; } = true;
        public bool CanSlash { get; private set; } = true;
        public bool Vulnerable { get; private set; } = true;
        public bool IsHit { get; private set; }
        private bool CanAttack => timer <= 0;
        
        private bool CanAttackV1 => movement.IsGrounded && movement.CanMove && CanSlash;
        private bool CanAttackV2 => !movement.IsGrounded && movement.CanMove && CanSlam;
        private bool CanAttackV3 => movement.IsMoving && movement.IsHoldingRun && !movement.IsDashing && CanDash;
        
        private void Start()
        {
            movement = GetComponent<Movement>();
        }

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
            if (!CanAttack || IsHit) return;
            
            if (CanAttackV1)
            {
                bool? hasEnergy = energyConsumption?.Invoke(slashCost);
                
                if (hasEnergy is false) return; 
                
                timer = onAttackTime;
                playerCombatV1?.Invoke();
                applyForwardDamage = StartCoroutine(ApplyForwardDamageOn(applySlashDamageOn));
                StartCoroutine(SlashTimer(slashCooldown));
                vulnerableInBetween = StartCoroutine(InvulnerableInBetween(applySlashDamageOn, onAttackTime));
            }
            else if (CanAttackV2)
            {
                bool? hasEnergy = energyConsumption?.Invoke(slamCost);
                
                if (hasEnergy is false) return; 
                
                timer = onAttackTime;
                movement.ApplyMotion(new Vector3(0, slamWeight, 0));
                playerCombatV2?.Invoke();
                
                StartCoroutine(ApplyCenterDamageOn(applySlamDamageOn));
                StartCoroutine(SlamTimer(slamCooldown));
                vulnerableInBetween = StartCoroutine(InvulnerableInBetween(applySlamDamageOn, onAttackTime));
            }
        }

        private IEnumerator InvulnerableInBetween(float start, float after)
        {
            if (start > after)
            {
                Debug.Log($"Start Is Bigger Than After {this}");
                yield break;
            }

            yield return new WaitForSeconds(start);
            Vulnerable = false;
            yield return new WaitForSeconds(start - after);
            Vulnerable = true;
        }

        private void SecondAttack()
        {
            if (!CanAttack || IsHit) return;
            
            if (CanAttackV3)
            {
                bool? hasEnergy = energyConsumption?.Invoke(dashCost);
                
                if (hasEnergy is false) return; 
                
                timer = onDashTime;
                dashTimer = onDashTime;
                playerCombatV3?.Invoke();
                playerCombatV3Float?.Invoke(onDashTime);
                StartCoroutine(DashCooldown(dashCooldown));
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
            forwardAttack.ApplyDamage(1);
        }

        private IEnumerator ApplyCenterDamageOn(float time)
        {
            Vulnerable = false;
            yield return new WaitForSeconds(time);
            Vulnerable = true;
            centerAttack.ApplyDamage(1);
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
            if (IsDashing) Vulnerable = false;
            else
            {
                Vulnerable = true;
                return;
            }
            
            forwardAttack.ApplyDamage(1);
        }

        private IEnumerator HitIsOverTimer()
        {
            Vulnerable = false;
            
            IsHit = true;
            
            yield return new WaitForSeconds(hitTime);

            IsHit = false;
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
            StartCoroutine(HitIsOverTimer());
        }
    }
}