using UnityEngine;

namespace Stats
{
    public class BaseStatus : ScriptableObject
    {
        public float maxHealth = 100f;
        
        public float Health { get; protected set; }


        public virtual void AddHealth(float value)
        {
            Health = Mathf.Clamp(value + Health, 0, maxHealth);
        }

        public virtual void OnStart()
        {
            Health = maxHealth;
        }
    }
}