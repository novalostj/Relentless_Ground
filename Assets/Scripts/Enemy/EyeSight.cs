using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EyeSight : MonoBehaviour
    {
        public Transform target;
        public float sightDistance = 5f;
        public LayerMask collisionMasks;
        public float agroTime = 2f;
        public bool agroed = false;
        
        [Range(-1,1)]
        public float eyeRange = 0.7f;

        public bool HasSight { get; private set; }

        public Vector3 TargetLastPosition => target.position;

        private bool agroChecker;

        void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 other = (target.position - transform.position).normalized;
            
            float sight = Vector3.Dot(forward, other);

            HasSight = sight >= eyeRange && InSight();
            if (HasSight)
            {
                agroChecker = agroed = true;
            }
            else if (agroChecker)
            {
                agroChecker = false;
                StopCoroutine(AgroCoroutine());
                StartCoroutine(AgroCoroutine());
            }
        }

        private bool InSight()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            
            Physics.Raycast(transform.position, direction, out RaycastHit hit, sightDistance, collisionMasks);

            Color color = hit.collider && hit.collider.CompareTag("Player") ? Color.green : Color.red;
            
            Debug.DrawRay(transform.position, direction * sightDistance, color);
            
            return hit.collider && hit.collider.CompareTag("Player");
        }

        private IEnumerator AgroCoroutine()
        {
            yield return new WaitForSeconds(agroTime);
            agroed = false;
        }
    }
}