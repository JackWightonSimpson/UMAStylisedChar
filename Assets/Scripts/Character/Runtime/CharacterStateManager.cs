using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.SaveLoad;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character
{
    public class CharacterStateManager : MonoBehaviour, ISaveSerializable
    {

        [SerializeField]
        private List<CharacterAbility> abilities = new List<CharacterAbility>();

        [SerializeField] 
        private CharacterAbility activeAbility;

        private State state = new State();

        [field: SerializeField] public Vector3 LastVelocity => state.LastVelocity;
        
        [field:SerializeField] 
        public Vector3 NextVelocity {
            get => state.NextVelocity;
            set => state.NextVelocity = value;
        }
        
        [field:SerializeField] 
        public Vector3 RootMotionMove => state.RootMotionMove;
        
        [field:SerializeField] 
        public Quaternion LastOrientation => state.LastOrientation;
        
        [field:SerializeField] 
        public Quaternion NextOrientation {
            get => state.NextOrientation;
            set => state.NextOrientation = value;
        }
        
        

        [SerializeField] public Transform cameraTransform;
        [field:SerializeField] 
        public LayerMask GroundLayers { get; set; }
        [field:SerializeField] 
        public bool Grounded { get; private set; }

        [field: SerializeField] public float GroundCheckOffset { get; private set; } = -.02f;
        [field: SerializeField] 
        public float GroundCheckRadius { get; private set; } = -.01f;
        
        [field:HideInInspector]
        [field:SerializeField] 
        public CharacterController controller { get; private set; }
        
        [field:HideInInspector]
        [field:SerializeField] 
        public PlayerInput playerInput { get; private set; }
        
        [field:HideInInspector]
        [field:SerializeField] 
        public Animator animator { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<Animator>();
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            SetupStates();
        }

        private void OnEnable()
        {
            SetupStates();
        }
        
        private void SetupStates()
        {
            var states = GetComponents<CharacterAbility>();
            var ordered = states.OrderBy(s => s.Order);
            foreach (var state in states)
            {
                if (!abilities.Contains(state))
                {
                    abilities.Add(state);
                }
            }
            foreach (var characterAbility in abilities)
            {
                characterAbility.Initialise();
            }
        }
        
        void FootL() {}

        void FootR() {}
        
        void Land() {}
        
        void Hit() {}

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + controller.radius + GroundCheckOffset, transform.position.z);
            //TODO: normals
            Grounded = Physics.CheckSphere(spherePosition, controller.radius + GroundCheckRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            animator.SetBool("Grounded", Grounded);
        }


        public void FixedUpdate()
        {
            //update env data
            NextVelocity = Vector3.zero;
            state.RootMotionMove -= state.RootMotionMove.y*Vector3.up;
            

            //Test Conditions
            // animator.SetBool("Grounded", controller.isGrounded);
            GroundedCheck();
            
            foreach (var characterAbility in abilities)
            {
                characterAbility.TryStop();
            }
            foreach (var characterAbility in abilities)
            {
                characterAbility.TryStart();
            }
            
            foreach (var characterAbility in abilities)
            {
                if (characterAbility.Active)
                {
                    characterAbility.UpdateCharacter();
                }
            }

            controller.Move(NextVelocity * Time.deltaTime);
            state.LastVelocity = controller.velocity;
            transform.localRotation = NextOrientation;
            animator.SetFloat("turnDelta",Mathf.Rad2Deg*Quaternion.Angle(LastOrientation, NextOrientation));
            state.LastOrientation = transform.localRotation;
            
            foreach (var characterAbility in abilities)
            {
                characterAbility.Cleanup();
            }
            state.RootMotionMove = Vector3.zero;
            animator.SetFloat("forwardSpeed",Vector3.Dot(state.LastVelocity, transform.forward));
            animator.SetFloat("sideSpeed",Vector3.Dot(state.LastVelocity, transform.right));
            animator.SetFloat("verticalSpeed",Vector3.Dot(state.LastVelocity, transform.up));
        }
        

        public bool Swimming => false;
        
        public bool CanFall => true;

        public bool UseRootMotion { get; set; } = true;

        private void OnAnimatorMove ()
        {
            state.RootMotionMove += this.animator.deltaPosition;
        }
        
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
			
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + controller.radius + GroundCheckOffset, transform.position.z);
            //TODO: normals
            // Grounded = Physics.CheckSphere(spherePosition, , GroundLayers, QueryTriggerInteraction.Ignore);
            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(spherePosition, controller.radius + GroundCheckRadius);
            // Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + GroundCheckRadius + _controller.height, transform.position.z), GroundedRadius);
        }

        public void SetSaveState(ObjectSaveData data)
        {
            data.savedStates["CharacterStateManagerState"] = new SavedState {data = JsonUtility.ToJson(state, true)};
            
            data.savedStates["CharacterStateManagerAbilities"] = new SavedState {data = JsonUtility.ToJson(
                abilities.Where(a => a.Active).Select(a => a.name).ToList(), 
                true)};
        }

        public void LoadSaveState(ObjectSaveData data)
        {
            if (data.savedStates.TryGetValue("CharacterStateManagerState", out var stateData))
            {
                JsonUtility.FromJsonOverwrite(stateData.data, state);
            }
            if (data.savedStates.TryGetValue("CharacterStateManagerAbilities", out var activeAbilityData))
            {
                var activeAbilities = JsonUtility.FromJson<List<string>>(activeAbilityData.data);
                foreach (var characterAbility in abilities)
                {
                    if (activeAbilities.Contains(characterAbility.name))
                    {
                        characterAbility.TryStart();
                    }
                    else if(characterAbility.Active)
                    {
                        characterAbility.TryStop();
                    }
                }
            }
        }

        [Serializable]
        private class State
        {

            [field:SerializeField] 
            public Vector3 LastVelocity { get; set; }
        
            [field:SerializeField] 
            public Vector3 NextVelocity { get; set; }
        
            [field:SerializeField] 
            public Vector3 RootMotionMove { get; set; }
        
            [field:SerializeField] 
            public Quaternion LastOrientation { get; set; }
        
            [field:SerializeField] 
            public Quaternion NextOrientation { get; set; }
        
            [field:SerializeField] 
            public string ActiveAbilityName { get; set; }
        }
    }
}