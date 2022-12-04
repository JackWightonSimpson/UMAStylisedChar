using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Jump : CharacterAbility
    {
        [SerializeField]
        private float jumpHeight = 1.5f;
        // private InputAction jump;
        [SerializeField]
        private bool doJump;
        [SerializeField]
        private bool startJump;
        [SerializeField]
        private Vector3 targetV;
        
        [SerializeField] private bool useEvent = true;
        public override void Init()
        {
            // jump = CharacterStateManager.PlayerInput.actions.FindAction("Jump");
            // jump.performed += (c) =>
            // {
            //     startJump = true;
            // };
        }

        private void OnJump(InputValue value)
        {
            startJump = true;
        }
        
        public override void OnStart()
        {
            targetV = CharacterStateManager.LastVelocity;
            CharacterStateManager.Animator.SetTrigger("Jump");
            if (!useEvent)
            {
                Jump_StartJump();
            }
        }

        public override void OnStop()
        {
            // CharacterStateManager.Animator.SetBool("Jump", false);
            doJump = false;
        }

        public override bool CanStart()
        {
            // doJump = true;
            return CharacterStateManager.Grounded && startJump;
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
            // if (CharacterStateManager.Grounded)
            // {
            //     CharacterStateManager.NextVelocity += targetV;
            // }
            // CharacterStateManager.NextVelocity = Vector3.Max(CharacterStateManager.NextVelocity, targetV);
            // CharacterStateManager.NextVelocity = targetV;//Vector3.Lerp(CharacterStateManager.LastVelocity, targetV, CharacterStateManager.activeConfig.Acceleration * Time.deltaTime);
            if (doJump)
            {
                CharacterStateManager.NextVelocity -= CharacterStateManager.NextVelocity.y * Vector3.up;
                CharacterStateManager.NextVelocity += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Vector3.up;
                targetV = CharacterStateManager.NextVelocity;
                CharacterStateManager.state.fallTimeout = 0;
                doJump = false;
            }
        }

        public override void Cleanup()
        {
            startJump = false;
        }

        private void Jump_StartJump()
        {
            doJump = Active;

        }
    }
}