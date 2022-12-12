using UnityEngine;
using UnityEngine.AI;

namespace Simpson.AI
{
    public class WanderRandomly : AiBehaviour
    {
        [SerializeField] private float min = 2f; 
        [SerializeField] private float max = 10f;
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AiBrain brain;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            brain = GetComponent<AiBrain>();
        }


        public override bool CanStart()
        {
            return true;
        }

        public override bool CanStop()
        {
            return false;
        }

        protected override void Enter()
        {
            agent.stoppingDistance = 0.5f;
        }

        protected override void Exit()
        {
        }

        public override void DoUpdate()
        {
            if (!agent.hasPath || agent.isStopped || agent.remainingDistance < 0.1f)
            {
                var distance = Random.Range(min, max);
                var dir = Random.insideUnitCircle;
                agent.SetDestination(transform.position + new Vector3(dir.x, 0, dir.y) * distance);
            }
            brain.patrolOrigin = transform.position;
        }
    }
}