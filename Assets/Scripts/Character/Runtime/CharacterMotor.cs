using System.Collections.Generic;
using System.Linq;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class CharacterMotor : MonoBehaviour
    {
        private CharacterController controller;
        private PlayerInput playerInput;
        private Animator animator;

        [SerializeField] private Transform cameraTransform;

        [SerializeField] private float rotationalAcceleration = 10;
        
        [SerializeField] private float jumpHeight = 3f;

        [SerializeField] private MovementState grounded = new MovementState();
        [SerializeField] private MovementState inAir = new MovementState();
        // [SerializeField] private CharacterAction[] actions;
        
        [SerializeField] private SkinnedMeshRenderer baseRenderer;

        private Vector3 rootMotionForce;
        private Vector3 previous;

        private InputAction move;
        private InputAction attack;
        private InputAction jump;


        private AnimatorOverrideController overrideController;
        private RuntimeAnimatorController runtimeAnimator;

        private string[] actionSlots = new[] {"Action1", "Action2", "Action3"};
        [SerializeField] 
        private AnimationClip[] actionSlots2;
        private int actionIndex = 0;

        // private CharacterAction activeAction;

        private List<MovementState> movementStates = new List<MovementState>();
    
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<Animator>();

            runtimeAnimator = animator.runtimeAnimatorController;
            overrideController = new AnimatorOverrideController(runtimeAnimator);
            animator.runtimeAnimatorController = overrideController;
            runtimeAnimator = overrideController;
            // animator
            
            move = playerInput.actions.FindAction("Move");
            attack = playerInput.actions.FindAction("Attack");
            jump = playerInput.actions.FindAction("Jump");
            
            movementStates.Add(grounded);

            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinnedMeshRenderer != baseRenderer)
                {
                    skinnedMeshRenderer.bones = baseRenderer.bones;

                }
            }
        }
        
        void FootL() {}

        void FootR() {}
        
        void Land() {}
        
        void Hit() {}
    
        // Update is called once per frame
        void Update()
        {
            animator.SetBool("Grounded", controller.isGrounded);
            if (controller.isGrounded)
            {
                movementStates[0] = grounded;
            }
            else
            {
                movementStates[0] = inAir;
            }

            // if (activeAction != null)
            // {
            //     if (animator.GetInteger("ActiveAction") != actionIndex+1)
            //     {
            //         EndAction(activeAction);
            //     }
            // }
            // else
            // {
            //     // foreach (var action in actions)
            //     // {
            //     //     if (IsInputTriggered(action.input) && 
            //     //         (!action.requiresGround || controller.isGrounded) &&
            //     //         (!action.requiresAir || !controller.isGrounded))
            //     //     {
            //     //         PlayAction(action);
            //     //         break;
            //     //     }
            //     // }
            // }
            //
            //
            // animator.SetBool("Attack", attack.IsPressed());
            //
            // animator.SetBool("Jump", jump.IsPressed() && controller.isGrounded);
            // if (jump.IsPressed() && controller.isGrounded)
            // {
            //     previous = new Vector3(previous.x, Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), previous.z);
            // }
        }

        private void FixedUpdate()
        {
            var moveState = movementStates.Last();
            var fall = moveState.canFall ?  (previous.y + Physics.gravity.y*Time.deltaTime) * Vector3.up : Vector3.zero;
            var moveDir = move.ReadValue<Vector2>();
            var xForm = controller.transform;
            
            var direction = xForm.forward * moveDir.y + xForm.right * moveDir.x;
            direction = direction.normalized;
            var speed = Vector3.Lerp(new Vector3(previous.x, 0, previous.z), 
                xForm.forward*moveState.speed,
                moveState.acceleration * Time.deltaTime);
        
            animator.SetFloat("Forward", moveDir.magnitude);
        
            if (moveDir.magnitude > 0.01f)
            {
                var forward = cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
                var right = cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;
                RotateToTarget(forward*moveDir.y + right*moveDir.x);
            }

            var rm = moveState.useRootMotion ? rootMotionForce : speed*Time.deltaTime;// transform.InverseTransformVector(rootMotionForce);
            // var rm = transform.forward * moveDir.magnitude * Time.deltaTime;
            controller.Move(rm + fall * Time.deltaTime);
            previous = controller.velocity;
            this.rootMotionForce = Vector3.zero;
        }


        // public void PlayAction(CharacterAction action)
        // {
        //     overrideController[actionSlots[actionIndex]] = action.animation;
        //     // overrideController.ApplyOverrides();
        //     // overrideController.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>()
        //     // {
        //     //     new KeyValuePair<AnimationClip, AnimationClip>(actionSlots2[actionIndex], action.animation)
        //     // });
        //     animator.CrossFade(actionSlots[actionIndex],0.25f,1);
        //     // animator.SetBool("Loop1", action.loop);
        //     // animator.SetBool("", action.loop);
        //     if (action.setVelocity)
        //     {
        //         previous = action.velocity;
        //     }
        //     movementStates.Add(action.movementState);
        //     activeAction = action;
        //     
        // }
        //
        // public void EndAction(CharacterAction action)
        // {
        //     movementStates.Remove(action.movementState);
        //     activeAction = null;
        // }
        
        public bool IsInputTriggered(string action)
        {
            return playerInput.actions[action].IsPressed();
        }
        

        public Vector3 Velocity()
        {
            return previous;
        }
        
        private void RotateToTarget(Vector3 targetHeading)
        {
            var rot = transform.rotation.eulerAngles.y;
            var targetRot = Vector3.SignedAngle(Vector3.forward, targetHeading, Vector3.up);

            
            
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,targetRot,0), rotationalAcceleration * Time.deltaTime);
            
            // transform.Rotate(Vector3.up, Mathf.LerpAngle(rot, targetRot, rotationalAcceleration * Time.deltaTime));
        }

        private void OnAnimatorMove ()
        {
            this.rootMotionForce += this.animator.deltaPosition;// / Time.deltaTime;
        }
    }
}
