using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class FpsMovement : CharacterAbility
    {
        private InputAction move;
        private InputAction toggle;
        public float turnAcceleration = 10f;

        [SerializeField] private CinemachineVirtualCamera defaultCam;
        [SerializeField] private CinemachineVirtualCamera aimCam;

        public bool toggleActivated = false;
        public bool stopActivated = false;
        private bool activated = false;
        
        public override void Init()
        {
            move = CharacterStateManager.playerInput.actions.FindAction("Move");
            toggle = CharacterStateManager.playerInput.actions.FindAction("ToggleFp");
            toggle.performed += c =>
            {
                toggleActivated = !Active;
                stopActivated = Active;
            };
        }

        public override void OnStart()
        {
            aimCam.Priority = 11;
            defaultCam.Priority = 9;
            activated = true;
        }

        public override void OnStop()
        {
            aimCam.Priority = 9;
            defaultCam.Priority = 11;
            CharacterStateManager.animator.SetFloat("Move", 0f);
            activated = false;
        }

        public override bool CanStart()
        {
            return toggleActivated && !Active;
        }

        public override bool CanStop()
        {
            return stopActivated;
        }

        public override void UpdateCharacter()
        {
            toggleActivated = false;
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
            toggleActivated = false;
        }
    }
}