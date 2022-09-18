using System;
using UnityEngine;

namespace Stats
{
    [Serializable]
    public class Regeneration
    {
        [SerializeField] private float regenerationStrength = 10f;
        [SerializeField] private float timeWhenFullRegen = 2f;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float pauseTime = 0.5f;
        [Range(0, 1)] [SerializeField] private float decreaseMultiplierBy = 1;

        private float multiplier, timer, overTimeConsumption;
        private bool CanRegenerate => timer == 0;
        private bool isPaused;

        public float RegenerationValue
        {
            get => regenerationStrength;
            set => regenerationStrength = value;
        }
        public float TimeWhenFull
        {
            get => timeWhenFullRegen;
            set => timeWhenFullRegen = value;
        }

        public float PauseTime
        {
            get => pauseTime;
            set => pauseTime = value;
        }
        public float DecreaseMultiplier 
        {
            get => decreaseMultiplierBy;
            set => decreaseMultiplierBy = value;
        }



        public float Regenerate(float deltaTime)
        {
            if (isPaused) return overTimeConsumption * deltaTime;

            timer = Mathf.Clamp(timer - deltaTime, 0, pauseTime);

            if (!CanRegenerate)
                return 0;

            multiplier = Mathf.Clamp(multiplier + deltaTime / timeWhenFullRegen, 0, 1);

            return regenerationStrength * deltaTime * curve.Evaluate(multiplier);
        }

        public void Halt()
        {
            timer = pauseTime;
            multiplier = Mathf.Clamp(multiplier - decreaseMultiplierBy, 0, 1);
        }

        public void Pause(bool paused, float overTimeValue)
        {
            overTimeConsumption = overTimeValue;
            isPaused = paused;
        }
    }
    
    [Serializable]
    public class SphereColliders
    {
        public Vector3 localForwardPosition = Vector3.zero;
        public float radius = 1.5f;
        public LayerMask layerMask;
    }
    
    
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