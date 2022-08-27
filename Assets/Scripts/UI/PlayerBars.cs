using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerBars : MonoBehaviour
    {
        [SerializeField] private PlayerStatus playerStatus;
        [SerializeField] private Image health, energy, stamina;
        

        private void Update()
        {
            UpdateBars();
        }

        private void UpdateBars()
        {
            health.fillAmount = playerStatus.Health / playerStatus.MaxHealth;
            energy.fillAmount = playerStatus.Energy / playerStatus.MaxEnergy;
            stamina.fillAmount = playerStatus.Stamina / playerStatus.MaxStamina;
        }
    }
}