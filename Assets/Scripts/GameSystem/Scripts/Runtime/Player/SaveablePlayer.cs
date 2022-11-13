using GameSystem.SaveLoad;
using UnityEngine;

namespace GameSystem.Player
{
    public class SaveablePlayer : MonoBehaviour
    {

        public Transform playerTransform;
        
        public void SetSaveState(ObjectSaveData data)
        {
            data.savedStates["player_transform"] = new SavedState
            {
                data = JsonUtility.ToJson(transform)
            };
            if (TryGetComponent<CharacterController>(out var controller))
            {
                data.savedStates["player_CharacterController"] = new SavedState
                {
                    data = JsonUtility.ToJson(controller)
                };
            }
            if (TryGetComponent<Animator>(out var animator))
            {
                data.savedStates["player_Animator"] = new SavedState
                {
                    data = JsonUtility.ToJson(animator)
                };
            }
            foreach (var serializable in GetComponents<ISaveSerializable>())
            {
                serializable.SetSaveState(data);
            }
        }


        public void LoadSaveState(ObjectSaveData data)
        {
            if (data.savedStates.TryGetValue("player_transform", out var xformState))
            {
                JsonUtility.FromJsonOverwrite(xformState.data, transform);
            }
            if (data.savedStates.TryGetValue("player_CharacterController", out var controllerState))
            {
                JsonUtility.FromJsonOverwrite(controllerState.data, GetComponent<CharacterController>());
            }
            if (data.savedStates.TryGetValue("player_Animator", out var animator))
            {
                JsonUtility.FromJsonOverwrite(animator.data, GetComponent<Animator>());
            }
            foreach (var serializable in GetComponents<ISaveSerializable>())
            {
                serializable.LoadSaveState(data);
            }
        }
        
    }
}