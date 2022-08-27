using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Stats
{
    public class BaseStatus : ScriptableObject
    {
        private float health;

        public float MaxHealth => 100f;
        
        public float Health => health;
        

        public virtual void AddHealth(float value)
        {
            health = Mathf.Clamp(value + health, 0, MaxHealth);
        }

        public virtual void OnStart()
        {
            health = MaxHealth;
        }
    }
}