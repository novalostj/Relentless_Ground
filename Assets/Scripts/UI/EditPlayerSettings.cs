using System;
using System.Globalization;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EditPlayerSettings : MonoBehaviour
    {
        [SerializeField] private PlayerStatus playerStatus;

        [Header("Slash")] 
        public InputField slashCd;
        public InputField slashCost;
        public InputField slashDmg;
        
        [Header("Slam")] 
        public InputField slamCd;
        public InputField slamCost;
        public InputField slamDmg;
        public InputField leapMulti;
        
        [Header("Dash")] 
        public InputField dashCd;
        public InputField dashCost;
        public InputField dashDmg;
        public InputField dashTime;
        
        [Header("Health")] 
        public InputField hValue;
        public InputField hFullOn;
        public InputField hPauseTime;
        public InputField hDcrMulti;
        
        [Header("Stamina")] 
        public InputField sValue;
        public InputField sFullOn;
        public InputField sPauseTime;
        public InputField sDcrMulti;
        
        [Header("Energy")] 
        public InputField eValue;
        public InputField eFullOn;
        public InputField ePauseTime;
        public InputField eDcrMulti;
        
        [Header("Spec")]
        public InputField speed;
        public InputField runMulti;
        public InputField dashMulti;
        public InputField runSCost;
        
        [Header("Jump")]
        public InputField gravity;
        public InputField jmpPwr;
        public InputField landWait;
        public InputField jumpSCost;
        

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            
            if (gameObject.activeSelf) return;

            Apply();
        }

        private void Start()
        {
            GetInfos();
        }

        public void Apply()
        {
            playerStatus.Slash.cooldown =  float.Parse(slashCd.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Slash.energyCost = float.Parse(slashCost.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Slash.damage = float.Parse(slashDmg.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.Slam.cooldown =  float.Parse(slamCd.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Slam.energyCost = float.Parse(slamCost.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Slam.damage = float.Parse(slamDmg.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Slam.voMultiplier = float.Parse(leapMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.Dash.cooldown =  float.Parse(dashCd.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Dash.energyCost = float.Parse(dashCost.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Dash.damage = float.Parse(dashDmg.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Dash.time = float.Parse(dashTime.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.HealthRegeneration.RegenerationValue =  float.Parse(hValue.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.HealthRegeneration.TimeWhenFull = float.Parse(hFullOn.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.HealthRegeneration.PauseTime = float.Parse(hPauseTime.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.HealthRegeneration.DecreaseMultiplier = float.Parse(hDcrMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.StaminaRegeneration.RegenerationValue =  float.Parse(sValue.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.StaminaRegeneration.TimeWhenFull = float.Parse(sFullOn.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.StaminaRegeneration.PauseTime = float.Parse(sPauseTime.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.StaminaRegeneration.DecreaseMultiplier = float.Parse(sDcrMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.EnergyRegeneration.RegenerationValue =  float.Parse(eValue.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.EnergyRegeneration.TimeWhenFull = float.Parse(eFullOn.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.EnergyRegeneration.PauseTime = float.Parse(ePauseTime.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.EnergyRegeneration.DecreaseMultiplier = float.Parse(eDcrMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.Movement.gravity =  float.Parse(gravity.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.jumpStrength = float.Parse(jmpPwr.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.onLandWait = float.Parse(landWait.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.jumpStaminaCost = float.Parse(jumpSCost.text, CultureInfo.InvariantCulture.NumberFormat);
            
            playerStatus.Movement.speed =  float.Parse(speed.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.runMultiplier = float.Parse(runMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.dashMultiplier = float.Parse(dashMulti.text, CultureInfo.InvariantCulture.NumberFormat);
            playerStatus.Movement.runCostPerSeconds = float.Parse(runSCost.text, CultureInfo.InvariantCulture.NumberFormat);
        }

        public void GetInfos()
        {
            slashCd.text = playerStatus.Slash.cooldown.ToString();
            slashCost.text = playerStatus.Slash.energyCost.ToString();
            slashDmg.text = playerStatus.Slash.damage.ToString();
            
            slamCd.text = playerStatus.Slam.cooldown.ToString();
            slamCost.text = playerStatus.Slam.energyCost.ToString();
            slamDmg.text = playerStatus.Slam.damage.ToString();
            leapMulti.text = playerStatus.Slam.voMultiplier.ToString();
            
            dashCd.text = playerStatus.Dash.cooldown.ToString();
            dashCost.text = playerStatus.Dash.energyCost.ToString();
            dashDmg.text = playerStatus.Dash.damage.ToString();
            dashTime.text = playerStatus.Dash.time.ToString();

            hValue.text = playerStatus.HealthRegeneration.RegenerationValue.ToString();
            hFullOn.text = playerStatus.HealthRegeneration.TimeWhenFull.ToString();
            hPauseTime.text = playerStatus.HealthRegeneration.PauseTime.ToString();
            hDcrMulti.text = playerStatus.HealthRegeneration.DecreaseMultiplier.ToString();
            
            sValue.text = playerStatus.StaminaRegeneration.RegenerationValue.ToString();
            sFullOn.text = playerStatus.StaminaRegeneration.TimeWhenFull.ToString();
            sPauseTime.text = playerStatus.StaminaRegeneration.PauseTime.ToString();
            sDcrMulti.text = playerStatus.StaminaRegeneration.DecreaseMultiplier.ToString();
            
            eValue.text = playerStatus.EnergyRegeneration.RegenerationValue.ToString();
            eFullOn.text = playerStatus.EnergyRegeneration.TimeWhenFull.ToString();
            ePauseTime.text = playerStatus.EnergyRegeneration.PauseTime.ToString();
            eDcrMulti.text = playerStatus.EnergyRegeneration.DecreaseMultiplier.ToString();

            speed.text = playerStatus.Movement.speed.ToString();
            runMulti.text = playerStatus.Movement.runMultiplier.ToString();
            dashMulti.text = playerStatus.Movement.dashMultiplier.ToString();
            runSCost.text = playerStatus.Movement.runCostPerSeconds.ToString();
            
            gravity.text = playerStatus.Movement.gravity.ToString();
            jmpPwr.text = playerStatus.Movement.jumpStrength.ToString();
            landWait.text = playerStatus.Movement.onLandWait.ToString();
            jumpSCost.text = playerStatus.Movement.jumpStaminaCost.ToString();
        }
        
        
        
    }
}