using System.Collections;
using Enemy.AI.Combat;
using Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Enemy.AI
{
    public class EnemyStatusCopy
    {
        public UnityEvent death;
        
        public float maxHealth = 100f;
        
        public float Health { get; protected set; }
        
        public void AddHealth(float value)
        {
            Health = Mathf.Clamp(value + Health, 0, maxHealth);
            if (Health == 0)
                death?.Invoke();
        }
        
        public void OnStart()
        {
            Health = maxHealth;
        }
        
        public EnemyStatusCopy(EnemyStatus status)
        {
            maxHealth = status.maxHealth;
            death = new UnityEvent();
            OnStart();
        }
    }
    
    public class BaseAI : MonoBehaviour
    {
        [SerializeField] private EnemyStatus scriptableStatus;
        [SerializeField] private float goInterval = 0.2f;
        [SerializeField] private float minimumDistance = 2f;
        
        private BaseCombat oldBaseCombat;
        private NavMeshAgent agent;
        private EyeSight eyeSight;
        public EnemyStatusCopy EnemyStatus { get; private set; }

        [HideInInspector]
        public UnityEvent onDeath;
        
        public bool IsDead { get; private set; }

        
        protected virtual void Start()
        {
            EnemyStatus = new(scriptableStatus);
            EnemyStatus.death.AddListener(Dead);
            oldBaseCombat = GetComponent<BaseCombat>();
            
            eyeSight = GetComponentInChildren<EyeSight>();
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = minimumDistance;

            StartCoroutine(FollowTargetInterval());
        }

        protected virtual void GoToPath(Vector3 point)
        {
            if (!eyeSight.Target) return;
            
            var path = new NavMeshPath();
            agent.CalculatePath(point, path);
            
            if (path.status != NavMeshPathStatus.PathInvalid) agent.SetPath(path);
        }

        public virtual IEnumerator Halt(float time)
        {
            agent.isStopped = true;
            
            yield return new WaitForSeconds(time);
            
            agent.isStopped = false;
        }

        protected virtual IEnumerator FollowTargetInterval()
        {
            while (true)
            {
                yield return new WaitForSeconds(goInterval);

                if (eyeSight.isAgro && !agent.isStopped) GoToPath(eyeSight.Target.position);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        public void SetAgentState(bool value) => agent.isStopped = value;

        public void ReceiveDamage(float value)
        {
            EnemyStatus.AddHealth(-value);
        }

        private void Dead()
        {
            IsDead = true;
            agent.isStopped = true;
            StartCoroutine(WaitThenDeath(oldBaseCombat.HitInvulnerableTime));
        }

        private IEnumerator WaitThenDeath(float time)
        {
            yield return new WaitForSeconds(time);
            onDeath?.Invoke();
            StopAllCoroutines();
            Destroy(gameObject, 5);
        }
    }
}
