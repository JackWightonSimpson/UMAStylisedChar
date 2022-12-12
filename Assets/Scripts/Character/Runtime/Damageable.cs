using UnityEngine;

namespace Simpson.Character
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private float hp = 100;
        [SerializeField] private float cooldown = 2;
        [SerializeField] private float time = 2;
        [SerializeField] private float threshold = 5;

        [SerializeField] private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnDamaged(float damage)
        {
            hp = Mathf.Max(0f, hp - damage);
            // if (damage * (time / cooldown) > threshold)
            // {
                animator.SetTrigger("hit");
            // }

            time = Mathf.Max(0f, time - damage);
        }

        private void FixedUpdate()
        {
            if (time < cooldown)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = cooldown;
            }
        }
    }
}