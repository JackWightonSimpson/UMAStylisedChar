using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character
{
    public class CharacterStateManager : MonoBehaviour
    {

        [SerializeField]
        private List<CharacterAbility> abilities = new List<CharacterAbility>();

        [SerializeField] 
        private CharacterAbility activeAbility;
        
        
        [field:SerializeField] 
        public Vector3 LastVelocity { get; private set; }
        
        [field:SerializeField] 
        public Vector3 NextVelocity { get; set; }
        
        [field:SerializeField] 
        public Vector3 RootMotionMove { get; private set; }
        
        [field:SerializeField] 
        public Quaternion LastOrientation { get; private set; }
        
        [field:SerializeField] 
        public Quaternion NextOrientation { get; set; }
        
        

        [SerializeField] public Transform cameraTransform;
        
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

        public void FixedUpdate()
        {
            //update env data
            NextVelocity = Vector3.zero;

            //Test Conditions
            animator.SetBool("Grounded", controller.isGrounded);
            
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
            LastVelocity = controller.velocity;
            transform.localRotation = NextOrientation;
            LastOrientation = transform.localRotation;
            
            foreach (var characterAbility in abilities)
            {
                characterAbility.Cleanup();
            }
            this.RootMotionMove = Vector3.zero;
        }

        public bool Grounded => controller.isGrounded;

        public bool Swimming => false;
        
        public bool CanFall => true;

        public bool UseRootMotion { get; set; } = true;

        private void OnAnimatorMove ()
        {
            this.RootMotionMove += this.animator.deltaPosition;
        }
    }
}