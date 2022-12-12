using System.Linq;
using GameSystem.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Simpson.AI
{
    public class FollowPlayer : AiBehaviour
    {

        [SerializeField] private int navUpdate = 5;
        [SerializeField] private int ticks = 0;
        
        [SerializeField] private Vector3 start = Vector3.zero;
        [SerializeField] private float followDistance = 20;
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AiBrain brain;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            brain = GetComponent<AiBrain>();
        }


        public override bool CanStart()
        {
            return brain.targets.Any(c => c.transform == PlayerManager.Instance.activePlayer.playerTransform);
        }

        public override bool CanStop()
        {
            return Vector3.Distance(brain.patrolOrigin, transform.position) > followDistance;
        }

        protected override void Enter()
        {
            start = transform.position;
            agent.stoppingDistance = 1.5f;
        }

        protected override void Exit()
        {
            agent.SetDestination(transform.position);
        }

        public override void DoUpdate()
        {
            //+PlayerManager.Instance.activePlayer.playerTransform.forward*-1.5f
            agent.SetDestination(PlayerManager.Instance.activePlayer.playerTransform.position);
        }
    }
}