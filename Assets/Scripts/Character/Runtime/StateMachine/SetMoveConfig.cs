using UnityEngine;

namespace Simpson.Character.StateMachine
{
    public class SetMoveConfig: StateMachineBehaviour
    {
        [SerializeField] private MovementConfig movementConfig;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.gameObject.BroadcastMessage("AddMoveConfig", movementConfig);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.gameObject.BroadcastMessage("RemoveMoveConfig", movementConfig);
        }
        
    }
}