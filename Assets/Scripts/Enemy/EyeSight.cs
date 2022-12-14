using System;
using System.Collections;
using Combat;
using UnityEngine;

namespace Enemy
{
    public class EyeSight : MonoBehaviour
    {
        [SerializeField] private bool alwaysTargetPlayer;
        [SerializeField] private ReferenceScriptableObject referenceScriptableObject;
        
        public float sightDistance = 5f;
        public LayerMask collisionMasks;
        public float agroTime = 2f;
        public bool isAgro;
        
        [Range(-1,1)]
        public float eyeRange = 0.7f;

        public bool HasSight { get; private set; }
        public float TargetDistance => Vector3.Distance(transform.position, Target.position);


        public Transform Target { get; private set; }
        private bool agroChecker;
        private float eyeRangeToggle;

        public Coroutine AgroCoroutine { get; set; }

        private void Start()
        {
            eyeRangeToggle = eyeRange;
            
            if (referenceScriptableObject.target) Target = referenceScriptableObject.target.transform;
        }

        void Update()
        {
            if (!Target)
            {
                if (!referenceScriptableObject.target) return;
                
                Target = referenceScriptableObject.target.transform;
                return;
            }
            
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 other = (Target.position - transform.position).normalized;
            
            float sight = Vector3.Dot(forward, other);

            HasSight = sight >= eyeRangeToggle && InSight();
            
            if (HasSight)
            {
                agroChecker = isAgro = true;
            }
            else if (agroChecker || !alwaysTargetPlayer)
            {
                agroChecker = false;
                if (AgroCoroutine != null) StopCoroutine(AgroTimer());
                AgroCoroutine = StartCoroutine(AgroTimer());
            }
            
            if (alwaysTargetPlayer) isAgro = true;
        }

        private bool InSight()
        {
            Vector3 direction = (Target.position - transform.position).normalized;
            Physics.Raycast(transform.position, direction, out RaycastHit hit, sightDistance, collisionMasks);

#if UNITY_EDITOR
            Color color = hit.collider && hit.collider.CompareTag("Player") ? Color.green : Color.red;
            Debug.DrawRay(transform.position, direction * sightDistance, color);
#endif
                
            return hit.collider && hit.collider.CompareTag("Player");
        }

        public IEnumerator AgroTimer()
        {
            eyeRangeToggle = -1f;
            isAgro = true;
            
            yield return new WaitForSeconds(agroTime);
            
            eyeRangeToggle = eyeRange;
            isAgro = false;
        }
    }
}