using Simpson.World;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Interact : CharacterAbility
    {
        private InputAction interact;
        private Interactor Interactor;
        
        public override void Init()
        {
            Interactor = GetComponent<Interactor>();
            interact = CharacterStateManager.playerInput.actions.FindAction("Interact");
        }
        
        public override void OnStart()
        {
            CharacterStateManager.animator.CrossFade("Interact", 0.2f);
        }

        public override void OnStop()
        {
            Interactor.EndInteraction();
        }

        public override bool CanStart()
        {
            return Interactor.GetInteractionTarget() != null && interact.IsPressed();
        }

        public override bool CanStop()
        {
            return !Interactor.IsInRange();
        }

        public override void UpdateCharacter()
        {
            
        }

        public override void Cleanup()
        {
            
        }
    }
}