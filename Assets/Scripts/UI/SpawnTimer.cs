using System;
using Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpawnTimer : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private Text text;

        private void Update()
        {
            if (!spawner) return;
            
            text.text = ((float)Math.Round(spawner.LocalTimer * 100f) / 100f).ToString();
        }
    }
}