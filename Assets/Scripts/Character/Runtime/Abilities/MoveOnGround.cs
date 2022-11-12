using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class MoveOnGround : CharacterAbility
    {
        private InputAction move;
        private InputAction aim;       
        
        public float turnAcceleration = 10f;

        public override void Init()
        {
            move = CharacterStateManager.playerInput.actions.FindAction("Move");
            aim = CharacterStateManager.playerInput.actions.FindAction("Aim");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
            CharacterStateManager.animator.SetFloat("Move", 0f);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && !aim.IsPressed();
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.Grounded || aim.IsPressed();
        }

        public override void UpdateCharacter()
        {
            var moveDir = move.ReadValue<Vector2>();
            var xForm = transform;
            CharacterStateManager.animator.SetFloat("Forward", moveDir.magnitude);
            CharacterStateManager.animator.SetFloat("Side", 0f);
            CharacterStateManager.animator.SetFloat("Move", moveDir.magnitude);
            
            if (moveDir.magnitude > 0.01f)
            {
                
                var forward = CharacterStateManager.cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
                var right = CharacterStateManager.cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;
                var targetRot = Vector3.SignedAngle(Vector3.forward, forward*moveDir.y + right*moveDir.x, Vector3.up);
                CharacterStateManager.NextOrientation =
                    Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,targetRot,0), turnAcceleration * Time.deltaTime);
            }
        }

        public override void Cleanup()
        {
        }
    }
}