using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.AI
{
    public class BaseAI : MonoBehaviour
    {
        [SerializeField] private float goInterval = 0.2f;
        [SerializeField] private float minimumDistance = 2f;

        private Transform Target => eyeSight.Target;
        private BaseCombat baseCombat;
        private NavMeshAgent agent;
        private EyeSight eyeSight;

        private void OnEnable()
        {
            if (baseCombat) baseCombat.hit.AddListener(FacePlayer);
        }

        private void OnDisable()
        {
            if (baseCombat) baseCombat.hit.RemoveListener(FacePlayer);
        }

        protected virtual void Start()
        {
            baseCombat = GetComponent<BaseCombat>();
            baseCombat.hit.AddListener(FacePlayer);
            
            eyeSight = GetComponentInChildren<EyeSight>();
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = minimumDistance;

            StartCoroutine(FollowTargetInterval());
        }

        protected virtual void GoToPath(Vector3 point)
        {
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
                
                if (eyeSight.isAgro && !agent.isStopped) GoToPath(Target.position);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        private void FacePlayer()
        {
            transform.LookAt(Target);
        }

        public void SetAgentState(bool value) => agent.isStopped = value;
        
    }
}
