using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class FpsMovement : CharacterAbility
    {
        private Vector2 move;
        private Vector2 look;
        // private InputAction toggle;
        public float turnAcceleration = 10f;

        [SerializeField] private CinemachineVirtualCamera defaultCam;
        [SerializeField] private CinemachineVirtualCamera aimCam;
        [SerializeField] private Transform camTarget;

        public bool toggleActivated = false;
        public bool stopActivated = false;
        
        public override void Init()
        {
            // move = CharacterStateManager.PlayerInput.actions.FindAction("Move");
            // look = CharacterStateManager.PlayerInput.actions.FindAction("Look");
            // toggle = CharacterStateManager.PlayerInput.actions.FindAction("ToggleFp");
            // toggle.performed += c =>
            // {
            //     toggleActivated = !Active;
            //     stopActivated = Active;
            // };
        }

        private void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }
        
        private void OnLook(InputValue value)
        {
            look = value.Get<Vector2>();
        }

        private void OnToggleFp(InputValue value)
        {
            toggleActivated = !Active;
            stopActivated = Active;
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
            return toggleActivated && !Active;
        }

        public override bool CanStop()
        {
            return stopActivated;
        }

        public override void UpdateCharacter()
        {
            toggleActivated = false;
            var moveDir = move;
            
            CharacterStateManager.Animator.SetFloat("Forward", moveDir.y);
            CharacterStateManager.Animator.SetFloat("Side", moveDir.x);
            
            var lookVector = look;
            var currentRot = Vector3.SignedAngle(Vector3.forward, CharacterStateManager.transform.forward, Vector3.up);
            // var currentPitch = Vector3.SignedAngle(Vector3.up, aimCam.transform.up, Vector3.right);
            if (lookVector.sqrMagnitude >= 0.01f)
            {
                currentRot += lookVector.x * turnAcceleration;
                // currentPitch += lookVector.y * turnAcceleration;
            }

            // clamp our rotations so our values are limited 360 degrees
            currentRot = ClampAngle(currentRot, float.MinValue, float.MaxValue);

            CharacterStateManager.NextOrientation =
                Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,currentRot,0), CharacterStateManager.activeConfig.TurnAcceleration * Time.deltaTime);
            
            
            // aimCam.transform.rotation =
            //     Quaternion.Slerp(aimCam.transform.rotation,Quaternion.Euler(currentPitch,0,0), turnAcceleration * Time.deltaTime);
            
        }

        public override void Cleanup()
        {
            toggleActivated = false;
        }
        
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}