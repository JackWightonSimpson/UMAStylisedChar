using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Run : CharacterAbility
    {
        private bool run;     
        
        public override void Init()
        {
            // attack = CharacterStateManager.PlayerInput.actions.FindAction("Sprint");
        }
        
        private void OnSprint(InputValue value)
        {
            run = !run;//value.isPressed;
        }
        
        public override void OnStart()
        {
            CharacterStateManager.Animator.SetBool("Run", true);
        }

        public override void OnStop()
        {
            CharacterStateManager.Animator.SetBool("Run", false);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && run;
        }

        public override bool CanStop()
        {
            return !run || !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
        }
    }
}