using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simpson.World
{
    public class Interactor : MonoBehaviour
    {

        public List<Trigger> triggers = new List<Trigger>();

        private Trigger interactionTarget;
        private Trigger currentTarget;

        private void Update()
        {
            interactionTarget = currentTarget != null ? currentTarget : triggers.FirstOrDefault();
        }

        public Trigger GetInteractionTarget()
        {
            return interactionTarget;
        }

        public void StartInteraction(Trigger t)
        {
            currentTarget = t;
        }
        
        public void EndInteraction()
        {
            currentTarget = null;
        }

        public bool IsInteracting()
        {
            return currentTarget != null;
        }
        
        
        public bool IsInRange()
        {
            return currentTarget != null && triggers.Contains(currentTarget);
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (TryGetComponent<Trigger>(out var trigger))
            {
                triggers.Add(trigger);
            }
        }

        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (TryGetComponent<Trigger>(out var trigger))
            {
                triggers.Remove(trigger);
            }
        }
    }
}