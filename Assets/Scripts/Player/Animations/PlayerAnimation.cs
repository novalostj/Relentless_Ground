using _2._5D_Objects;
using Player.Control;
using UnityEngine;

namespace Player.Animations
{
    public class PlayerAnimation : MonoBehaviour
    {
        public PlayerCombat playerCombat;
        public Movement movement;
        public Transform orientation, cameraAnchor;
        public Animator animator;
        public SpriteRotation spriteRotation;

        #region Animator Cached Index
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Front = Animator.StringToHash("Front");
        private static readonly int Side = Animator.StringToHash("Side");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Land = Animator.StringToHash("OnLand");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Climb = Animator.StringToHash("OnClimb");
        private static readonly int OnFinishedAttack = Animator.StringToHash("OnFinishedAttack");
        private static readonly int OnAttackV1 = Animator.StringToHash("OnAttack_V1");
        private static readonly int OnAttackV2 = Animator.StringToHash("OnAttack_V2");
        private static readonly int OnAttackV3 = Animator.StringToHash("OnAttack_V3");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int IsHoldingRun = Animator.StringToHash("IsHoldingRun");

        #endregion
        
        private void OnEnable()
        {
            Movement.onJump += OnJump;
            Movement.onLand += OnLand;
            PlayerCombat.attackV1 += OnAttack_V1;
            PlayerCombat.attackV2 += OnAttack_V2;
            PlayerCombat.attackV3 += OnAttack_V3;
            PlayerCombat.onFinished += OnFinishedAttacking;
        }

        private void OnDisable()
        {
            Movement.onJump -= OnJump;
            Movement.onLand -= OnLand;
            PlayerCombat.attackV1 -= OnAttack_V1;
            PlayerCombat.attackV2 -= OnAttack_V2;
            PlayerCombat.attackV3 -= OnAttack_V3;
            PlayerCombat.onFinished -= OnFinishedAttacking;
        }

        private void Update()
        {
            CorrectSpriteForward();
            SetDirections();
            
            float moveMagnitude = movement.moveDir.magnitude;
            animator.SetFloat(Speed, moveMagnitude);
            animator.SetBool(Falling, movement.IsFalling && !playerCombat.IsAttacking);
            animator.SetBool(IsAttacking, playerCombat.IsAttacking);
            animator.SetBool(IsHoldingRun, movement.IsHoldingRun);
        }
    
        private void OnClimb() => animator.SetTrigger(Climb); 
        private void OnJump() => animator.SetTrigger(Jump);
        private void OnLand() => animator.SetTrigger(Land);
        private void OnFinishedAttacking() => animator.SetTrigger(OnFinishedAttack);
        private void OnAttack_V1() => animator.SetTrigger(OnAttackV1);
        private void OnAttack_V2() => animator.SetTrigger(OnAttackV2);
        private void OnAttack_V3() => animator.SetTrigger(OnAttackV3);

        private void SetDirections()
        {
            animator.SetFloat(Front, spriteRotation.Front);
            animator.SetFloat(Side, spriteRotation.Side);
        }

        private void CorrectSpriteForward()
        {
            if (!movement.IsMoving || !movement.CanMove) return;
        
            orientation.LookAt(orientation.position + movement.moveDir);
        }
    }
}