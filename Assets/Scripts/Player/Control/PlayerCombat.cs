using UnityEngine;

namespace Player.Control
{
    public class PlayerCombat : MonoBehaviour
    {
        public delegate void AttackEvent();
        public delegate void AttackEvent<in T>(T value);
        public static AttackEvent attackV1;
        public static AttackEvent attackV2;
        public static AttackEvent attackV3;
        public static AttackEvent<float> attackV3Float;
        public static AttackEvent onFinished;

        [SerializeField] private float attackV2Weight = -0.5f;
        [SerializeField] private float onDashTime = 0.4f;
        [SerializeField] private float onAttackTime = 0.7f;

        private Movement movement;
        private float timer;

        public bool IsAttacking => timer > 0;
        
        private bool CanAttack => timer <= 0;
        private bool CanAttackV1 => movement.isGrounded && movement.CanMove;
        private bool CanAttackV2 => movement.IsFalling && movement.CanMove;
        private bool CanAttackV3 => movement.IsMoving && movement.IsHoldingRun && movement.CanDash;
        
        private void Start()
        {
            movement = GetComponent<Movement>();
        }

        private void OnEnable()
        {
            InputDetection.onAttack += Attack;
        }
        private void OnDisable()
        {
            InputDetection.onAttack -= Attack;
        }

        private void Update()
        {
            SetTimer();
        }

        private void SetTimer()
        {
            if (timer == 0) return;
            
            timer = Mathf.Clamp(timer - Time.deltaTime, 0, 20); 
            
            if (timer == 0) onFinished?.Invoke();
        }

        private void Attack()
        {
            if (!CanAttack) return;
            
            if (CanAttackV3)
            {
                timer = onDashTime;
                attackV3?.Invoke();
                attackV3Float?.Invoke(onDashTime);
            }
            else if (CanAttackV1)
            {
                timer = onAttackTime;
                attackV1?.Invoke();
            }
            else if (CanAttackV2)
            {
                timer = onAttackTime;
                movement.ApplyMotion(new Vector3(0, attackV2Weight, 0));
                attackV2?.Invoke();
            }
        }
    }
}