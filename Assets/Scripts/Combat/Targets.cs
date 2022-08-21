using System;
using System.Collections;
using System.Collections.Generic;
using Player.Control;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public interface ITargetable
    {
        public void ReceiveDamage();
    }

    public class Targets : MonoBehaviour
    {
        [SerializeField] private List<string> targetTags;
        public UnityEvent onAttack;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsInTargetList(other.tag)) return;
            
            ITargetable target = other.GetComponent<ITargetable>(); 
            onAttack.AddListener(target.ReceiveDamage);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsInTargetList(other.tag)) return;

            ITargetable target = other.GetComponent<ITargetable>();
            onAttack.RemoveListener(target.ReceiveDamage);
        }

        private bool IsInTargetList(string targetTag)
        {
            foreach (var sTargetTag in targetTags) if (targetTag == sTargetTag) return true;
            return false;
        }
    }
}