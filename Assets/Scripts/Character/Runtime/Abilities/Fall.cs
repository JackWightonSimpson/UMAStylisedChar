using UnityEngine;

namespace Simpson.Character.Abilities
{
    public class Fall : CharacterAbility
    {
        public override void OnStart()
        {
            CharacterStateManager.controller.stepOffset = 0f;
        }

        public override void OnStop()
        {
            CharacterStateManager.controller.stepOffset = 0.3f;
        }

        public override bool CanStart()
        {
            return CharacterStateManager.CanFall;
        }

        public override bool CanStop()
        {
            return !CharacterStateManager.CanFall;
        }

        public override void UpdateCharacter()
        {
            CharacterStateManager.NextVelocity += (CharacterStateManager.LastVelocity.y*Vector3.up + Physics.gravity * Time.deltaTime);
        }

        public override void Cleanup()
        {
        }
    }
}