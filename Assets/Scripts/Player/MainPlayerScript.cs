using System;
using Combat;
using Player.Control;
using Stats;
using UnityEngine;
using Movement = Player.Control.Movement;

namespace Player
{
    [Serializable]
    public class EnemiesReference
    {
        [SerializeField] private ReferenceScriptableObject enemyReference;
        [SerializeField] private GameObject toLookAt;

        public void Setup()
        {
            enemyReference.target = toLookAt;
        }
    }
    
    public class MainPlayerScript : MonoBehaviour 
    {
        [SerializeField] private PlayerStatus playerStatus;
        [SerializeField] private EnemiesReference enemiesReference;

        private Movement movement;
        private PlayerCombat playerCombat;

        private Regeneration Health => playerStatus.HealthRegeneration;
        private Regeneration Stamina => playerStatus.StaminaRegeneration;
        private Regeneration Energy => playerStatus.EnergyRegeneration;
        
        
     
        private void OnEnable()
        {
            PlayerCombat.onHitFloat += ReceiveDamage;
            PlayerCombat.energyConsumption += EnergyConsumption;
            Movement.onRun += RunConsumption;
            Movement.onJumpFloat += StaminaConsumption;
            PlayerStatus.noHealth += OnDeath;
        }

        private void OnDisable() 
        { 
            PlayerCombat.onHitFloat -= ReceiveDamage; 
            PlayerCombat.energyConsumption -= EnergyConsumption;
            Movement.onRun -= RunConsumption;
            Movement.onJumpFloat -= StaminaConsumption;
            PlayerStatus.noHealth -= OnDeath;
        }
        
        private void Start()
        {
            playerCombat = GetComponent<PlayerCombat>();
            movement = GetComponent<Movement>();
            enemiesReference.Setup();
            playerStatus.OnStart();
        }
        
        private void ReceiveDamage(float value) 
        { 
            playerStatus.AddHealth(-value);
            Health.Halt();
        }
     
        private bool EnergyConsumption(float value)
        {
            bool hasEnoughEnergy = playerStatus.OnEnergyConsumption(value);
     
            if (hasEnoughEnergy) Energy.Halt();
     
            return hasEnoughEnergy;
        }
     
        private void Update()
        { 
            Regenerate(Time.deltaTime);
        }
     
        private void Regenerate(float deltaTime)
        {
            playerStatus.AddHealth(Health.Regenerate(deltaTime));
            playerStatus.AddEnergy(Energy.Regenerate(deltaTime));
            playerStatus.AddStamina(Stamina.Regenerate(deltaTime));
        }
     
        private void RunConsumption(bool isPaused, float value)
        {
            Stamina.Pause(isPaused, value);
            Stamina.Halt();
        }
        
        private bool StaminaConsumption(float value)
        {
            bool hasEnoughStamina = playerStatus.OnStaminaConsumption(value);
            
            if (hasEnoughStamina) Stamina.Halt();

            return hasEnoughStamina;
        }

        private void OnDeath()
        {
            movement.enabled = false;
            playerCombat.enabled = false;
        }
    }
}