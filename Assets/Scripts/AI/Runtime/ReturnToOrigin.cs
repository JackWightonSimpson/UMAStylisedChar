using UnityEngine;
using UnityEngine.AI;

namespace Simpson.AI
{
    public class ReturnToOrigin : AiBehaviour
    {        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AiBrain brain;
        [SerializeField] private float followDistance = 20;
        [SerializeField] private float endDistance = 2;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            brain = GetComponent<AiBrain>();
        }

        public override bool CanStart()
        {
            return Vector3.Distance(brain.patrolOrigin, transform.position) > followDistance;
        }

        public override bool CanStop()
        {
            return Vector3.Distance(brain.patrolOrigin, transform.position) < endDistance;
        }

        protected override void Enter()
        {
            agent.SetDestination(brain.patrolOrigin);
        }

        protected override void Exit()
        {
        }

        public override void DoUpdate()
        {
        }
    }
}