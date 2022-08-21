using System;
using Player.Control;
using UnityEngine;

namespace _2._5D_Objects
{
    public class ForwardPosition : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Transform cameraAnchor;
        [SerializeField] private PlayerCombat playerCombat;

        private float currentY;
        private Vector2 inputDir;

        private void OnEnable()
        {
            InputDetection.onMovement += GetMovementInputs;
        }

        private void OnDisable()
        {
            InputDetection.onMovement -= GetMovementInputs;
        }

        private void Start()
        {
            currentY = transform.position.y;
        }


        private void GetMovementInputs(Vector2 input) => inputDir = input.normalized;
        
        private void Update()
        {
            if (!playerCombat || playerCombat.IsAttacking || !movement || 
                !movement.IsMoving || inputDir.magnitude < 0.5f) return;

            Vector3 currentForward = 
                -cameraAnchor.forward * inputDir.y + 
                -cameraAnchor.right * inputDir.x;

            currentForward.y = currentY;
            
            transform.localPosition = currentForward;
        }
    }
}