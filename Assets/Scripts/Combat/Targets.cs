using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public interface ITargetable
    {
        public void ReceiveDamage(float value);
        public void ForceReceiveDamage(float value);
        public void DisableMovement(float time);
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