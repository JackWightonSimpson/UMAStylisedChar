using UnityEngine;

namespace Simpson.Character.Abilities
{
    public class RootMotionMove : CharacterAbility
    {
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override bool CanStart()
        {
            return CharacterStateManager.UseRootMotion;
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.UseRootMotion;
        }

        public override void UpdateCharacter()
        {
            CharacterStateManager.NextVelocity += CharacterStateManager.RootMotionMove / Time.deltaTime;
        }

        public override void Cleanup()
        {
        }
    }
}