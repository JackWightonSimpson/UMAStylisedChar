using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class AimMovement : CharacterAbility
    {
        private InputAction move;
        private InputAction aim;
        public float turnAcceleration = 10f;

        [SerializeField] private CinemachineVirtualCamera defaultCam;
        [SerializeField] private CinemachineVirtualCamera aimCam;
        
        public override void Init()
        {
            move = CharacterStateManager.PlayerInput.actions.FindAction("Move");
            aim = CharacterStateManager.PlayerInput.actions.FindAction("Aim");
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
            CharacterStateManager.Animator.SetFloat("Move", 0f);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && aim.IsPressed();
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.Grounded || !aim.IsPressed();
        }

        public override void UpdateCharacter()
        {
            var moveDir = move.ReadValue<Vector2>();
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