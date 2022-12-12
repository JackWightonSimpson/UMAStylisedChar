using UnityEngine;
using UnityEngine.AI;

namespace Simpson.AI
{
    public class FollowWaypoints : AiBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AiBrain brain;
        [SerializeField] private Transform[] points;
        private int index;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            brain = GetComponent<AiBrain>();
        }


        public override void DoUpdate()
        {
            brain.patrolOrigin = transform.position;
            if (points.Length < 1)
            {
                return;
            }

            if (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                return;
            }

            index = (index + 1) % points.Length;
            agent.SetDestination(points[index].position);
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
            agent.SetDestination(transform.position);
        }
    }
}