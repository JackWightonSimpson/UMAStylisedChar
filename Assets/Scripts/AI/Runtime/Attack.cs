using GameSystem.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Simpson.AI
{
    public class Attack : AiBehaviour
    {        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AiBrain brain;
        [SerializeField] private float followDistance = 1;
        [SerializeField] private float ticks = 2;
        [SerializeField] private float timer = 0;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            brain = GetComponent<AiBrain>();
            brain = GetComponent<AiBrain>();
        }

        public override bool CanStart()
        {
            var t = PlayerManager.Instance.activePlayer.playerTransform.position - transform.position;
            Vector3.Angle(transform.forward, t.normalized);
            return t.magnitude < followDistance && Mathf.Abs(Vector3.Angle(transform.forward, t.normalized)) < 15f;
        }

        public override bool CanStop()
        {
            return timer > ticks;//Vector3.Distance(brain.patrolOrigin, transform.position) > followDistance;
        }

        protected override void Enter()
        {
            agent.isStopped = true;
            SendMessage("OnAttack", true);
            timer = 0;
        }

        protected override void Exit()
        {
            agent.isStopped = false;
            SendMessage("OnAttack", false);
        }

        public override void DoUpdate()
        {
            timer += Time.deltaTime;
        }
    }
}