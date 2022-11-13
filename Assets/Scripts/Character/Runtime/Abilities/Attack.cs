﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simpson.Character.Abilities
{
    public class Attack : CharacterAbility
    {
        private InputAction attack;     
        
        [SerializeField] private List<Hitbox> hitBoxes = new List<Hitbox>();

        [SerializeField]
        private Animator animator;

        private List<GameObject> hit = new List<GameObject>();
        
        
        private Hitbox activeCollider;

        [SerializeField]
        private bool attacking;
        
        [SerializeField]
        private float hitForce = 20f;
        
        
        public override void Init()
        {
            attack = CharacterStateManager.playerInput.actions.FindAction("Attack");
        }
        
        public override void OnStart()
        {
            CharacterStateManager.animator.SetBool("Attack", true);
        }

        public override void OnStop()
        {
            CharacterStateManager.animator.SetBool("Attack", false);
        }

        public override bool CanStart()
        {
            return CharacterStateManager.Grounded && attack.IsPressed();
        }

        public override bool CanStop()
        {
            return  !attack.IsPressed()  || !CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
        }

        public override void Cleanup()
        {
        }
        
        private void OnHit(Collider collider)
        {
            if (!hit.Contains(collider.gameObject))
            {
                hit.Add(collider.gameObject);
                Debug.Log("Hit: "+collider.gameObject.name);
                if (collider.gameObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    Debug.Log("Pushing: "+collider.gameObject.name);
                    rb.AddForce(transform.forward*hitForce);
                }
            }
        }
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            foreach (var hitBox in hitBoxes)
            {
                hitBox.gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (attacking && !animator.GetBool("Attacking"))
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            hit.Clear();
            attacking = true;
            animator.SetBool("Attacking", true);
            activeCollider = hitBoxes[animator.GetInteger("Hitbox")];
            activeCollider.gameObject.SetActive(true);
            activeCollider.OnCollision += OnHit;
        }

        private void Deactivate()
        {
            hit.Clear();
            attacking = false;
            if (activeCollider != null)
            {
                activeCollider.gameObject.SetActive(false);
                activeCollider.OnCollision -= OnHit;
                activeCollider = null;
            }
        }

        private void Attack_HitboxOn()
        {
            Activate();
        }
        
        
        private void Attack_HitboxOff()
        {
            Deactivate();
        }
        
        private void Attack_CanComboOn()
        {
        }
        
        
        private void Attack_CanComboOff()
        {
        }
    }
}