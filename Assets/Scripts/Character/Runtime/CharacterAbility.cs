using UnityEngine;

namespace Simpson.Character
{
    public abstract class CharacterAbility : MonoBehaviour
    {

        // private CharacterStateManager characterStateManager;
        protected CharacterStateManager CharacterStateManager { get; private set; }
        
        [field:SerializeField]
        public int Order { get; private set; }
        
        [field:SerializeField]
        public bool Active { get; private set; }


        public void Initialise()
        {
            CharacterStateManager = GetComponent<CharacterStateManager>();
            Init();
        }

        public virtual void Init()
        {
            
        }
        
        public abstract void OnStart();
        public abstract void OnStop();
        public abstract bool CanStart();
        public abstract bool CanStop();

        public void TryStart()
        {
            if (!Active && CanStart())
            {
                Active = true;
                OnStart();
            }
        }
        public void TryStop()
        {
            if (Active && CanStop())
            {
                Active = false;
                OnStop();
            }
        }

        
        public abstract void UpdateCharacter();


        public abstract void Cleanup();
    }
}