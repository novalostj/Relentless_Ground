using UnityEngine;

namespace _2._5D_Objects
{
    public class SpriteRotation : MonoBehaviour
    {
        public bool isCameraTarget = true;
        public Transform target;

        public float Front { get; private set; }
        public float Side { get; private set; }

        private void Start()
        {
            SetCamera();
        }

        private void Update()
        {
            if (!target)
            {
                SetCamera();
                return;
            }
            
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 other = (target.position - transform.position).normalized;

            Front = Vector3.Dot(forward, other);
            Side = Vector3.Dot(right, other);
        }

        private void SetCamera()
        {
            if (isCameraTarget)
            {
                var main = Camera.main;
                if (!main) return;
                
                target = main.transform;
            }
        }
    }
}