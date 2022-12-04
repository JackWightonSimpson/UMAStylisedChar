using GameSystem.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Simpson.Character
{
    public class FollowWaypoints : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform[] points;
        private int index;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }


        private void FixedUpdate()
        {
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
    }
}