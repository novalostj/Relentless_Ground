using System;
using UnityEngine;

namespace Stats
{
    [Serializable]
    public class Movement
    {
        [Header("Spec")]
        public float speed = 8f;
        public float runMultiplier = 2f;
        public float dashMultiplier = 2.5f;
        public float runCostPerSeconds = 1f;
        
        [Header("Jump")]
        public float gravity = -9.81f;
        public float jumpStrength = 0.08f;
        public float onLandWait = 0.6f;
        public bool canDoubleJump;
        public float jumpStaminaCost = 20f;
    }
    
    [Serializable]
    public class Attack
    {
        public float cooldown = 1;
        public float applyDamageOn = 1;
        public float energyCost = 20f;
        public float damage = 20f;
    }

    [Serializable]
    public class Dash : Attack
    {
        public float time = 1;
    }

    [Serializable]
    public class Slam : Attack
    {
        public float voMultiplier = 1f;
    }
    
    [CreateAssetMenu(fileName = "Player_Scriptable", menuName = "Player Scriptable")]
    public class PlayerStatus : BaseStatus
    {
        public delegate void PlayerEvent();
        
        public static PlayerEvent updateStatus;
        public static PlayerEvent noEnergy;
        public static PlayerEvent noHealth;
        public static PlayerEvent noStamina;

        [SerializeField] private Movement movement;
        [SerializeField] private Attack slash;
        [SerializeField] private Slam slam;
        [SerializeField] private Dash dash;
        [SerializeField] private SphereColliders forwardCollider, slamCollider;
        [SerializeField] private Regeneration healthRegeneration, staminaRegeneration, energyRegeneration;
        
        private float energy = 0;
        private float stamina = 0;
        
        public float MaxEnergy => 100f;
        public float MaxStamina => 25f;
        
        public float Energy => energy;
        public float Stamina => stamina;

        public Attack Slash => slash;
        public Slam Slam => slam;
        public Dash Dash => dash;
        public Regeneration HealthRegeneration => healthRegeneration;
        public Regeneration StaminaRegeneration => staminaRegeneration;
        public Regeneration EnergyRegeneration => energyRegeneration;
        public SphereColliders ForwardCollider => forwardCollider;
        public SphereColliders SlamCollider => slamCollider;
        public Movement Movement => movement;


        public bool IsDead { get; private set; }

        public virtual void AddEnergy(float value)
        {
            energy = Mathf.Clamp(value + energy, 0, MaxEnergy);
            
            if (energy == 0)
                noEnergy?.Invoke();
        }
        
        public virtual void AddStamina(float value)
        {
            stamina = Mathf.Clamp(value + stamina, 0, MaxStamina);
            
            if (stamina == 0)
                noStamina?.Invoke();
        }

        public override void AddHealth(float value)
        {
            base.AddHealth(value);
            
            updateStatus?.Invoke();
            
            if (Health == 0)
            {
                noHealth?.Invoke();
                IsDead = true;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            energy = MaxEnergy;
            stamina = MaxStamina;
        }

        public bool OnEnergyConsumption(float value)
        {
            float localEnergy = energy - value;

            if (localEnergy < 0)
                return false;

            energy = localEnergy;
            updateStatus?.Invoke();
            return true;
        }

        public bool OnStaminaConsumption(float value)
        {
            float localStamina = stamina - value;

            if (localStamina < 0)
                return false;

            stamina = localStamina;
            updateStatus?.Invoke();
            return true;
        }
    }
}