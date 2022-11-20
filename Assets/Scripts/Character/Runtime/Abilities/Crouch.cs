using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Crouch : CharacterAbility
    {
        private InputAction action;     
        
        public override void Init()
        {
            action = CharacterStateManager.PlayerInput.actions.FindAction("Crouch");
        }
        
        public override void OnStart()
        {
            CharacterStateManager.Animator.SetBool("Crouching", true);
        }

        public override void OnStop()
        {
            CharacterStateManager.Animator.SetBool("Crouching", false);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && action.IsPressed();
        }

        public override bool CanStop()
        {
            return !action.IsPressed() || !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
        }
    }
}