using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class AimMovement : CharacterAbility
    {
        private Vector2 move;
        private bool aim;
        public float turnAcceleration = 10f;

        [SerializeField] private CinemachineVirtualCamera defaultCam;
        [SerializeField] private CinemachineVirtualCamera aimCam;
        
        public override void Init()
        {
            // aim = CharacterStateManager.PlayerInput.actions.FindAction("Aim");
        }

        private void OnAim(InputValue value)
        {
            aim = value.isPressed;
        }
        
        private void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public override void OnStart()
        {
            aimCam.Priority = 11;
            defaultCam.Priority = 9;
        }

        public override void OnStop()
        {
            aimCam.Priority = 9;
            defaultCam.Priority = 11;
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && aim;
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.Grounded || !aim;
        }

        public override void UpdateCharacter()
        {
            var moveDir = move;
            var xForm = transform;
            CharacterStateManager.Animator.SetFloat("Forward", moveDir.y);
            CharacterStateManager.Animator.SetFloat("Side", moveDir.x);
            
                
            var targetRot = Vector3.SignedAngle(Vector3.forward, CharacterStateManager.cameraTransform.forward, Vector3.up);
            CharacterStateManager.NextOrientation =
                Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,targetRot,0), turnAcceleration * Time.deltaTime);
            
        }

        public override void Cleanup()
        {
        }
    }
}