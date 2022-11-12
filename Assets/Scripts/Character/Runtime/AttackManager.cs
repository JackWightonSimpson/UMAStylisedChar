using System.Collections.Generic;
using UnityEngine;

namespace Simpson.Character
{
    public class AttackManager : MonoBehaviour
    {
        [SerializeField] private List<Collider> hitBoxes = new List<Collider>();

        [SerializeField]
        private Animator animator;

        private List<GameObject> hit = new List<GameObject>();

        [SerializeField]
        private bool active;

        private void OnHit()
        {
            
        }
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            
            // hitBoxes
            
        }

        private void Activate()
        {
            hit.Clear();
            active = true;
        }
    }
}