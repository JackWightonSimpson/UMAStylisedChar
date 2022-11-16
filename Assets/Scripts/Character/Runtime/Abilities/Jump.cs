using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Jump : CharacterAbility
    {
        [SerializeField]
        private float jumpHeight = 1.5f;
        private InputAction jump;     
        
        public override void Init()
        {
            jump = CharacterStateManager.playerInput.actions.FindAction("Jump");
        }
        
        public override void OnStart()
        {
            // CharacterStateManager.animator.SetBool("Jump", true);
        }

        public override void OnStop()
        {
            // CharacterStateManager.animator.SetBool("Jump", false);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && jump.IsPressed();
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
            CharacterStateManager.NextVelocity += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Vector3.up;
        }

        public override void Cleanup()
        {
        }
    }
}