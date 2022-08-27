using Unity.Collections;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "Player_Scriptable", menuName = "Player Scriptable")]
    public class PlayerStatus : BaseStatus
    {
        public delegate void PlayerEvent();
        
        public static PlayerEvent updateStatus;
        public static PlayerEvent noEnergy;
        public static PlayerEvent noHealth;
        public static PlayerEvent noStamina;

        private float energy = 0;
        private float stamina = 0;
        
        public float MaxEnergy => 100f;
        public float MaxStamina => 25f;
        
        public float Energy => energy;
        public float Stamina => stamina;
        
        
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