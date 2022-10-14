using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Control
{
    public class MouseControl : MonoBehaviour
    {
        private bool mouseIsHidden;
        
        private void Start()
        {
            SetCursor(true);
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ToggleCursor();
            }
        }

        private void ToggleCursor() => SetCursor(!mouseIsHidden);


        private void SetCursor(bool value)
        {
            mouseIsHidden = value;

            Cursor.lockState = mouseIsHidden ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}