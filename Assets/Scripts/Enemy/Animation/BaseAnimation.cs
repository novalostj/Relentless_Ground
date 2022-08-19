using _2._5D_Objects;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Animation
{
    public class BaseAnimation : MonoBehaviour
    {
        public SpriteRotation spriteRotation;
        public Animator animator;
        public NavMeshAgent agent;
        
        private static readonly int Front = Animator.StringToHash("Front");
        private static readonly int Side = Animator.StringToHash("Side");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Update()
        {
            SetDirections();
            
            animator.SetFloat(Speed, agent.velocity.magnitude);
        }
        
        private void SetDirections()
        {
            animator.SetFloat(Front, spriteRotation.Front);
            animator.SetFloat(Side, spriteRotation.Side);
        }
        
        
    }
}
