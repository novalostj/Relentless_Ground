using Combat;
using UnityEngine;

namespace Projectile
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private string targetTag, targetPositionTag;
        
        public float damage = 10f;
        public float speed = 10f;

        private void FixedUpdate()
        {
            transform.Translate(Vector3.forward * (speed * Time.fixedDeltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(targetTag) && !other.CompareTag("Ground")) return;
            
            ITargetable iTarget = other.GetComponent<ITargetable>();
            
            iTarget?.ReceiveDamage(damage);
            Destroy(gameObject);
        }
    }
}
