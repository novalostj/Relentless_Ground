using System;
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
        
        [SerializeField] private InputAction movement;
        [SerializeField] private InputAction jump;
        [SerializeField] private InputAction attack;


        private void OnEnable()
        {
            movement.Enable();
            jump.Enable();
            attack.Enable();
        }

        private void OnDisable()
        {
            movement.Disable();
            jump.Disable();
            attack.Enable();
        }

        private void Update()
        {
            Vector2 movementDir = movement.ReadValue<Vector2>();
            
            onMovement?.Invoke(movementDir);

            if (jump.IsPressed())
                onJump?.Invoke();
            
            if (attack.IsPressed())
                onAttack?.Invoke();
        }
    }
}