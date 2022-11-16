using System;
using UnityEngine;

namespace GameSystem.SaveLoad.TypeHandling
{
    public class TransformSaver : TypeSaver<Transform>
    {
        public override string Serialise(Transform data)
        {
            // position = xform.position;
            // rotation = xform.rotation;
            // scale = xform.localScale;
            return ""; //JsonUtility.ToJson(new Tr);
        }
        
        public override void DeSerialise(string data, Transform target)
        {
            var transformData = JsonUtility.FromJson<TransformData>(data);
            if (target.TryGetComponent<CharacterController>(out var characterController))
            {
                characterController.enabled = false;
                target.position = transformData.position;
                target.rotation = transformData.rotation;
                target.localScale = transformData.scale;
                characterController.enabled = true;
            }
            else
            {
                target.position = transformData.position;
                target.rotation = transformData.rotation;
                target.localScale = transformData.scale;
            }
        }
    }

    [Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }
}