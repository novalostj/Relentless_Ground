using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Control
{
    public class InputDetection : MonoBehaviour
    {
        public delegate void InputEvent<T>(T value);
        public delegate void InputEvent();
        
        public static InputEvent<Vector2> onMovement;
        public static InputEvent onJump;
        public static InputEvent onAttack;
        public static InputEvent<bool> onRun;
            
        [SerializeField] private InputAction movement;
        [SerializeField] private InputAction jump;
        [SerializeField] private InputAction attack;
        [SerializeField] private InputAction run;

        private void OnEnable()
        {
            movement.Enable();
            jump.Enable();
            attack.Enable();
            run.Enable();
        }

        private void OnDisable()
        {
            movement.Disable();
            jump.Disable();
            attack.Disable();
            run.Disable();
        }

        private void Update()
        {
            Vector2 movementDir = movement.ReadValue<Vector2>();
            
            onMovement?.Invoke(movementDir);

            if (jump.WasPressedThisFrame())
                onJump?.Invoke();
            
            if (attack.WasPressedThisFrame())
                onAttack?.Invoke();
            
            onRun?.Invoke(run.IsPressed());
        }
    }
}