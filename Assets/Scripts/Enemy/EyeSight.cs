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
        public bool isAgro;
        
        [Range(-1,1)]
        public float eyeRange = 0.7f;

        public bool HasSight { get; private set; }
        public float TargetDistance => Vector3.Distance(transform.position, target.position);

        public Vector3 TargetLastPosition => target.position;
        
        private bool agroChecker;

        public Coroutine AgroCoroutine { get; set; }

        void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 other = (target.position - transform.position).normalized;
            
            float sight = Vector3.Dot(forward, other);

            HasSight = sight >= eyeRange && InSight();
            
            if (HasSight)
            {
                agroChecker = isAgro = true;
            }
            else if (agroChecker)
            {
                agroChecker = false;
                if (AgroCoroutine != null) StopCoroutine(AgroTimer());
                AgroCoroutine = StartCoroutine(AgroTimer());
            }
        }

        private bool InSight()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Physics.Raycast(transform.position, direction, out RaycastHit hit, sightDistance, collisionMasks);

#if UNITY_EDITOR
            Color color = hit.collider && hit.collider.CompareTag("Player") ? Color.green : Color.red;
            Debug.DrawRay(transform.position, direction * sightDistance, color);
#endif
            
            return hit.collider && hit.collider.CompareTag("Player");
        }

        public IEnumerator AgroTimer()
        {
            isAgro = true;
            yield return new WaitForSeconds(agroTime);
            isAgro = false;
        }
    }
}