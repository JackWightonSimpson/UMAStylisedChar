using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Run : CharacterAbility
    {
        private InputAction attack;     
        
        public override void Init()
        {
            attack = CharacterStateManager.PlayerInput.actions.FindAction("Sprint");
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