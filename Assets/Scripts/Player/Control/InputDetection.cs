using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Control
{
    public class InputDetection : MonoBehaviour
    {
        public delegate void InputEvent<T>(T value);
        public delegate void InputEvent();

        public static InputEvent<Vector2> onMovement;
        public static InputEvent<bool> onRun;
        public static InputEvent<int> ability;
        public static InputEvent
            onJump,
            onAttack,
            onSecondAttack;

        [SerializeField] private InputAction
            movement,
            jump,
            run,
            attack,
            secondAttack,
            ability1,
            ability2,
            ability3,
            ability4;

        private Vector2 movementInputVector2;
        
        private void OnEnable()
        {
            movement.Enable();
            jump.Enable();
            attack.Enable();
            run.Enable();
            secondAttack.Enable();
        }

        private void OnDisable()
        {
            movement.Disable();
            jump.Disable();
            attack.Disable();
            run.Disable();
            secondAttack.Disable();
        }

        private void Update()
        {
            movementInputVector2 = movement.ReadValue<Vector2>();
            
            onMovement?.Invoke(movementInputVector2);

            if (jump.WasPressedThisFrame())
                onJump?.Invoke();
            if (attack.WasPressedThisFrame())
                onAttack?.Invoke();
            if (secondAttack.WasPressedThisFrame())
                onSecondAttack?.Invoke();
            if (ability1.WasPressedThisFrame())
                ability?.Invoke(1);
            if (ability2.WasPressedThisFrame())
                ability?.Invoke(2);
            if (ability3.WasPressedThisFrame())
                ability?.Invoke(3);
            if (ability4.WasPressedThisFrame())
                ability?.Invoke(4);
                
            onRun?.Invoke(run.IsPressed());
        }
    }
}