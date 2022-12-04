using GameSystem.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Simpson.Character
{
    public class FollowPlayer : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }


        private void FixedUpdate()
        {
            agent.SetDestination(PlayerManager.Instance.activePlayer.playerTransform.position+Vector3.back);
        }
    }
}