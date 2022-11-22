using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Crouch : CharacterAbility
    {
        [SerializeField] private bool shouldCrouch;
        
        public override void Init()
        {
            // action = CharacterStateManager.PlayerInput.actions.FindAction("Crouch");
        }

        private void OnCrouch(InputValue value)
        {
            shouldCrouch = !shouldCrouch;
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
            return CharacterStateManager.Grounded && shouldCrouch;
        }

        public override bool CanStop()
        {
            return !shouldCrouch || !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
        }
    }
}