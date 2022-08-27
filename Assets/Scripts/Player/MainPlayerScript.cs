using System;
using Player.Control;
using Stats;
using UnityEngine;

namespace Player
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


    public class MainPlayerScript : MonoBehaviour 
    {
        [SerializeField] private PlayerStatus playerStatus;
     
        [SerializeField] private Regeneration
            health, stamina, energy;
     
        private void OnEnable()
        {
            PlayerCombat.onHitFloat += ReceiveDamage;
            PlayerCombat.energyConsumption += EnergyConsumption;
            Movement.onRun += RunConsumption;
            Movement.onJumpFloat += StaminaConsumption;
        }

        private void OnDisable() 
        { 
            PlayerCombat.onHitFloat -= ReceiveDamage; 
            PlayerCombat.energyConsumption -= EnergyConsumption;
            Movement.onRun -= RunConsumption;
            Movement.onJumpFloat -= StaminaConsumption;
        }
        private void Start()
        { 
            playerStatus.OnStart();
        }
        
        private void ReceiveDamage(float value) 
        { 
            playerStatus.AddHealth(-value);
            health.Halt();
        }
     
        private bool EnergyConsumption(float value)
        {
            bool hasEnoughEnergy = playerStatus.OnEnergyConsumption(value);
     
            if (hasEnoughEnergy) energy.Halt();
     
            return hasEnoughEnergy;
        }
     
        private void Update()
        { 
            Regenerate(Time.deltaTime);
        }
     
        private void Regenerate(float deltaTime)
        {
            playerStatus.AddHealth(health.Regenerate(deltaTime));
            playerStatus.AddEnergy(energy.Regenerate(deltaTime));
            playerStatus.AddStamina(stamina.Regenerate(deltaTime));
        }
     
        private void RunConsumption(bool isPaused, float value)
        {
            stamina.Pause(isPaused, value);
            stamina.Halt();
        }
        
        private bool StaminaConsumption(float value)
        {
            bool hasEnoughStamina = playerStatus.OnStaminaConsumption(value);
            
            if (hasEnoughStamina) stamina.Halt();

            return hasEnoughStamina;
        }
    }
}