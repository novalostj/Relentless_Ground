using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combat
{
    public static class CombatCollider
    {
        public static List<ITargetable> GetTargets(Vector3 position, float radius, LayerMask layerMask)
        {
            var colliders = GetColliders(position, radius, layerMask);

            return (from collider in colliders select collider.GetComponent<ITargetable>()).ToList();
        }
    
        public static Collider[] GetColliders(Vector3 position, float radius, LayerMask layerMask) =>
            Physics.OverlapSphere(position, radius, layerMask);
        
        public static void AttackSphere(Vector3 position, float radius, float damage, LayerMask targetLayer)
        {
            var foundTargets = GetTargets(position, radius, targetLayer);

            foreach (var iTarget in foundTargets)
                iTarget.ReceiveDamage(damage);
        }
    }
    
    public interface ITargetable
    {
        public void ReceiveDamage(float value);
        public void ForceReceiveDamage(float value);
        public void DisableMovement(float time);
        public void ForceDisableMovement(float time);
        public IEnumerator DisableMovementCoroutine(float time);
    }

    public class Targets : MonoBehaviour
    {
        [SerializeField] private List<string> targetTags;
        private List<ITargetable> targetsList = new List<ITargetable>();


        private void OnTriggerEnter(Collider other)
        {
            if (!IsTargetInList(other.tag)) return;
            
            ITargetable target = other.GetComponent<ITargetable>(); 
            targetsList.Add(target);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsTargetInList(other.tag)) return;

            ITargetable target = other.GetComponent<ITargetable>();
            targetsList.Remove(target);
        }

        private bool IsTargetInList(string targetTag)
        {
            foreach (var sTargetTag in targetTags) if (targetTag == sTargetTag) return true;
            return false;
        }

        public void ApplyDamage(float value)
        {
            if (targetsList.Count == 0) return;
            
            foreach (var target in targetsList)
                target.ReceiveDamage(value);
        }
    }
}