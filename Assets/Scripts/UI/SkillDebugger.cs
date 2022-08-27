using System;
using Player.Control;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillDebugger : MonoBehaviour
    {
        public Text text;

        public PlayerCombat playerCombat;

        private void Update()
        {
            text.text = $"slash - {playerCombat.CanSlash}\n" +
                        $"slam - {playerCombat.CanSlam}\n" +
                        $"dash - {playerCombat.CanDash}";
        }
    }
}