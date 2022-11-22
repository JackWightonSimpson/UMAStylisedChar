using Simpson.World;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Interact : CharacterAbility
    {
        // private InputAction interact;
        private Interactor Interactor;
        private bool interact;
        
        public override void Init()
        {
            Interactor = GetComponent<Interactor>();
            // interact = CharacterStateManager.PlayerInput.actions.FindAction("Interact");
        }
        
        public void OnInteract(InputValue value)
        {
            interact = true;//value.;
        }
        
        public override void OnStart()
        {
            CharacterStateManager.Animator.CrossFade("Interact", 0.2f);
        }

        public override void OnStop()
        {
            Interactor.EndInteraction();
        }

        public override bool CanStart()
        {
            return Interactor.GetInteractionTarget() != null && interact;
        }

        public override bool CanStop()
        {
            return !Interactor.IsInRange();
        }

        public override void UpdateCharacter()
        {
            //If
        }

        public override void Cleanup()
        {
            interact = false;
        }
    }
}