using UnityEngine;

namespace Simpson.World
{
    public class Trigger : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.TryGetComponent<Interactor>(out var trigger))
            {
                trigger.triggers.Add(this);
            }
        }

        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Interactor>(out var trigger))
            {
                trigger.triggers.Remove(this);
            }
        }
    }
}