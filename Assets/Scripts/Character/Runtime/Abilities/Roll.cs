using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Roll : CharacterAbility
    {
        // private InputAction action;

        private bool doRoll;
        
        public override void Init()
        {
            // action = CharacterStateManager.PlayerInput.actions.FindAction("Roll");
            // action.performed += context => doRoll = true;
        }

        public void OnRoll(InputValue value)
        {
            doRoll = true;//value.;
        }
        
        public override void OnStart()
        {
            CharacterStateManager.Animator.SetInteger("move type", 2);
            CharacterStateManager.Animator.SetTrigger("Transition");
        }

        public override void OnStop()
        {
            CharacterStateManager.Animator.SetInteger("move type", 0);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && doRoll;
        }

        public override bool CanStop()
        {
            return true;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
            doRoll = false;
        }
    }
}