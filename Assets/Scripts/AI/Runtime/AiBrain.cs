using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Simpson.AI
{
    public class AiBrain : MonoBehaviour
    {
        [SerializeField] private LayerMask targetsMark;
        [SerializeField] private float detectRadius = 10;

        [SerializeField] public Collider[] targets = new Collider[0];


        [SerializeField] private AiBehaviour[] behaviours;
        [SerializeField] private AiBehaviour active;
        [SerializeField] public Vector3 patrolOrigin;

        private void Awake()
        {
            behaviours = GetComponents<AiBehaviour>();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.Paused)
            {
                return;
            }
            targets = Physics.OverlapSphere(transform.position, detectRadius, targetsMark);

            if (active != null && active.CanStop())
            {
                active.Active = false;
                active = null;
            }
            var next = active;
            foreach (var behaviour in behaviours)
            {
                if (behaviour.CanStart())
                {
                    next = behaviour;
                    break;
                }
                if (behaviour == active)
                {
                    break;
                }
            }
            if (next != active)
            {
                if (active != null)
                {
                    active.Active = false;
                }

                if (next != null)
                {
                    next.Active = true;
                }

                active = next;
            }

            if (active != null)
            {
                active.DoUpdate();
            }

        }
    }
}