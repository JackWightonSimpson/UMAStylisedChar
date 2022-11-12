using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Run : CharacterAbility
    {
        private InputAction attack;     
        
        public override void Init()
        {
            attack = CharacterStateManager.playerInput.actions.FindAction("Sprint");
        }
        
        public override void OnStart()
        {
            CharacterStateManager.animator.SetBool("Run", true);
        }

        public override void OnStop()
        {
            CharacterStateManager.animator.SetBool("Run", false);
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