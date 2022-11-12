using UnityEngine;

namespace Simpson.Character.Abilities
{
    public class Decelerate : CharacterAbility
    {
        public float acceleration = 2f;
        
        
        private Vector3 v = Vector3.zero; 
        
        public override void OnStart()
        {
            v = CharacterStateManager.LastVelocity;
            v.y = 0;
            CharacterStateManager.UseRootMotion = false;
        }

        public override void OnStop()
        {
            CharacterStateManager.UseRootMotion = true;
        }

        public override bool CanStart()
        {
            return !CharacterStateManager.Grounded;
        }

        public override bool CanStop()
        {
            return CharacterStateManager.Grounded;
        }

        public override void UpdateCharacter()
        {
            // var v = CharacterStateManager.LastVelocity;
            v = Vector3.Lerp(v, Vector3.zero, acceleration * Time.deltaTime);
            CharacterStateManager.NextVelocity += v;
        }

        public override void Cleanup()
        {
        }
    }
}