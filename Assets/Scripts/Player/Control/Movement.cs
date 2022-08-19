using System.Collections;
using UnityEngine;

namespace Player.Control
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerCombat))]
    public class Movement : MonoBehaviour
    {
        public delegate void OnAction();
        public static OnAction onJump;
        public static OnAction onLand;
        
        [Header("Spec")]
        public float speed = 8f;
        public float runMultiplier = 2f;
        public float dashMultiplier = 2.5f;
        public float dashCooldown = 3f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        public float onLandWait = 0.6f;
        
        [Header("Setting")]
        public Transform groundCheck;
        public Transform cameraAnchor;
        public float groundDistance = 0.4f;
        public LayerMask groundLayer;
    
        [HideInInspector] public Vector3 moveDir;
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public Vector3 velocity;
        private CharacterController controller;
        private PlayerCombat playerCombat;
        private Coroutine dashCoroutine;
        private Vector2 inputDir;
        private float timer;
        private float dashTimer;
        private bool isDashing;
        private Vector3 lastMoveDir;
        
        public bool CanDash => dashTimer <= 0;
        public bool IsHoldingRun { get; private set; }
        public bool CanMove => timer <= 0 && (!playerCombat.IsAttacking || isDashing);
        public bool IsMoving => moveDir.magnitude > 0.1f;
        public bool IsFalling => velocity.y < -0.02f;

        private void OnEnable()
        {
            InputDetection.onMovement += GetMovementInputs;
            InputDetection.onJump += Jump;
            InputDetection.onRun += Run;
            PlayerCombat.attackV3Float += Dash;
        }

        private void OnDisable()
        {
            InputDetection.onMovement -= GetMovementInputs;
            InputDetection.onJump -= Jump;
            InputDetection.onRun -= Run;
            PlayerCombat.attackV3Float -= Dash;
        }

        private void Start()
        {
            timer = 0f;
            playerCombat = GetComponent<PlayerCombat>();
            controller = GetComponent<CharacterController>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        private void Update()
        {
            SetupTimer(Time.deltaTime);

            moveDir =
                (cameraAnchor ? cameraAnchor.right : transform.right) * inputDir.x +
                (cameraAnchor ? cameraAnchor.forward : transform.forward) * inputDir.y;

            bool checkFloor = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            
            CheckFloor(checkFloor);
            
            isGrounded = checkFloor;
        }

        private void FixedUpdate()
        {
            velocity.y = isDashing ? 0 : velocity.y + gravity * Time.fixedDeltaTime;

            Vector3 movementDirection = 
                isDashing ? lastMoveDir : 
                CanMove ? moveDir.normalized : new();

            float newSpeed = 
                isDashing ? speed * dashMultiplier : 
                IsHoldingRun ? speed * runMultiplier : speed;
            
            Move(velocity + movementDirection * (newSpeed * Time.fixedDeltaTime));
        }

        private void SetupTimer(float deltaTime)
        {
            timer = Mathf.Clamp(timer - deltaTime, 0, 20);
            dashTimer = Mathf.Clamp(dashTimer - deltaTime, 0, dashCooldown);
        }

        private void Move(Vector3 motion) => controller.Move(motion);
        public void ApplyMotion(Vector3 motion) => velocity += motion;

        private void CheckFloor(bool checkFloor)
        {
            if (!checkFloor) return;
                
            if (velocity.y < 0)
                velocity.y = -0.01f;
            
            if (!isGrounded && !playerCombat.IsAttacking)
            {
                onLand?.Invoke();
                timer = onLandWait;
            }
        }

        private void GetMovementInputs(Vector2 input) => inputDir = input;

        private void Jump()
        {
            if (!CanMove || !isGrounded) return;
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            onJump?.Invoke();
        }

        private void Run(bool value)
        {
            if (!CanMove || !isGrounded)
            {
                if (!value) IsHoldingRun = false;
                return;
            }
            
            IsHoldingRun = value;
        }

        private void Dash(float time)
        {
            isDashing = true;
            lastMoveDir = (cameraAnchor ? cameraAnchor.right : transform.right) * inputDir.x +
                          (cameraAnchor ? cameraAnchor.forward : transform.forward) * inputDir.y;;
            dashTimer = dashCooldown;
            dashCoroutine = StartCoroutine(IEDashTime(time));
        }

        private IEnumerator IEDashTime(float time)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                isDashing = false;
                StopCoroutine(dashCoroutine);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
    }
}