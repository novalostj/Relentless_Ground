using UnityEngine;
using UnityEngine.Events;

namespace Stats
{
    [CreateAssetMenu(fileName = "Enemy_Scriptable", menuName = "Enemy Scriptable")]
    public class EnemyStatus : BaseStatus
    {
        private UnityEvent death;

        public override void AddHealth(float value)
        {
            base.AddHealth(value);
            
            if (Health == 0)
                death?.Invoke();
        }

        public EnemyStatus(EnemyStatus status)
        {
            maxHealth = status.maxHealth;
            OnStart();
        }
    }
}