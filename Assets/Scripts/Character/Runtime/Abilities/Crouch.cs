using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Crouch : CharacterAbility
    {
        private InputAction attack;     
        
        public override void Init()
        {
            attack = CharacterStateManager.playerInput.actions.FindAction("Crouch");
        }
        
        public override void OnStart()
        {
            CharacterStateManager.animator.SetBool("Crouching", true);
        }

        public override void OnStop()
        {
            CharacterStateManager.animator.SetBool("Crouching", false);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && attack.IsPressed();
        }

        public override bool CanStop()
        {
            return !attack.IsPressed() || !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
        }
    }
}