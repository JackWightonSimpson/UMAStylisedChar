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
            move = CharacterStateManager.playerInput.actions.FindAction("Move");
            aim = CharacterStateManager.playerInput.actions.FindAction("Aim");
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
            CharacterStateManager.animator.SetFloat("Move", 0f);
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
            CharacterStateManager.animator.SetFloat("Forward", moveDir.y);
            CharacterStateManager.animator.SetFloat("Side", moveDir.x);
            CharacterStateManager.animator.SetFloat("Move", moveDir.magnitude);
            
            
            //TODO: orientation
                
            var targetRot = Vector3.SignedAngle(Vector3.forward, CharacterStateManager.cameraTransform.forward, Vector3.up);
            CharacterStateManager.NextOrientation =
                Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,targetRot,0), turnAcceleration * Time.deltaTime);
            
            // var direction = xForm.forward * moveDir.y + xForm.right * moveDir.x;
            // direction = direction.normalized;
            
            //
            // var speed = Vector3.Lerp(new Vector3(previous.x, 0, previous.z), 
            //     xForm.forward*moveState.speed,
            //     moveState.acceleration * Time.deltaTime);
            //
            // if (moveDir.magnitude > 0.01f)
            // {
            //     var forward = cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
            //     var right = cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;
            //     RotateToTarget(forward*moveDir.y + right*moveDir.x);
            // }
        }

        public override void Cleanup()
        {
        }
    }
}