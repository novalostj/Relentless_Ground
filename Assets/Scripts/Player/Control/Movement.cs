using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Control
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        public delegate void OnAction();
        public static OnAction onJump;
        public static OnAction onLand;
        
        [Header("Spec")]
        public float speed = 8f;
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
        private CharacterController _controller;
        private Vector2 inputDir;
        private float _timer;
        
        public bool CanMove => _timer <= 0;
        public bool IsMoving => moveDir.magnitude > 0.1f;
        public bool IsFalling => velocity.y < -0.02f;

        private void OnEnable()
        {
            InputDetection.onMovement += GetMovementInputs;
            InputDetection.onJump += Jump;
        }

        private void OnDisable()
        {
            InputDetection.onMovement -= GetMovementInputs;
            InputDetection.onJump += Jump;
        }

        private void Start()
        {
            _timer = 0f;
            _controller = GetComponent<CharacterController>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        private void Update()
        {
            _timer = Mathf.Clamp(_timer - Time.deltaTime, 0, 20);

            moveDir =
                (cameraAnchor ? cameraAnchor.right : transform.right) * inputDir.x +
                (cameraAnchor ? cameraAnchor.forward : transform.forward) * inputDir.y;

            bool checkFloor = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            
            CheckFloor(checkFloor);
            
            isGrounded = checkFloor;
        }

        private void FixedUpdate()
        {
            velocity.y += gravity * Time.fixedDeltaTime;

            Vector3 movementDirection = CanMove ? moveDir.normalized : new();
            
            _controller.Move(velocity + movementDirection * (speed * Time.fixedDeltaTime));
        }

        private void CheckFloor(bool checkFloor)
        {
            if (!checkFloor) return;
                
            if (velocity.y < 0)
                velocity.y = -0.01f;
            
            if (!isGrounded)
            {
                onLand?.Invoke();
                _timer = onLandWait;
            }
        }

        private void GetMovementInputs(Vector2 input) => inputDir = input;

        private void Jump()
        {
            if (!CanMove || !isGrounded) return;
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            onJump?.Invoke();
        }
    }
}