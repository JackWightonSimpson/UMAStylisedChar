using UnityEngine;

namespace Simpson.AI
{
    public abstract class AiBehaviour : MonoBehaviour
    {

        private bool active = false;
        [field:SerializeField]
        public bool Active
        {
            get => active;
            set
            {
                if (!active && value)
                {
                    Enter();
                } 
                if (active && !value)
                {
                    Exit();
                } 
                active = value;
            }
        }

        public abstract bool CanStart();
        
        public abstract bool CanStop();

        
        protected abstract void Enter();
        protected abstract void Exit();
        
        public abstract void DoUpdate();
    }
}