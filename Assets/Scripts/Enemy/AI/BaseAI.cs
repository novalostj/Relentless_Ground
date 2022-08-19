using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Enemy.AI
{
    public class BaseAI : MonoBehaviour
    {
        public Transform target;
        private NavMeshAgent agent;

        [SerializeField] private float minimumDistance = 2f;

        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = minimumDistance;
        }

        protected virtual void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                GoToPath(target.position);
            }
        }

        protected virtual void GoToPath(Vector3 point)
        {
            var path = new NavMeshPath();
            agent.CalculatePath(point, path);
            
            if (path.status == NavMeshPathStatus.PathInvalid) agent.SetPath(path);
        }
    }
}
