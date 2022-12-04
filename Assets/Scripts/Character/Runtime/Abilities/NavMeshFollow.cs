using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class NavMeshFollow : CharacterAbility
    {

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float speedAdjust = 3f;

        public override void Init()
        {
            base.Init();
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.autoTraverseOffMeshLink = false;
        }

        public override void OnStart()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.autoTraverseOffMeshLink = false;
        }

        public override void OnStop()
        {
        }

        public override bool CanStart()
        {
            return agent.isActiveAndEnabled;
        }

        public override bool CanStop()
        {
            return !agent.isActiveAndEnabled;
        }

        public override void UpdateCharacter()
        {
            var target = agent.steeringTarget;
            if (agent.desiredVelocity.magnitude < 0.05f)
            {
                CharacterStateManager.Animator.SetFloat("Forward", 0f);
            } 
            else
            {
                CharacterStateManager.Animator.SetFloat("Forward", agent.desiredVelocity.magnitude/speedAdjust);
                CharacterStateManager.NextOrientation = Quaternion.LookRotation(agent.desiredVelocity);
                if (agent.desiredVelocity.magnitude > 1.6 && ! CharacterStateManager.Animator.GetBool("Run"))
                {
                    SendMessage("OnSprint", new InputValue());
                }
                else if(agent.desiredVelocity.magnitude < 1.5 && CharacterStateManager.Animator.GetBool("Run"))
                {
                    
                    SendMessage("OnSprint", new InputValue());
                }
            }


            if (agent.isOnOffMeshLink )
            {
                if (CharacterStateManager.Grounded)
                {
                    SendMessage("OnJump", new InputValue());
                }
                CharacterStateManager.Animator.SetFloat("Forward", 1f);
                CharacterStateManager.NextOrientation = Quaternion.LookRotation(agent.currentOffMeshLinkData.endPos-agent.currentOffMeshLinkData.startPos);
                if (Vector3.Distance(transform.position, agent.currentOffMeshLinkData.startPos) >
                    Vector3.Distance(agent.currentOffMeshLinkData.endPos, agent.currentOffMeshLinkData.startPos))
                {
                    agent.CompleteOffMeshLink();
                }
            }
            // if(agent.lin)
        }

        public override void Cleanup()
        {
            // agent.velocity = CharacterStateManager.Controller.velocity;
            agent.nextPosition = transform.position;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.autoTraverseOffMeshLink = false;
            // agent.
        }
        
        #region debug

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }
#endif
            // if (agent.path.status)
            // {
                Gizmos.color = Color.yellow;
                var p = transform.position;
                Gizmos.DrawSphere(p, 0.1f);
                foreach (var c in agent.path.corners) 
                {
                    Gizmos.DrawLine(p, c);
                    p = c;
                    Gizmos.DrawSphere(p, 0.1f);
                }
                
            // }
            Gizmos.color = agent.isOnOffMeshLink ? Color.cyan : Color.magenta;
            Gizmos.DrawRay(transform.position+Vector3.up*0.1f, agent.desiredVelocity);
        }
        #endregion
    }
}