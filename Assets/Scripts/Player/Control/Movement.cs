using System.Collections;
using Stats;
using UnityEngine;

namespace Player.Control
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerCombat))]
    public class Movement : MonoBehaviour
    {
        public delegate void OnAction();
        public delegate bool OnAction<in T>(T value);
        public delegate void OnAction<in T, in TF>(T value, TF value2);
        
        public static OnAction onJump;
        public static OnAction<bool, float> onRun;
        public static OnAction<float> onJumpFloat;
        public static OnAction onLand;
        public static OnAction onStand;


        [Header("Spec")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private float runMultiplier = 2f;
        [SerializeField] private float dashMultiplier = 2.5f;
        [SerializeField] private float runCostPerSeconds = 1f;
        
        [Header("Jump")]
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpStrength = 0.08f;
        [SerializeField] private float onLandWait = 0.6f;
        [SerializeField] private bool canDoubleJump;
        [SerializeField] private float jumpStaminaCost = 20f;
        
        [Header("Setting")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform cameraAnchor;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        
        private CharacterController controller;
        private PlayerCombat playerCombat;
        public Coroutine fallToStand;
        private float timer, dashTimer;
        private bool isDashing, runToggleForEvent, doubleJumpToggle;
        private Vector3 lastMoveDir;
        private Vector2 inputDir;
        
        public Vector3 InputMoveDirection { get; private set; }
        public Vector3 MovementDirection { get; private set; }
        public Vector3 FacingDirection { get; private set; }
        public bool IsGrounded { get; private set; }
        public Vector3 Velocity { get; private set; }
        public bool IsDashing => dashTimer > 0;
        public bool IsHoldingRun { get; private set; }
        public bool CanMove => timer <= 0 && ((!playerCombat.IsAttacking && !playerCombat.CrowdControl) || isDashing);
        public bool IsMoving => InputMoveDirection.magnitude > 0.2f;
        public bool IsFalling => Velocity.y < -0.02f;
        
        public float VelocityX
        {
            get => Velocity.x;
            set => Velocity = new Vector3(value, Velocity.y, Velocity.z);
        }
        
        public float VelocityY
        {
            get => Velocity.y;
            set => Velocity = new Vector3(Velocity.x, value, Velocity.z);
        }
        
        public float VelocityZ
        {
            get => Velocity.z;
            set => Velocity = new Vector3(Velocity.x, Velocity.y, value);
        }

        private void OnEnable()
        {
            InputDetection.onMovement += GetMovementInputs;
            PlayerStatus.noStamina += StopRun;
            InputDetection.onJump += Jump;
            InputDetection.onRun += Run;
            PlayerCombat.playerCombatV3Float += Dash;
        }

        private void OnDisable()
        {
            InputDetection.onMovement -= GetMovementInputs;
            PlayerStatus.noStamina -= StopRun;
            InputDetection.onJump -= Jump;
            InputDetection.onRun -= Run;
            PlayerCombat.playerCombatV3Float -= Dash;
        }

        private void Start()
        {
            timer = 0f;
            playerCombat = GetComponent<PlayerCombat>();
            controller = GetComponent<CharacterController>();
            runToggleForEvent = IsHoldingRun;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        private void Update()
        {
            if (runToggleForEvent != IsHoldingRun)
            {
                runToggleForEvent = IsHoldingRun;
                onRun?.Invoke(runToggleForEvent, -runCostPerSeconds);
            }
            
            SetupTimer(Time.deltaTime);

            InputMoveDirection =
                (cameraAnchor ? cameraAnchor.right : transform.right) * inputDir.x +
                (cameraAnchor ? cameraAnchor.forward : transform.forward) * inputDir.y;

            bool checkFloor = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            
            CheckFloor(checkFloor);
            
            if (IsGrounded) doubleJumpToggle = true;
            
            IsGrounded = checkFloor;
        }

        private void FixedUpdate()
        {
            VelocityY = isDashing ? 0 : Velocity.y + gravity * Time.fixedDeltaTime;

            MovementDirection = 
                isDashing ? lastMoveDir : 
                CanMove ? InputMoveDirection.normalized : Vector3.zero;

            float newSpeed = 
                isDashing ? speed * dashMultiplier : 
                IsHoldingRun ? speed * runMultiplier : speed;

            if (IsMoving && !playerCombat.IsAttacking) FacingDirection = MovementDirection.normalized;
            
            Move(Velocity + MovementDirection * (newSpeed * Time.fixedDeltaTime));
        }

        private void SetupTimer(float deltaTime)
        {
            timer = Mathf.Clamp(timer - deltaTime, 0, 20);
        }

        private void Move(Vector3 motion) => controller.Move(motion);
        public void ApplyMotion(Vector3 motion) => Velocity += motion;

        private void CheckFloor(bool checkFloor)
        {
            if (!checkFloor) return;

            if (Velocity.y < 0)
                VelocityY = -0.01f;
            
            if (!IsGrounded && !playerCombat.IsAttacking)
            {
                onLand?.Invoke();
                fallToStand = StartCoroutine(LandToStand());
                timer = onLandWait;
            }
        }

        private void GetMovementInputs(Vector2 input) => inputDir = input;

        private void Jump()
        {
            if (!CanMove || isDashing) return;

            if (!IsGrounded)
            {
                if (!canDoubleJump || !doubleJumpToggle) return;
                
                doubleJumpToggle = false;
            }
            
            bool? hasEnoughStamina = onJumpFloat?.Invoke(jumpStaminaCost);

            if (hasEnoughStamina == false) return;
            
            VelocityY = Mathf.Sqrt(jumpStrength * -2f * gravity);
            onJump?.Invoke();
        }

        private void Run(bool value)
        {
            if (!CanMove || !IsGrounded)
            {
                if (!value) IsHoldingRun = false;
                return;
            }
            IsHoldingRun = value;
        }

        private void Dash(float time)
        {
            lastMoveDir = (cameraAnchor ? cameraAnchor.right : transform.right) * inputDir.x +
                          (cameraAnchor ? cameraAnchor.forward : transform.forward) * inputDir.y;
            
            StartCoroutine(IEDashTime(time));
        }

        private IEnumerator IEDashTime(float time)
        {
            isDashing = true;
            yield return new WaitForSeconds(time);
            isDashing = false;
        }

        private IEnumerator LandToStand()
        {
            yield return new WaitForSeconds(onLandWait);
            onStand?.Invoke();
        }

        private void StopRun() => IsHoldingRun = false;
    }
}