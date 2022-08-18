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

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ToggleCursor();
            }
        }

        private void ToggleCursor()
        {
            mouseIsHidden = !mouseIsHidden;

            Cursor.lockState = mouseIsHidden ? CursorLockMode.Locked : CursorLockMode.None;
        }

        
        private void SetCursor(bool value)
        {
            mouseIsHidden = value;

            Cursor.lockState = mouseIsHidden ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}