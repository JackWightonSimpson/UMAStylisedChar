using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine.Utility;
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
        
        [SerializeField] public State state = new State();

        [SerializeField] public MovementConfig activeConfig;
        [SerializeField] private List<MovementConfig> configs;
        

        [field:SerializeField] 
        public LayerMask GroundLayers { get; set; }

        private RaycastHit[] hits = new RaycastHit[10];

        [field: SerializeField] 
        public float GroundCheckOffset { get; private set; } = -.02f;
        [field: SerializeField] 
        public float GroundCheckRadius { get; private set; } = -.01f;
        
        [field:HideInInspector]
        [SerializeField] 
        public Transform cameraTransform;
        
        [field:HideInInspector]
        [field:SerializeField] 
        public CharacterController Controller { get; private set; }
        
        [field:HideInInspector]
        [field:SerializeField] 
        public PlayerInput PlayerInput { get; private set; }
        
        [field:HideInInspector]
        [field:SerializeField] 
        public Animator Animator { get; private set; }

        private InputAction move;
        

        public bool Swimming => false;
        
        public bool CanFall => activeConfig.CanFall;

        public bool UseRootMotion => activeConfig.UseRootMotion;
        [field:SerializeField] 
        public bool Grounded { get; private set; }
        [field:SerializeField] 
        public Vector3 GroundNormal { get; private set; } = Vector3.up;

        public Vector3 LastVelocity => state.LastVelocity;
        
        public Vector3 NextVelocity {
            get => state.NextVelocity;
            set => state.NextVelocity = value;
        }
        
        public Vector3 RootMotionMove => state.RootMotionMove;
        
        public Quaternion LastOrientation => state.LastOrientation;
        
        public Quaternion NextOrientation {
            get => state.NextOrientation;
            set => state.NextOrientation = value;
        }

        
        #region setup
        
        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            PlayerInput = GetComponent<PlayerInput>();
            Animator = GetComponent<Animator>();
            if (cameraTransform == null)
            {
                //TODO: handle scene switch
                cameraTransform = Camera.main.transform;
            }

            move = PlayerInput.actions.FindAction("Move");
        }

        private void Start()
        {
            SetupStates();
            SetupConfig();
        }

        private void SetupConfig()
        {
            configs.Sort();
            activeConfig = configs.LastOrDefault();
        }

        private void OnEnable()
        {
            SetupStates();
            move = PlayerInput.actions.FindAction("Move");
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

        #endregion
        
        private void GroundedCheck()
        {
            // set sphere position, with offset
            var radius = Controller.radius;
            var position = transform.position;
            var spherePosition = new Vector3(position.x, position.y + radius, position.z);
            //TODO: normals
            var hitCount = Physics.SphereCastNonAlloc(spherePosition, radius+GroundCheckRadius, Vector3.down, hits, 
                GroundCheckOffset*-1, GroundLayers, QueryTriggerInteraction.Ignore);
            
            Grounded = hitCount > 0;
            GroundNormal = Vector3.zero;
            for (var i = 0; i < hitCount; i++)
            {
                GroundNormal += hits[i].normal;
            }

            GroundNormal = GroundNormal.normalized;
            // update animator if using character
            Animator.SetBool("Grounded", Grounded);
            if (Grounded)
            {
                // reset the fall timeout timer
                state.fallTimeoutDelta = state.FallTimeout;
                Animator.SetBool("falling", false);
                Controller.stepOffset = 0.3f;
                state.LastVelocity = new Vector3(LastVelocity.x,Mathf.Max(-2f, LastVelocity.y), LastVelocity.z);
            }
            else
            {
                if (state.fallTimeoutDelta >= 0.0f)
                {
                    state.fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (!Animator.GetBool("falling"))
                    {
                        // Animator.SetTrigger("Transition");
                    }
                    Controller.stepOffset = 0.0f;
                    Animator.SetBool("falling", true);
                }
            }
        }

        public void FixedUpdate()
        {
            NextVelocity = Vector3.zero;
            
            GroundedCheck();
            
            foreach (var characterAbility in abilities)
            {
                characterAbility.TryStop();
            }
            foreach (var characterAbility in abilities)
            {
                characterAbility.TryStart();
            }

            
            var moveDir = move.ReadValue<Vector2>();
            Animator.SetFloat("Forward",moveDir.magnitude/Math.Max(1f,moveDir.magnitude));
            Animator.SetFloat("Side", 0f);
            Animator.SetFloat("Move", moveDir.magnitude/Math.Max(1f,moveDir.magnitude));
            
            if (moveDir.magnitude > 0.01f)
            {
                var forward = cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
                var right = cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;
                var targetRot = Vector3.SignedAngle(Vector3.forward, forward*moveDir.y + right*moveDir.x, Vector3.up);
                NextOrientation =
                    Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,targetRot,0), activeConfig.TurnAcceleration * Time.deltaTime);
            }
            
            if (activeConfig.UseRootMotion)
            {
                NextVelocity = RootMotionMove / Time.deltaTime;
                NextVelocity += LastVelocity.y * Vector3.up;
            }
            else
            {
                NextVelocity = Vector3.Lerp(LastVelocity, NextVelocity, activeConfig.Acceleration * Time.deltaTime);
            }
            
            foreach (var characterAbility in abilities)
            {
                if (characterAbility.Active)
                {
                    characterAbility.UpdateCharacter();
                }
            }
            
            if (CanFall)
            {
                //TODO: better componentwise v calc ?
                NextVelocity += Physics.gravity * Time.deltaTime;
            }

            
            Controller.Move(NextVelocity * Time.deltaTime);
            state.LastVelocity = NextVelocity;
            transform.localRotation = NextOrientation;
            Animator.SetFloat("turnDelta",Mathf.Rad2Deg*Quaternion.Angle(LastOrientation, NextOrientation));
            Animator.SetFloat("forwardSpeed",Vector3.Dot(state.LastVelocity, transform.forward));
            Animator.SetFloat("sideSpeed",Vector3.Dot(state.LastVelocity, transform.right));
            Animator.SetFloat("verticalSpeed",Vector3.Dot(NextVelocity, transform.up));
            state.LastOrientation = transform.localRotation;
            state.RootMotionMove = Vector3.zero;
            
            foreach (var characterAbility in abilities)
            {
                characterAbility.Cleanup();
            }
        }

        #region events/animation
        
        private void OnAnimatorMove ()
        {
            state.RootMotionMove += this.Animator.deltaPosition;
        }

        private void AddMoveConfig(MovementConfig config)
        {
            configs.Add(config);
            SetupConfig();    
        }
        
        private void RemoveMoveConfig(MovementConfig config)
        {
            configs.Remove(config);
            SetupConfig();
        }
        
        void FootL() {}

        void FootR() {}
        
        void Land() {}
        
        void Hit() {}

        #endregion
        
        #region debug

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
			
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + Controller.radius + GroundCheckOffset, transform.position.z);
            
            Gizmos.DrawSphere(spherePosition, Controller.radius + GroundCheckRadius);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, GroundNormal);
        }
        #endregion

        #region save/load

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
        

        #endregion


        [Serializable]
        public class State
        {

            [field:SerializeField] 
            public Vector3 LastVelocity { get; set; } = Vector3.zero;
        
            [field:SerializeField] 
            public Vector3 NextVelocity { get; set; } = Vector3.zero;
        
            [field:SerializeField] 
            public Vector3 RootMotionMove { get; set; } = Vector3.zero;
        
            [field:SerializeField] 
            public Quaternion LastOrientation { get; set; } = Quaternion.identity;
        
            [field:SerializeField] 
            public Quaternion NextOrientation { get; set; } = Quaternion.identity;
        
            [field:SerializeField] 
            public string ActiveAbilityName { get; set; }
            
            [Tooltip("The height the player can jump")]
            public float JumpHeight = 1.2f;
            [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
            public float Gravity = -15.0f;
            [Space(10)]
            [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
            public float FallTimeout = 0.15f;
            public float fallTimeoutDelta;
        }
    }
}