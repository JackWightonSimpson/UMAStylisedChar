using System;
using UnityEngine;

namespace Simpson.Character
{
    public class Hitbox : MonoBehaviour
    {

        public event Action<Collider> OnCollision;

        private void OnTriggerExit(Collider other)
        {
            if (OnCollision != null)
            {
                OnCollision(other);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OnCollision != null)
            {
                OnCollision(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (OnCollision != null)
            {
                OnCollision(other);
            }
        }
    }
}